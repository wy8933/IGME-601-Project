
using UnityEngine;
using UnityEngine.InputSystem;
using ItemSystem;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using System;
using AudioSystem;

//[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // Player Input Actions Class
    private PlayerInputActions _playerInputActions;
    // Player Rigidbody and CapsuleCollider
    private Rigidbody _rb;
    private CapsuleCollider _capsuleCollider;

    // Mouse Sensitivity
    [Header("Mouse Sensitivity")]
    [SerializeField] public float MouseSensitivityX = 1.0f;
    [SerializeField] public float MouseSensitivityY = 1.0f;

    // Follow Camera
    [Header("Follow Camera")]
    [SerializeField] Camera PlayerCamera;

    // Camera Yaw & Pitch
    [Header("Camera Pitch & Yaw")]
    [SerializeField] float LookYawSpeed = 2.0f;
    [SerializeField] float LookPitchSpeed = 1.0f;
    [SerializeField] float LookPitchLimit = 60.0f; 
    private float _rotationX = 0.0f;

    // Camera Lean Left/Right Variables
    private bool _canLean = true;
    [Header("Leaning")]
    [SerializeField] Transform LeanPivot;
    private float _currentLean;         // Actual value of the lean
    private float _targetLean;          // Will change as player pressed Q or E to lean
    [SerializeField] float LeanAngle;     // Set the _targetLean depending on LeanAngle       - I found a value of 20 to work best
    [SerializeField] float LeanSmoothing; // Used to smooth the _currentLean to _targetLean   - I found a value of 0.3 to work best
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
    [SerializeField] float Stamina = 100.0f;
    private float _staminaDepletionFactor = 10.0f;
    private float _staminaRegenFactor = 5.0f;

    // Jump Variables
    [Header("Jump")]
    [SerializeField] float JumpForce = 3.0f;
    private float _gravity = -9.81f;
    private float _groundedThreshold = 0.05f;

    // Crouch Variables
    private bool _isCrouching;
    [Header("Crouch")]
    [SerializeField] float DefaultHeight = 2.0f;
    [SerializeField] float CrouchHeight = 1.0f;
    [SerializeField] float CrouchSpeed = 2.0f;
    private float DefaultCameraY;
    private float CrouchCameraY;
    private float _crouchOffset = 0.7f;

    // Item Hotbar Variables
    private int _selectedItemIndex = 0;
    private GameObject[] _itemHotbar = new GameObject[4];

    [SerializeField] GameObject HotbarContainer;
    private CanvasGroup _canvasGroup;
    [SerializeField] GameObject Item1Icon;
    [SerializeField] GameObject Item2Icon;
    [SerializeField] GameObject Item3Icon;
    [SerializeField] GameObject Item4Icon;

    private float _fadeDuration = 1.0f;

    // Journal Variables
    private bool _inJournal = false;
    [Header("Journal")]
    [SerializeField] Journal_UI journal;

    [Header("Sound Data")]
    [SerializeField] SoundDataSO SprintSlowSO;
    [SerializeField] SoundDataSO SprintMedSO;
    [SerializeField] SoundDataSO SprintFastSO;
    private float _audioCooldownTime = 0.5f;
    private float lastPlayTime;

    // Layermasks
    private int _IgnorePlayerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        if (PlayerCamera)
        {
            DefaultCameraY = PlayerCamera.transform.position.y;
            CrouchCameraY = PlayerCamera.transform.position.y - _crouchOffset;
        }

        _canvasGroup = HotbarContainer.GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;

        // Initialize Playermasks
        _IgnorePlayerMask = ~LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    private void Update()
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

    public void PlaySound(SoundDataSO sd)
    {
        if (Time.time - lastPlayTime >= _audioCooldownTime)
        {
            if (AudioManager.Instance)
            {
                AudioManager.Instance.Play(sd);
            }
            lastPlayTime = Time.time;
        }
    }

    private void CheckSprint(float dt)
    {
        if (_isSprinting && !_inJournal)
        {
            _canLean = false;
            Stamina -= _staminaDepletionFactor * dt;

            SprintPantingDepletionSFX();

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

            SprintPantingRegenSFX();

            if (Stamina > 10.0f)
            {
                _canSprint = true;
            }
        }

        // Clamp Stamina between [0, 100]
        Stamina = Mathf.Clamp(Stamina, 0, 100);

        // Debug Logs
        Debug.Log("Stamina: " + Stamina);
        Debug.Log("isSprinting: " + _isSprinting);
    }

    private void SprintPantingDepletionSFX()
    {
        if (Stamina > 66.0f)
        {
            PlaySound(SprintSlowSO);
        }
        else if (Stamina > 33.0f)
        {
            AudioManager.Instance.Stop(gameObject, SprintSlowSO);
            PlaySound(SprintMedSO);
        }
        else if (Stamina > 0)
        {
            AudioManager.Instance.Stop(gameObject, SprintMedSO);
            PlaySound(SprintFastSO);
        }
    }

    private void SprintPantingRegenSFX()
    {
        if (Stamina <= 33.0f)
        {
            PlaySound(SprintFastSO);
        }
        else if (Stamina <= 66.0f)
        {
            AudioManager.Instance.Stop(gameObject, SprintFastSO);
            PlaySound(SprintMedSO);
        }
        else if (Stamina < 100)
        {
            AudioManager.Instance.Stop(gameObject, SprintMedSO);
            PlaySound(SprintSlowSO);
        }
        else
        {
            AudioManager.Instance.Stop(gameObject, SprintSlowSO);
        }
    }

    private void Move(float dt)
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

    private void OrientCameraToRotation(float dt)
    {
        if (!_inJournal)
        {
            if (_canMove)
            {
                Vector2 lookValue = _playerInputActions.Player.Look.ReadValue<Vector2>();
                Vector2 mouse = new Vector2(MouseSensitivityX * lookValue.x * dt, MouseSensitivityY * lookValue.y * dt);

                //transform.rotation *= Quaternion.Euler(0, mouse.x * LookYawSpeed, 0);   // Yaw
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

    private void CheckCrouch()
    {
        if (!_inJournal)
        {
            if (_canMove)
            {
                if (_isCrouching)
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

    private void Jump()
    {
        if (!_inJournal)
        {
            if (_canMove && IsGrounded())
            {
                _rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            } 
        }
    }

    private void ApplyGravity(float dt)
    {
        if (!IsGrounded())
        {
            _rb.AddForce(Vector3.up * _gravity * dt, ForceMode.Impulse);
        }
    }

    private void LeanLeftRight()
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

    public void AddItem(GameObject item)
    {
        if (_itemHotbar[_selectedItemIndex] != null)
        {
            return;
        }

        _itemHotbar[_selectedItemIndex] = item;
        item.GetComponent<ItemInstance>().AttachToParent(this.gameObject);
        Debug.Log("Item added to hotbar! " + _itemHotbar[_selectedItemIndex].ToString());

        UpdateHotbarItemIcon();

        StartCoroutine(FadeSequence());
    }

    public void Use()
    {
        if (!_inJournal)
        {
            Debug.Log(_selectedItemIndex);

            if (_itemHotbar[_selectedItemIndex] != null)
            {
                _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().Use(this.gameObject);
            }
            else
            {
                Debug.Log("No Item Selected!");
            }
        }
    }

    private void DropItem()
    {
        if (!_inJournal)
        {
            if (_itemHotbar[_selectedItemIndex] != null)
            {
                Debug.Log("Drop " + _itemHotbar[_selectedItemIndex]);
                _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().DetachFromParent(this.gameObject);
                _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().EnableRigidBodyCollisions();
                _itemHotbar[_selectedItemIndex] = null;

                RemoveHotbarItemIcon();

                StartCoroutine(FadeSequence());
            }
        }
    }

    public void ToggleJournal()
    {
        _inJournal = !_inJournal;
        journal.gameObject.SetActive(_inJournal);
        if (_inJournal)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
    }

    private bool IsGrounded()
    {
        return Mathf.Abs(_rb.linearVelocity.y) < _groundedThreshold;
    }

    private void UpdateHotbarItemIcon()
    {
        switch (_selectedItemIndex)
        {
            case 1:
                Item2Icon.GetComponent<RawImage>().texture = _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().item.itemIcon.texture;
                Item2Icon.GetComponent<RawImage>().color = Color.yellow;
                break;
            case 2:
                Item3Icon.GetComponent<RawImage>().texture = _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().item.itemIcon.texture;
                Item3Icon.GetComponent<RawImage>().color = Color.yellow;
                break;
            case 3:
                Item4Icon.GetComponent<RawImage>().texture = _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().item.itemIcon.texture;
                Item4Icon.GetComponent<RawImage>().color = Color.yellow;
                break;
            default:
                Item1Icon.GetComponent<RawImage>().texture = _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().item.itemIcon.texture;
                Item1Icon.GetComponent<RawImage>().color = Color.yellow;
                break;
        }
    }

    private void RemoveHotbarItemIcon()
    {
        Color resetColor = new Color(0, 0, 0, 0.5f);

        switch (_selectedItemIndex)
        {
            case 1:
                Item2Icon.GetComponent<RawImage>().texture = null;
                Item2Icon.GetComponent<RawImage>().color = resetColor;
                break;
            case 2:
                Item3Icon.GetComponent<RawImage>().texture = null;
                Item3Icon.GetComponent<RawImage>().color = resetColor;
                break;
            case 3:
                Item4Icon.GetComponent<RawImage>().texture = null;
                Item4Icon.GetComponent<RawImage>().color = resetColor;
                break;
            default:
                Item1Icon.GetComponent<RawImage>().texture = null;
                Item1Icon.GetComponent<RawImage>().color = resetColor;
                break;
        }
    }

    public IEnumerator FadeSequence()
    {
        // Fade In
        yield return StartCoroutine(DoFade(_canvasGroup.alpha, 1));

        // Stay Visible
        yield return new WaitForSeconds(1);

        // Fade Out
        yield return StartCoroutine(DoFade(_canvasGroup.alpha, 0));
    }

    private IEnumerator DoFade(float startAlpha, float endAlpha)
    {
        float timer = 0;

        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / _fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = endAlpha;
    }

    private void ToggleHotbarDisplay()
    {
        HotbarContainer.SetActive(!HotbarContainer.activeSelf);
    }


    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void OnEnable()
    {
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Jump.performed += OnJumpPerformed;
        _playerInputActions.Player.Interact.started += OnInteractStarted;
        _playerInputActions.Player.Drop.performed += OnDropPerformed;
        _playerInputActions.Player.Use.started += OnUseStarted;
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

    private void OnDisable() 
    {
        _playerInputActions.Player.Jump.performed -= OnJumpPerformed;
        _playerInputActions.Player.Interact.started -= OnInteractStarted;
        _playerInputActions.Player.Use.started -= OnUseStarted;
        _playerInputActions.Player.Item1Hotbar.performed -= OnItem1HotbarPerformed;
        _playerInputActions.Player.Item2Hotbar.performed -= OnItem2HotbarPerformed;
        _playerInputActions.Player.Item3Hotbar.performed -= OnItem3HotbarPerformed;
        _playerInputActions.Player.Item4Hotbar.performed -= OnItem4HotbarPerformed;
        _playerInputActions.Player.OpenJournal.performed -= OnOpenJournalPerformed;
        _playerInputActions.Player.Disable();
    }

    private void OnSprintPerformed(InputAction.CallbackContext ctx)
    {
        if (_canSprint)
        {
            _isSprinting = true;
        }
    }

    private void OnSprintCanceled(InputAction.CallbackContext ctx)
    {
        _isSprinting = false;
    }

    private void OnLeanLeftPerformed(InputAction.CallbackContext ctx)
    {
        _isLeaningLeft = true;
        _isLeaningRight = false;
    }

    private void OnLeanLeftCanceled(InputAction.CallbackContext ctx)
    {
        _isLeaningLeft = false;
    }

    private void OnLeanRightPerformed(InputAction.CallbackContext ctx)
    {
        _isLeaningRight = true;
        _isLeaningLeft = false;
    }

    private void OnLeanRightCanceled(InputAction.CallbackContext ctx)
    {
        _isLeaningRight = false;
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        Jump();
    }

    private void OnInteractStarted(InputAction.CallbackContext ctx)
    {
        if (_itemHotbar[_selectedItemIndex] != null)
        {
            Debug.Log("Already holding an item");
            return;
        }

        // TODO: Test edges cases while pulling up the journal
        if (!_inJournal && Interaction.Target != null)
        {
            Interaction.isPressed = true;
            Interaction.Instigator = this.gameObject; // Save a reference of player inside interacted object
        }
    }

    private void OnDropPerformed(InputAction.CallbackContext ctx)
    {
        DropItem();
    }

    private void OnUseStarted(InputAction.CallbackContext ctx)
    {
        Use();
    }

    private void OnItem1HotbarPerformed(InputAction.CallbackContext ctx)
    {
        if (_itemHotbar[_selectedItemIndex])
        {
            _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().UnEquip();
        }
        else
        {
            ResetPreviousEmptySlot();
        }

        _selectedItemIndex = 0;

        if (_itemHotbar[_selectedItemIndex])
        {
            _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().Equip();
        }
        else
        {
            Item1Icon.GetComponent<RawImage>().color = Color.red;
        }

        StartCoroutine(FadeSequence());
    }

    private void OnItem2HotbarPerformed(InputAction.CallbackContext ctx)
    {
        if (_itemHotbar[_selectedItemIndex])
        {
            _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().UnEquip();
        }
        else
        {
            ResetPreviousEmptySlot();
        }

        _selectedItemIndex = 1;

        if (_itemHotbar[_selectedItemIndex])
        {
            _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().Equip();
        }
        else
        {
            Item2Icon.GetComponent<RawImage>().color = Color.red;
        }

        StartCoroutine(FadeSequence());
    }
    private void OnItem3HotbarPerformed(InputAction.CallbackContext ctx)
    {
        if (_itemHotbar[_selectedItemIndex])
        {
            _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().UnEquip();
        }
        else
        {
            ResetPreviousEmptySlot();
        }

        _selectedItemIndex = 2;

        if (_itemHotbar[_selectedItemIndex])
        {
            _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().Equip();
        }
        else
        {
            Item3Icon.GetComponent<RawImage>().color = Color.red;
        }

        StartCoroutine(FadeSequence());
    }

    private void OnItem4HotbarPerformed(InputAction.CallbackContext ctx)
    {
        if (_itemHotbar[_selectedItemIndex])
        {
            _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().UnEquip();
        }
        else
        {
            ResetPreviousEmptySlot();
        }

        _selectedItemIndex = 3;

        if (_itemHotbar[_selectedItemIndex])
        {
            _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().Equip();
        }
        else
        {
            Item4Icon.GetComponent<RawImage>().color = Color.red;
        }

        StartCoroutine(FadeSequence());
    }

    private void ResetPreviousEmptySlot()
    {
        if (_itemHotbar[_selectedItemIndex] == null)
        {
            switch (_selectedItemIndex)
            {
                case 1:
                    Item2Icon.GetComponent<RawImage>().color = new Color(0, 0, 0, 0.5f);
                    break;
                case 2:
                    Item3Icon.GetComponent<RawImage>().color = new Color(0, 0, 0, 0.5f);
                    break;
                case 3:
                    Item4Icon.GetComponent<RawImage>().color = new Color(0, 0, 0, 0.5f);
                    break;
                default:
                    Item1Icon.GetComponent<RawImage>().color = new Color(0, 0, 0, 0.5f);
                    break;
            }
        }
    }


    private void OnOpenJournalPerformed(InputAction.CallbackContext ctx)
    {
        ToggleJournal();
    }

    private void OnCrouchPerformed(InputAction.CallbackContext ctx)
    {
        _isCrouching = true;
    }

    private void OnCrouchCanceled(InputAction.CallbackContext ctx)
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
            hit.collider.TryGetComponent(out Interaction obj) &&
            obj.canInteract)
        {
            Interaction.SetPriorityTarget(obj);
            //Interaction.Target.Highlight();
        }
        else
        {
            Interaction.SetPriorityTarget(null);
        }
    }
}
