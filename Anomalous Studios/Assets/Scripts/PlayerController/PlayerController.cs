using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // Player Input Actions Class
    PlayerInputActions _playerInputActions;
    // Player Rigidbody and CapsuleCollider
    Rigidbody _rb;
    CapsuleCollider _capsuleCollider;

    // Follow Camera
    public Camera PlayerCamera;

    // Mouse Sensitivity
    public float MouseSensitivityX = 1.0f;
    public float MouseSensitivityY = 1.0f;

    // Camera Yaw & Pitch
    public float LookYawSpeed = 2.0f;
    public float LookPitchSpeed = 1.0f;
    public float LookPitchLimit = 60.0f; 
    float _rotationX = 0.0f;

    // Camera Lean Left/Right Variables
    bool _canLean = true;
    public Transform LeanPivot;
    float _currentLean;         // Actual value of the lean
    float _targetLean;          // Will change as player pressed Q or E to lean
    public float LeanAngle;     // Set the _targetLean depending on LeanAngle       - I found a value of 20 to work best
    public float LeanSmoothing; // Used to smooth the _currentLean to _targetLean   - I found a value of 0.3 to work best
    float _leanVelocity;
    bool _isLeaningLeft;
    bool _isLeaningRight;

    // Movement Variables
    bool _canMove = true;
    static public float WalkSpeed = 3.0f;
    static public float RunSpeed = 6.0f;
    float _walkSpeed = WalkSpeed;
    float _runSpeed = RunSpeed;
    Vector2 _moveInput;

    // Sprint Variables
    bool _canSprint = true;
    bool _isSprinting = false;
    public float Stamina = 100.0f;
    float _staminaDepletionFactor = 20.0f;
    float _staminaRegenFactor = 5.0f;
    
    // Jump Variables
    public float JumpForce = 3.0f;
    float _gravity = -9.81f;
    float _groundedThreshold = 0.05f;

    // Crouch Variables
    bool _isCrouching;
    public float DefaultHeight = 2.0f;
    public float CrouchHeight = 1.0f;
    public float CrouchSpeed = 2.0f;
    float DefaultCameraY;
    float CrouchCameraY;
    float _crouchOffset = 0.7f;

    // Item Hotbar Variables
    public int SelectedItemIndex = -1;
    string[] _itemHotbar = new string[4];

    // Journal Variables
    bool _inJournal = false;

    // Layermasks
    private int _IgnorePlayerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Populate hotbar with temporary placeholder items
        _itemHotbar[0] = "FlashLight";
        _itemHotbar[1] = "Rusty Key";
        _itemHotbar[2] = "Batteries";
        _itemHotbar[3] = "Taco Cat";

        if (PlayerCamera)
        {
            DefaultCameraY = PlayerCamera.transform.position.y;
            CrouchCameraY = PlayerCamera.transform.position.y - _crouchOffset;
        }

        // Initialize Playermasks
        _IgnorePlayerMask = ~LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Check Crouch conditions
        CheckCrouch();

        // Lean Left/Right
        LeanLeftRight();

        ScanInteractables(5.0f);
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

    void CheckSprint(float dt)
    {
        if (_isSprinting && !_inJournal)
        {
            _canLean = false;
            Stamina -= _staminaDepletionFactor * dt;

            if (Stamina < 0)
            {
                _canSprint = false;
                _isSprinting = false;
            }
        }
        else
        {
            _canLean = true;
            Stamina += _staminaRegenFactor * dt;

            if (Stamina > 10.0f)
            {
                _canSprint = true;
            }
        }

        // Clamp Stamina between [0, 100]
        Stamina = Mathf.Clamp(Stamina, 0, 100);

        // Debug Logs
        //Debug.Log("Stamina: " + Stamina);
        //Debug.Log("isSprinting: " + _isSprinting);
    }

    void Move(float dt)
    {
        if(!_inJournal)
        {
            // Rotate Camera on X and Y axes according to mouse movement to simulate orientation
            if (_canMove)
            {
                _moveInput = _playerInputActions.Player.Move.ReadValue<Vector2>();
                Vector3 movement = _isSprinting ? transform.rotation * new Vector3(_moveInput.x, 0, _moveInput.y) * _runSpeed * dt
                                                : transform.rotation * new Vector3(_moveInput.x, 0, _moveInput.y) * _walkSpeed * dt;
                _rb.MovePosition(movement + _rb.position);
            }
        }
    }

    void OrientCameraToRotation(float dt)
    {
        if (!_inJournal)
        {
            if (_canMove)
            {
                Vector2 lookValue = _playerInputActions.Player.Look.ReadValue<Vector2>();
                Vector2 mouse = new Vector2(MouseSensitivityX * lookValue.x * dt, MouseSensitivityY * lookValue.y * dt);

                transform.rotation *= Quaternion.Euler(0, mouse.x * LookYawSpeed, 0);   // Yaw
               
                _rotationX -= mouse.y;
                _rotationX = Mathf.Clamp(_rotationX, -LookPitchLimit, LookPitchLimit);

                if(PlayerCamera != null)
                {
                    PlayerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);  // Pitch
                }
            }
        }
    }

    void CheckCrouch()
    {
        if (!_inJournal)
        {
            if (_canMove)
            {
                if(_isCrouching)
                {
                    _capsuleCollider.height = CrouchHeight;
                    _capsuleCollider.center = new Vector3(0, -0.5f, 0); // -0.5f to adjust the _capsuleCollider center to prevent floor clipping

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
                    _capsuleCollider.center = new Vector3(0, 0, 0); // reset the _capsuleCollider center

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

    void Jump()
    {
        if (!_inJournal)
        {
            if (_canMove && IsGrounded())
            {
                _rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            } 
        }
    }

    void ApplyGravity(float dt)
    {
        if (!IsGrounded())
        {
            _rb.AddForce(Vector3.up * _gravity * dt, ForceMode.Impulse);
        }
    }

    void LeanLeftRight()
    {
        if (!_inJournal)
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

    void Interact()
    {
        if (!_inJournal && Interaction.Target != null)
        {
            Interaction.Target.Interact();
        }
    }

    public void Use()
    {
        if (!_inJournal)
        {
            if (SelectedItemIndex != -1 && _itemHotbar[SelectedItemIndex] != null)
            {
                Debug.Log("Use Currently Selected {" + SelectedItemIndex + "} item: " + _itemHotbar[SelectedItemIndex]);
                _itemHotbar[SelectedItemIndex] = null;
            }
            else
            {
                Debug.Log("No Item Selected!");
            }
        }
    }

    void ToggleJournal()
    {
        _inJournal = !_inJournal;

        if (_inJournal)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    bool IsGrounded()
    {
        return Mathf.Abs(_rb.linearVelocity.y) < _groundedThreshold;
    }

    void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    void OnEnable()
    {
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Jump.performed += OnJumpPerformed;
        _playerInputActions.Player.Interact.performed += OnInteractPerformed;
        _playerInputActions.Player.Use.performed += OnUsePerformed;
        _playerInputActions.Player.Item1Hotbar.performed += OnItem1HotbarPerformed;
        _playerInputActions.Player.Item2Hotbar.performed += OnItem2HotbarPerformed;
        _playerInputActions.Player.Item3Hotbar.performed += OnItem3HotbarPerformed;
        _playerInputActions.Player.Item4Hotbar.performed += OnItem4HotbarPerformed;
        _playerInputActions.Player.OpenJournal.performed += OnOpenJournalPerformed;

        _playerInputActions.Player.Sprint.performed += OnSprintPerformed;
        _playerInputActions.Player.Sprint.canceled += OnSprintCanceled;
        _playerInputActions.Player.Crouch.performed += OnCrouchPerformed;
        _playerInputActions.Player.Crouch.canceled += OnCrouchCanceled;

        _playerInputActions.Player.LeanLeft.performed += OnLeanLeftPerformed;
        _playerInputActions.Player.LeanLeft.canceled += OnLeanLeftCanceled;
        _playerInputActions.Player.LeanRight.performed += OnLeanRightPerformed;
        _playerInputActions.Player.LeanRight.canceled += OnLeanRightCanceled;
    }

    void OnDisable() 
    {
        _playerInputActions.Player.Jump.performed -= OnJumpPerformed;
        _playerInputActions.Player.Interact.performed -= OnInteractPerformed;
        _playerInputActions.Player.Use.performed -= OnUsePerformed;
        _playerInputActions.Player.Item1Hotbar.performed -= OnItem1HotbarPerformed;
        _playerInputActions.Player.Item2Hotbar.performed -= OnItem2HotbarPerformed;
        _playerInputActions.Player.Item3Hotbar.performed -= OnItem3HotbarPerformed;
        _playerInputActions.Player.Item4Hotbar.performed -= OnItem4HotbarPerformed;
        _playerInputActions.Player.OpenJournal.performed -= OnOpenJournalPerformed;
        _playerInputActions.Player.Disable();
    }

    void OnSprintPerformed(InputAction.CallbackContext ctx)
    {
        if (_canSprint)
        {
            _isSprinting = true;
        }
    }

    void OnSprintCanceled(InputAction.CallbackContext ctx)
    {
        _isSprinting = false;
    }

    void OnLeanLeftPerformed(InputAction.CallbackContext ctx)
    {
        _isLeaningLeft = true;
        _isLeaningRight = false;
    }

    void OnLeanLeftCanceled(InputAction.CallbackContext ctx)
    {
        _isLeaningLeft = false;
    }

    void OnLeanRightPerformed(InputAction.CallbackContext ctx)
    {
        _isLeaningRight = true;
        _isLeaningLeft = false;
    }

    void OnLeanRightCanceled(InputAction.CallbackContext ctx)
    {
        _isLeaningRight = false;
    }

    void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        Jump();
    }

    void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        Interact();
    }

    void OnUsePerformed(InputAction.CallbackContext ctx)
    {
        Use();
    }

    void OnItem1HotbarPerformed(InputAction.CallbackContext ctx)
    {
        SelectedItemIndex = 0;
    }

    void OnItem2HotbarPerformed(InputAction.CallbackContext ctx)
    {
        SelectedItemIndex = 1;
    }
    void OnItem3HotbarPerformed(InputAction.CallbackContext ctx)
    {
        SelectedItemIndex = 2;
    }

    void OnItem4HotbarPerformed(InputAction.CallbackContext ctx)
    {
        SelectedItemIndex = 3;
    }

    void OnOpenJournalPerformed(InputAction.CallbackContext ctx)
    {
        ToggleJournal();
    }

    void OnCrouchPerformed(InputAction.CallbackContext ctx)
    {
        _isCrouching = true;
    }

    void OnCrouchCanceled(InputAction.CallbackContext ctx)
    {
        _isCrouching = false;
    }

    /// <summary>
    /// Finds objects within range that can be interacted with and highlights the item
    /// </summary>
    private void ScanInteractables(float interactRange)
    {

        // Ignores the player's collider when looking for interactions, allowing walls to occlude items
        // 1) Looks for any object  2) makes sure its an interactable  3) and that it is usable
        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward,
            out RaycastHit hit, interactRange, _IgnorePlayerMask) &&
            hit.collider.TryGetComponent<Interaction>(out Interaction obj) &&
            obj._canInteract)
        {
            Interaction.SetPriorityTarget(obj);
            Interaction.Target.Highlight();
        }
        else
        {
            Interaction.SetPriorityTarget(null);
        }

        // If target != null and If button is pressed
        // increment the timer

        // If timer less than zero
        // call Interact()
        // else
        // reset timer
    }
}
