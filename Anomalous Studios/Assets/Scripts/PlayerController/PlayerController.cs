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

    // Player Input Actions
    private PlayerActions _playerActions;

    // Player Rigidbody and CapsuleCollider
    private Rigidbody _rb;
    private CapsuleCollider _capsuleCollider;

    // Follow Camera
    [Header("Follow Camera")]
    [SerializeField] private Camera PlayerCamera;

    [Header("Item Container")]
    public Transform _itemContainerTransform;

    // Item Hotbar Script
    private ItemHotbar _itemHotbar;

    // Player SFX Script
    private PlayerSound _playerSound;

    // Player Journal Script
    private PlayerJournal _playerJournal;

    private float _gravity = -9.81f;
    private float _groundedThreshold = 0.05f;

    /// <summary>
    /// LEGACY: has been moved to UserInteraction. Remove when key obj becomes IInteractable
    /// </summary>
    public int IgnorePlayerMask;

    /// <summary>
    /// When a new level starts to load in, the player should be in the elevator or dead
    /// </summary>
    private EventBinding<LoadLevel> _levelLoading;

    private Vector3 _spawnPoint = Vector3.zero;

    // Getter Methods
    public ItemHotbar GetItemHotbar() { return _itemHotbar; }
    public PlayerJournal GetPlayerJournal() {  return _playerJournal; }
    public PlayerSound GetPlayerSound() { return _playerSound; }
    public PlayerInputBindings GetPlayerInput() { return _playerInput; }
    public PlayerActions GetPlayerActions() { return _playerActions; }
    public Rigidbody GetRB() { return _rb; }
    public CapsuleCollider GetCapsuleCollider() { return _capsuleCollider; }
    public Camera GetPlayerCamera() { return PlayerCamera; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        // Hide mouse cursor
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        // Get references to all necessary script components
        _playerInput = GetComponent<PlayerInputBindings>();
        _itemHotbar = GetComponent<ItemHotbar>();
        _playerSound = GetComponent<PlayerSound>();
        _playerJournal = GetComponent<PlayerJournal>();
        _playerActions = GetComponent<PlayerActions>();

        // The player should spawn wherever they start when the game initally loads - inside the elevator
        _spawnPoint = new Vector3(-27f, 1.2f, 0.0f);
        IgnorePlayerMask = ~LayerMask.GetMask("Player", "Ignore Raycast");
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void OnEnable()
    {
        _levelLoading = new EventBinding<LoadLevel>(ResetPlayer);
        EventBus<LoadLevel>.Register(_levelLoading);
    }

    private void OnDisable()
    {
        EventBus<LoadLevel>.DeRegister(_levelLoading);
    }

    private void FixedUpdate()
    {
        // Apply constant gravity force
        ApplyGravity(Time.fixedDeltaTime);
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
    /// Checks whether the player is grounded or airborne
    /// </summary>
    public bool IsGrounded()
    {
        return Mathf.Abs(_rb.linearVelocity.y) < _groundedThreshold;
    }

    /// <summary>
    /// When the player dies and the level restarts, reset everything about the player to the last iteration
    /// </summary>
    private void ResetPlayer(LoadLevel e)
    {
        // Remove items from inventory?
        // Fade in fade out black screen of death?
        // Disable journal?
        // What other edge cases when the level is reset..?

        // If this level is the last level, reset the player spawn
        if (e.newLevel == SceneLoader.CurrentLevel) { transform.position = _spawnPoint; }
    }

    /// <summary>
    /// Allows the player to interact with interactable items
    /// </summary>
    public void Interact() 
    {
        // If current item slot is empty
        if (!_itemHotbar.SlotHasItem())
        {
            InteractWithItem();
        }
        // Else find the next available item slot
        else
        {
            if (_itemHotbar.CheckAvailableItemSlots())
            {
                //_itemHotbar.SwitchToItem(_itemHotbar.GetSelectedItemIndex());
                InteractWithItem();
            }
        }
    }

    /// <summary>
    /// On successful item interaction, saves a reference of the player inside interacted object
    /// </summary>
    public void InteractWithItem()
    {
        // TODO: Test edges cases while pulling up the journal
        if (!_playerJournal.GetInJournal() && IInteractable.Target != null)
        {
            //IInteractable.isPressed = true;
            IInteractable.Instigator = this.gameObject; // Save a reference of player inside interacted object
        }
    }
}