using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    // Mouse Sensitivity
    [Header("Mouse Sensitivity")]
    [SerializeField] public float MouseSensitivityX = 1.0f;
    [SerializeField] public float MouseSensitivityY = 1.0f;

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
    private float _currentLean;                             // Actual value of the lean
    private float _targetLean;                              // Will change as player pressed Q or E to lean
    [SerializeField] private float LeanAngle = 20;          // Set the _targetLean depending on LeanAngle       - I found a value of 20 to work best
    [SerializeField] private float LeanSmoothing = 0.3f;    // Used to smooth the _currentLean to _targetLean   - I found a value of 0.3 to work best
    private float _leanVelocity;
    private bool _isLeaningLeft;
    private bool _isLeaningRight;

    // Movement Variables
    private bool _canMove = true;
    [Header("Movement")]
    static public float WalkSpeed = 6.0f;
    static public float RunSpeed = 9.0f;
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
    private bool _wasSprinting = false;

    // Jump Variables
    [Header("Jump")]
    [SerializeField] private float JumpForce = 3.0f;

    // Crouch Variables
    private bool _isCrouching;
    [Header("Crouch")]
    [SerializeField] private float DefaultHeight = 2.0f;
    [SerializeField] private float CrouchHeight = 1.0f;
    [SerializeField] private float CrouchSpeed = 2.0f;
    private float DefaultCameraY;
    private float CrouchCameraY;
    private float _crouchOffset = 0.7f;

    // Player Controller Script
    private PlayerController _playerController;

    // Getter Methods
    public bool CanSprint() { return _canSprint; }

    public bool CanMove() { return _canMove; }

    public bool CanLean() { return _canLean; }

    public float GetWalkSpeed() { return _walkSpeed; }

    // Setter Methods
    public void SetIsSprinting(bool InSprint) { _isSprinting = InSprint; }

    public void SetIsLeanLeft(bool InLeanLeft) { _isLeaningLeft = InLeanLeft; }

    public void SetIsLeanRight(bool InLeanRight) { _isLeaningRight = InLeanRight; }

    public void SetIsCrouching(bool InIsCrouching) { _isCrouching = InIsCrouching; }

    public void SetWalkSpeed(float InWalkSpeed) { _walkSpeed = InWalkSpeed; }

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();

        if (_playerController.GetPlayerCamera())
        {
            DefaultCameraY = _playerController.GetPlayerCamera().transform.position.y;
            CrouchCameraY = _playerController.GetPlayerCamera().transform.position.y - _crouchOffset;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Check Crouch conditions
        CheckCrouch();

        // Lean Left/Right
        //LeanLeftRight();
    }

    private void FixedUpdate()
    {
        // Check Sprint conditions;
        CheckSprint(Time.fixedDeltaTime);

        // Movement/Sprint
        Move(Time.fixedDeltaTime);

        // Orient Camera Rotation to Mouse Movement
        OrientCameraToRotation(Time.fixedDeltaTime);
    }

    /// <summary>
    /// Moves player controller in any direction
    /// </summary>
    /// <param name="dt">Delta Time</param>
    private void Move(float dt)
    {
        if (!_playerController.GetPlayerJournal().GetInJournal())
        {
            // Rotate Camera on X and Y axes according to mouse movement to simulate orientation
            if (_canMove)
            {
                _moveInput = _playerController.GetPlayerInput().GetPlayerInputActions().Player.Move.ReadValue<Vector2>();
                Vector3 movement = _isSprinting ? transform.rotation * new Vector3(_moveInput.x, 0, _moveInput.y) * _runSpeed * dt
                                                : transform.rotation * new Vector3(_moveInput.x, 0, _moveInput.y) * _walkSpeed * dt;
                _playerController.GetRB().MovePosition(movement + _playerController.GetRB().position);
                if (movement == Vector3.zero || !_playerController.IsGrounded())
                {
                    SoundEffectTrigger.Instance.StopFootsteps();
                }
                else
                {
                    SoundEffectTrigger.Instance.PlayFootsteps(0.5f);
                }
            }
        }
        else
        {
            SoundEffectTrigger.Instance.StopFootsteps();
        }
    }

    /// <summary>
    /// Orients camera to current player rotation
    /// </summary>
    /// <param name="dt">Delta Time</param>
    private void OrientCameraToRotation(float dt)
    {
        if (!_playerController.GetPlayerJournal().GetInJournal())
        {
            if (_canMove)
            {
                Vector2 lookValue = _playerController.GetPlayerInput().GetPlayerInputActions().Player.Look.ReadValue<Vector2>();
                Vector2 mouse = new Vector2(MouseSensitivityX * lookValue.x * dt, MouseSensitivityY * lookValue.y * dt);

                transform.Rotate(new Vector3(0, mouse.x * LookYawSpeed, 0));              // Yaw

                _rotationX -= mouse.y * LookPitchSpeed;
                _rotationX = Mathf.Clamp(_rotationX, -LookPitchLimit, LookPitchLimit);

                if (_playerController.GetPlayerCamera() != null)
                {
                    _playerController.GetPlayerCamera().transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);  // Pitch
                }
            }
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
        if (_isSprinting && !_playerController.GetPlayerJournal().GetInJournal())
        {
            _canLean = false;
            Stamina -= _staminaDepletionFactor * dt;

            if (!_wasSprinting)
            {
                SoundEffectTrigger.Instance.StopFootsteps();
            }
            if (_playerController.IsGrounded()) 
            {
                SoundEffectTrigger.Instance.PlayFootsteps(0.35f);
            }
            _playerController.GetPlayerSound().SprintPantingDepletionSFX();

            if (Stamina <= 0)
            {
                _canSprint = false;
                _isSprinting = false;
                _playerController.GetPlayerSound().SprintPantingRegenSFX(Stamina);
            }
        }
        else
        {
            _canLean = true;
            Stamina += _staminaRegenFactor * dt;

            if (_wasSprinting && !_isSprinting)
            {
                _playerController.GetPlayerSound().SprintPantingRegenSFX(Stamina);
                SoundEffectTrigger.Instance.StopFootsteps();
                SoundEffectTrigger.Instance.PlayFootsteps(0.5f);
            }

            if (Stamina > 50.0f)
            {
                _canSprint = true;
            }
        }

        // Clamp Stamina between [0, 100]
        Stamina = Mathf.Clamp(Stamina, 0, 100);

        _wasSprinting = _isSprinting;
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
    /// Allows player to smoothly lean left and right
    /// </summary>
    public void LeanLeftRight()
    {
        if (!_playerController.GetPlayerJournal().GetInJournal())
        {
            if (_canMove && _canLean)
            {
                if (_isLeaningLeft)
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
    /// Checks whether player is currently crouching or not. Adjusts necessary movement speeds and camera height
    /// </summary>
    private void CheckCrouch()
    {
        if (!_playerController.GetPlayerJournal().GetInJournal())
        {
            if (_canMove)
            {
                Vector3 crouchOffset = new Vector3(0, -0.5f, 0);
                Vector3 standOffset = Vector3.zero;
                if (_isCrouching)
                {
                    _playerController.GetCapsuleCollider().height = CrouchHeight;
                    _playerController.GetCapsuleCollider().center = crouchOffset; // -0.5f to adjust the _capsuleCollider center to prevent floor clipping

                    if (_playerController.GetPlayerCamera() != null)
                    {
                        _playerController.GetPlayerCamera().transform.localPosition = new Vector3(_playerController.GetPlayerCamera().transform.localPosition.x, CrouchCameraY, _playerController.GetPlayerCamera().transform.localPosition.z);
                    }

                    _walkSpeed = CrouchSpeed;
                    _runSpeed = CrouchSpeed;
                }
                else
                {
                    _playerController.GetCapsuleCollider().height = DefaultHeight;
                    _playerController.GetCapsuleCollider().center = standOffset; // reset the _capsuleCollider center

                    if (_playerController.GetPlayerCamera() != null)
                    {
                        _playerController.GetPlayerCamera().transform.localPosition = new Vector3(_playerController.GetPlayerCamera().transform.localPosition.x, DefaultCameraY, _playerController.GetPlayerCamera().transform.localPosition.z);
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
        if (!_playerController.GetPlayerJournal().GetInJournal())
        {
            if (_canMove && _playerController.IsGrounded())
            {
                _playerController.GetRB().AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            }
        }
    }

    /// <summary>
    /// Allows player to use current item
    /// </summary>
    public void Use()
    {
        if (!_playerController.GetPlayerJournal().GetInJournal())
        {
            DoorController dc = RayCastDoor();

            if (dc != null)
            {
                if (dc.CanInteract)
                {
                    dc.ToggleDoor();
                }
            }
            else
            {
                _playerController.GetItemHotbar().UseItem();
            }
        }
    }

    /// <summary>
    /// Allows player to drop current item
    /// </summary>
    public void DropItem()
    {
        if (!_playerController.GetPlayerJournal().GetInJournal())
        {
            _playerController.GetItemHotbar().DropItem();
        }
    }

    /// <summary>
    /// Raycasts to door game object. Used to enable door 
    /// opening/closing with LMB click when door is unlocked
    /// </summary>
    /// <returns>returns the door controller script if raycast on door successful</returns>
    public DoorController RayCastDoor()
    {
        float interactRange = 10.0f;

        if(Physics.Raycast(_playerController.GetPlayerCamera().transform.position, 
            _playerController.GetPlayerCamera().transform.forward, 
            out RaycastHit hit, interactRange, _playerController.IgnorePlayerMask))
        {
            // on successful raycast
            return hit.collider.gameObject.GetComponent<DoorController>();
        }

        return null;
    }
}