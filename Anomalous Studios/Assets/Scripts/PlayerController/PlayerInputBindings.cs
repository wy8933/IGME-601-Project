using ItemSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInputBindings : MonoBehaviour
{
    // Player Input Actions Class
    private PlayerInputActions _playerInputActions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
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
        _playerInputActions.Player.OpenHandbook.performed += OnOpenHandbookPerformed;

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
        _playerInputActions.Player.OpenHandbook.performed -= OnOpenHandbookPerformed;
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

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(FadeSequence());
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

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(FadeSequence());
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

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(FadeSequence());
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

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(FadeSequence());
    }

    private void OnOpenHandbookPerformed(InputAction.CallbackContext ctx)
    {
        ToggleHandbook();
    }

    private void OnCrouchPerformed(InputAction.CallbackContext ctx)
    {
        _isCrouching = true;
    }

    private void OnCrouchCanceled(InputAction.CallbackContext ctx)
    {
        _isCrouching = false;
    }
}
