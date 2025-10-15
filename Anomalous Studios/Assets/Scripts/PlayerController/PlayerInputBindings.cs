using ItemSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInputBindings : MonoBehaviour
{
    // Player Input Actions Script
    private PlayerInputActions _playerInputActions;
    // Player Controller Script
    private PlayerController _playerController;
    // Scroll Wheel Tracker
    private float _scrollAmount;
    // Getter Methods
    public PlayerInputActions GetPlayerInputActions()
    {
        return _playerInputActions;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
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
        _playerInputActions.Player.ScrollUp.performed += OnScrollUpPerformed;

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
        _playerInputActions.Player.ScrollUp.performed -= OnScrollUpPerformed;
        _playerInputActions.Player.Disable();
    }

    private void OnSprintPerformed(InputAction.CallbackContext ctx)
    {
        _playerController.GetPlayerActions().Sprint();
    }

    private void OnSprintCanceled(InputAction.CallbackContext ctx)
    {
        _playerController.GetPlayerActions().SetIsSprinting(false);
    }

    private void OnLeanLeftPerformed(InputAction.CallbackContext ctx)
    {
        _playerController.GetPlayerActions().SetIsLeanLeft(true);
        _playerController.GetPlayerActions().SetIsLeanRight(false);
    }

    private void OnLeanLeftCanceled(InputAction.CallbackContext ctx)
    {
        _playerController.GetPlayerActions().SetIsLeanLeft(false);
    }

    private void OnLeanRightPerformed(InputAction.CallbackContext ctx)
    {
        _playerController.GetPlayerActions().SetIsLeanRight(true);
        _playerController.GetPlayerActions().SetIsLeanLeft(false);
    }

    private void OnLeanRightCanceled(InputAction.CallbackContext ctx)
    {
        _playerController.GetPlayerActions().SetIsLeanRight(false);
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        _playerController.GetPlayerActions().Jump();
    }

    private void OnCrouchPerformed(InputAction.CallbackContext ctx)
    {
        _playerController.GetPlayerActions().SetIsCrouching(true);
    }

    private void OnCrouchCanceled(InputAction.CallbackContext ctx)
    {
        _playerController.GetPlayerActions().SetIsCrouching(false);
    }

    private void OnInteractStarted(InputAction.CallbackContext ctx)
    {
        _playerController.Interact();
    }

    private void OnDropPerformed(InputAction.CallbackContext ctx)
    {
        _playerController.GetPlayerActions().DropItem();
    }

    private void OnUseStarted(InputAction.CallbackContext ctx)
    {
        _playerController.GetPlayerActions().Use();
    }

    private void OnItem1HotbarPerformed(InputAction.CallbackContext ctx)
    {
        _playerController.GetItemHotbar().SwitchToItem(0);
    }

    private void OnItem2HotbarPerformed(InputAction.CallbackContext ctx)
    {
        _playerController.GetItemHotbar().SwitchToItem(1);
    }
    private void OnItem3HotbarPerformed(InputAction.CallbackContext ctx)
    {
        _playerController.GetItemHotbar().SwitchToItem(2);
    }

    private void OnItem4HotbarPerformed(InputAction.CallbackContext ctx)
    {
        _playerController.GetItemHotbar().SwitchToItem(3);
    }

    private void OnOpenHandbookPerformed(InputAction.CallbackContext ctx)
    {
        _playerController.GetPlayerJournal().ToggleHandbook();
    }

    private void OnScrollUpPerformed(InputAction.CallbackContext ctx)
    {
        Vector2 scrollPos = ctx.ReadValue<Vector2>();
        _scrollAmount += scrollPos.y;

        if(_scrollAmount >= 1.5)
        {
            _playerController.GetItemHotbar().ScrollUp();
            _scrollAmount = 0;
        }
        else if (_scrollAmount <= -1.5)
        {
            _playerController.GetItemHotbar().ScrollDown();
            _scrollAmount = 0;
        }
        
    }
}
