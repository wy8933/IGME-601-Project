using UnityEngine;
using UnityEngine.InputSystem;
using ItemSystem;
using UnityEngine.UI;
using System.Collections;
using AudioSystem;

//[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // Player Input Bindings Script
    private PlayerInputBindings _playerInput;

    // Player Rigidbody and CapsuleCollider
    private Rigidbody _rb;
    private CapsuleCollider _capsuleCollider;

    // Mouse Sensitivity
    [Header("Mouse Sensitivity")]
    [SerializeField] public float MouseSensitivityX = 1.0f;
    [SerializeField] public float MouseSensitivityY = 1.0f;

    // Follow Camera
    [Header("Follow Camera")]
    [SerializeField] public Camera PlayerCamera;

    // Camera Yaw & Pitch
    [Header("Camera Pitch & Yaw")]
    [SerializeField] private float LookYawSpeed = 2.0f;
    [SerializeField] private float LookPitchSpeed = 1.0f;
    [SerializeField] private float LookPitchLimit = 60.0f; 
    private float _rotationX = 0.0f;

    // Camera Lean Left/Right Variables
    private bool _canLean = true;
    [Header("Leaning")]
    [SerializeField] private Transform LeanPivot;
    private float _currentLean;             // Actual value of the lean
    private float _targetLean;              // Will change as player pressed Q or E to lean
    [SerializeField] private float LeanAngle;       // Set the _targetLean depending on LeanAngle       - I found a value of 20 to work best
    [SerializeField] private float LeanSmoothing;   // Used to smooth the _currentLean to _targetLean   - I found a value of 0.3 to work best
    private float _leanVelocity;
    private bool _isLeaningLeft;
    private bool _isLeaningRight;

    // Movement Variables
    private bool _canMove = true;
    [Header("Movement")]
    static public float WalkSpeed = 3.0f;
    static public float RunSpeed = 6.0f;
    private float _walkSpeed = WalkSpeed;
    private float _runSpeed = RunSpeed;
    private Vector2 _moveInput;

    // Sprint Variables
    private bool _canSprint = true;
    private bool _isSprinting = false;
    [Header("Sprint Stamina")]
    [SerializeField] private float Stamina = 100.0f;
    private float _staminaDepletionFactor = 10.0f;
    private float _staminaRegenFactor = 5.0f;

    // Jump Variables
    [Header("Jump")]
    [SerializeField] private float JumpForce = 3.0f;
    private float _gravity = -9.81f;
    private float _groundedThreshold = 0.05f;

    // Crouch Variables
    private bool _isCrouching;
    [Header("Crouch")]
    [SerializeField] private float DefaultHeight = 2.0f;
    [SerializeField] private float CrouchHeight = 1.0f;
    [SerializeField] private float CrouchSpeed = 2.0f;
    private float DefaultCameraY;
    private float CrouchCameraY;
    private float _crouchOffset = 0.7f;

    // Item Hotbar Script
    private ItemHotbar _itemHotbar;

    // Player SFX Script
    private PlayerSound _playerSound;

    // Player Journal Script
    private PlayerJournal _playerJournal;

    // Getter Methods
    public bool CanSprint() { return _canSprint; }

    public ItemHotbar GetItemHotbar() { return _itemHotbar; }

    public PlayerJournal GetPlayerJournal() {  return _playerJournal; }

    // Setter Methods
    public void SetIsSprinting(bool InSprint) { _isSprinting = InSprint; }

    public void SetIsLeanLeft(bool InLeanLeft) { _isLeaningLeft = InLeanLeft; }

    public void SetIsLeanRight(bool InLeanRight) { _isLeaningRight = InLeanRight; }

    public void SetIsCrouching(bool InIsCrouching) { _isCrouching = InIsCrouching; }

    /// <summary>
    /// LEGACY: has been moved to UserInteraction. Remove when key obj becomes IInteractable
    /// </summary>
    public int IgnorePlayerMask;

    /// <summary>
    /// When a new level starts to load in, the player should be in the elevator or dead
    /// </summary>
    private EventBinding<LevelLoading> _levelLoading;

    private Vector3 _spawnPoint = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        if (PlayerCamera)
        {
            DefaultCameraY = PlayerCamera.transform.position.y;
            CrouchCameraY = PlayerCamera.transform.position.y - _crouchOffset;
        }

        // Get references to all necessary script components
        _playerInput = GetComponent<PlayerInputBindings>();
        _itemHotbar = GetComponent<ItemHotbar>();
        _playerSound = GetComponent<PlayerSound>();
        _playerJournal = GetComponent<PlayerJournal>();

        // The player should spawn wherever they start when the game initally loads - inside the elevator
        _spawnPoint = new Vector3(-27f, 1.2f, 0.0f);
        IgnorePlayerMask = ~LayerMask.GetMask("Player", "Ignore Raycast");
    }

    // Update is called once per frame
    private void Update()
    {
        // Check Crouch conditions
        CheckCrouch();

        // Lean Left/Right
        LeanLeftRight();
    }

    private void FixedUpdate()
    {
        // Check Sprint conditions;
        CheckSprint(Time.fixedDeltaTime);

        // Movement/Sprint
        Move(Time.fixedDeltaTime);

        // Orient Camera Rotation to Mouse Movement
        OrientCameraToRotation(Time.fixedDeltaTime);

        // Apply constant gravity force
        ApplyGravity(Time.fixedDeltaTime);
    }

    /// <summary>
    /// Checks if player can sprint
    /// </summary>
    public void Sprint()
    {
        if (_canSprint)
        {
            _isSprinting = true;
        }
    }

    /// <summary>
    /// Checks player current sprinting state
    /// If sprinting, deplete stamina bar
    /// If not sprinting, regen stamina bar
    /// </summary>
    /// <param name="dt">Delta Time</param>
    private void CheckSprint(float dt)
    {
        if (_isSprinting && !_playerJournal.GetInJournal())
        {
            _canLean = false;
            Stamina -= _staminaDepletionFactor * dt;

            _playerSound.SprintPantingDepletionSFX(Stamina);

            if (Stamina <= 0)
            {
                _canSprint = false;
                _isSprinting = false;
            }
        }
        else
        {
            _canLean = true;
            Stamina += _staminaRegenFactor * dt;

            _playerSound.SprintPantingRegenSFX(Stamina);

            if (Stamina > 10.0f)
            {
                _canSprint = true;
            }
        }

        // Clamp Stamina between [0, 100]
        Stamina = Mathf.Clamp(Stamina, 0, 100);
    }

    /// <summary>
    /// Moves player controller in any direction
    /// </summary>
    /// <param name="dt">Delta Time</param>
    private void Move(float dt)
    {
        if(!_playerJournal.GetInJournal())
        {
            // Rotate Camera on X and Y axes according to mouse movement to simulate orientation
            if (_canMove)
            {
                _moveInput = _playerInput.GetPlayerInputActions().Player.Move.ReadValue<Vector2>(); 
                Vector3 movement = _isSprinting ? transform.rotation * new Vector3(_moveInput.x, 0, _moveInput.y) * _runSpeed * dt
                                                : transform.rotation * new Vector3(_moveInput.x, 0, _moveInput.y) * _walkSpeed * dt;
                _rb.MovePosition(movement + _rb.position);
            }
        }
    }

    /// <summary>
    /// Orients camera to current player rotation
    /// </summary>
    /// <param name="dt">Delta Time</param>
    private void OrientCameraToRotation(float dt)
    {
        if (!_playerJournal.GetInJournal())
        {
            if (_canMove)
            {
                Vector2 lookValue = _playerInput.GetPlayerInputActions().Player.Look.ReadValue<Vector2>();
                Vector2 mouse = new Vector2(MouseSensitivityX * lookValue.x * dt, MouseSensitivityY * lookValue.y * dt);

                transform.Rotate(new Vector3(0, mouse.x * LookYawSpeed, 0));              // Yaw
               
                _rotationX -= mouse.y * LookPitchSpeed;
                _rotationX = Mathf.Clamp(_rotationX, -LookPitchLimit, LookPitchLimit);

                if(PlayerCamera != null)
                {
                    PlayerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);  // Pitch
                }
            }
        }
    }

    /// <summary>
    /// Checks whether player is currently crouching or not. Adjusts necessary movement speeds and camera height
    /// </summary>
    private void CheckCrouch()
    {
        if (!_playerJournal.GetInJournal())
        {
            if (_canMove)
            {
                Vector3 crouchOffset = new Vector3(0, -0.5f, 0);
                Vector3 standOffset = Vector3.zero;
                if (_isCrouching)
                {
                    _capsuleCollider.height = CrouchHeight;
                    _capsuleCollider.center = crouchOffset; // -0.5f to adjust the _capsuleCollider center to prevent floor clipping

                    if (PlayerCamera != null)
                    {
                        PlayerCamera.transform.localPosition = new Vector3(PlayerCamera.transform.localPosition.x, CrouchCameraY, PlayerCamera.transform.localPosition.z);
                    }

                    _walkSpeed = CrouchSpeed;
                    _runSpeed = CrouchSpeed;
                }
                else
                {
                    _capsuleCollider.height = DefaultHeight;
                    _capsuleCollider.center = standOffset; // reset the _capsuleCollider center

                    if (PlayerCamera != null)
                    {
                        PlayerCamera.transform.localPosition = new Vector3(PlayerCamera.transform.localPosition.x, DefaultCameraY, PlayerCamera.transform.localPosition.z);
                    }

                    _walkSpeed = WalkSpeed;
                    _runSpeed = RunSpeed;
                }
            }
        }
    }

    /// <summary>
    /// Applies a jump force to the player's up vector allowing them to jump
    /// </summary>
    public void Jump()
    {
        if (!_playerJournal.GetInJournal())
        {
            if (_canMove && IsGrounded())
            {
                _rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            } 
        }
    }

    /// <summary>
    /// Applies a gravitational force when player is airborne
    /// </summary>
    /// <param name="dt"></param>
    private void ApplyGravity(float dt)
    {
        if (!IsGrounded())
        {
            _rb.AddForce(Vector3.up * _gravity * dt, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Allows player to smoothly lean left and right
    /// </summary>
    private void LeanLeftRight()
    {
        if (!_playerJournal.GetInJournal())
        {
            if (_canMove && _canLean)
            {
                if(_isLeaningLeft)
                {
                    // Lean Left
                    _targetLean = LeanAngle;
                    _canSprint = false;
                }
                else if (_isLeaningRight)
                {
                    // Lean Right
                    _targetLean = -LeanAngle;
                    _canSprint = false;
                }
                else
                {
                    // Reset leaning
                    _targetLean = 0;

                    _isLeaningLeft = false;
                    _isLeaningRight = false;

                    if (Stamina > 10.0f)
                    {
                        _canSprint = true;
                    }
                }

                _currentLean = Mathf.SmoothDamp(_currentLean, _targetLean, ref _leanVelocity, LeanSmoothing);
                LeanPivot.localRotation = Quaternion.Euler(new Vector3(0, 0, _currentLean));
            }
        }
    }

    /// <summary>
    /// Allows player to use current item
    /// </summary>
    public void Use()
    {
        if (!_playerJournal.GetInJournal())
        {
            _itemHotbar.UseItem();
        }
    }

    

    /// <summary>
    /// Checks whether the player is grounded or airborne
    /// </summary>
    private bool IsGrounded()
    {
        return Mathf.Abs(_rb.linearVelocity.y) < _groundedThreshold;
    }

    /// <summary>
    /// When the player dies and the level restarts, reset everything about the player to the last iteration
    /// </summary>
    private void ResetPlayer(LevelLoading e)
    {
        // Remove items from inventory?
        // Fade in fade out black screen of death?
        // Disable journal?
        // What other edge cases when the level is reset..?

        // If this level is the last level, reset the player spawn
        if (e.newLevel == Level.currentLevel) { transform.position = _spawnPoint; }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void OnEnable()
    {
        _levelLoading = new EventBinding<LevelLoading>(ResetPlayer);
        EventBus<LevelLoading>.Register(_levelLoading);
    }

    private void OnDisable() 
    {
        EventBus<LevelLoading>.DeRegister(_levelLoading);
    }

    /// <summary>
    /// Allows the player to interact with interactable items
    /// </summary>
    public void Interact() 
    {
        if (_itemHotbar.SlotHasItem())
        {
            return;
        }

        // TODO: Test edges cases while pulling up the journal
        if (!_playerJournal.GetInJournal() && IInteractable.Target != null)
        {
            //IInteractable.isPressed = true;
            IInteractable.Instigator = this.gameObject; // Save a reference of player inside interacted object
        }
    }

    /// <summary>
    /// Allows player to drop current item
    /// </summary>
    public void DropItem()
    {
        if (!_playerJournal.GetInJournal())
        {
            _itemHotbar.DropItem();
        }
    }
}