using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Finds objects within range that can be interacted with and marks them as the Target
/// </summary>
public class UserInteraction : MonoBehaviour
{
    [SerializeField] private float _interactRange = 3.0f;

    private PlayerInputActions _playerInputActions;

    private IEnumerator _co;

    private GameObject _playerCam;

    private int _ignorePlayerMask;

    void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _ignorePlayerMask = ~LayerMask.GetMask("Player", "Ignore Raycast");
        _playerCam = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void OnEnable()
    {
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Interact.started += OnInteractStarted;
        _playerInputActions.Player.Interact.canceled += OnInteractCanceled;
    }
    private void OnDisable()
    {
        _playerInputActions.Player.Interact.started -= OnInteractStarted;
        _playerInputActions.Player.Interact.canceled -= OnInteractCanceled;
        _playerInputActions.Player.Disable();
    }

    public void Update()
    {
        // Ignores the player's collider when looking for interactions, allowing walls to occlude items
        if (Physics.Raycast(_playerCam.transform.position, _playerCam.transform.forward,
            out RaycastHit hit, _interactRange, _ignorePlayerMask))
        {
            // Looks for only IInteractable obj, still sending null to Target if looking at nothing
            hit.collider.TryGetComponent(out IInteractable obj);

            // Do not bother setting a new Target if it is the same focus as before
            if (IInteractable.Target != obj)
            {
                IInteractable.SetPriorityTarget(obj);
                OnInteractCanceled(new InputAction.CallbackContext());
            }
        }
    }

    private void OnInteractStarted(InputAction.CallbackContext context)
    {
        if (IInteractable.Target != null)
        {
            if (IInteractable.Target.HoldTime == 0.0f)
            {
                // TODO: check obj.CanInteract and play appropriate sfx if it CAN or CAN'T
                IInteractable.Target.Interact();
            }

            else
            {
                _co = PressAndHold(IInteractable.Target.HoldTime);
                StartCoroutine(_co);
            }
        }
    }

    /// <summary>
    /// Prevents an interactable from being used when crosshair moves away from Target or lets go of key
    /// </summary>
    /// <param name="context"></param>
    private void OnInteractCanceled(InputAction.CallbackContext context)
    {
        if (_co != null)
        {
            StopCoroutine(_co);
            _co = null;
        }
    }

    /// <summary>
    /// Called when an interactable obj needs to be held down to be used
    /// </summary>
    /// <param name="holdTime"></param>
    /// <returns></returns>
    private IEnumerator PressAndHold(float holdTime)
    {
        yield return new WaitForSeconds(holdTime);
        IInteractable.Target.Interact();
    }
}
