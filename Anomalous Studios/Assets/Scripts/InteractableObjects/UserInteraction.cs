using AudioSystem;
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

    private Transform _playerCam;

    private int _ignorePlayerMask;

    public void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _ignorePlayerMask = ~LayerMask.GetMask("Player", "Ignore Raycast");
        _playerCam = transform.Find("LeanPivot/Main Camera");
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
        if (Physics.Raycast(_playerCam.position, _playerCam.forward,
            out RaycastHit hit, _interactRange, _ignorePlayerMask))
        {
            // Looks for only IInteractable obj, still sending null to Target if looking at nothing
            hit.collider.TryGetComponent(out IInteractable obj);

            // Do not bother setting a new Target if it is the same focus as before
            if (IInteractable.Target != obj)
            {
                OnInteractCanceled(new InputAction.CallbackContext());
                IInteractable.SetPriorityTarget(obj);
            }
        }
        // If the target is already null, don't bother
        else if (IInteractable.Target != null)
        {
            OnInteractCanceled(new InputAction.CallbackContext());
            IInteractable.SetPriorityTarget(null);
        }
    }

    private void OnInteractStarted(InputAction.CallbackContext context)
    {
        if (IInteractable.Target != null)
        {
            if (!IInteractable.Target.CanInteract)
            {
                AudioManager.Instance.Play(IInteractable.Target.FailedSFX, transform.position);
            }

            else if (IInteractable.Target.HoldTime == 0.0f)
            {
                AudioManager.Instance.Play(IInteractable.Target.SuccessSFX, transform.position);
                IInteractable.Target.Interact();
            }

            else
            {
                AudioManager.Instance.Play(IInteractable.Target.InitialSFX, transform.position);
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
            AudioManager.Instance.Play(IInteractable.Target.CancelSFX, transform.position);
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
        AudioManager.Instance.Play(IInteractable.Target.SuccessSFX, transform.position);
        IInteractable.Target.Interact();
    }
}
