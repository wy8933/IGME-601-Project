using UnityEngine;
using ItemSystem;
using AudioSystem;

public class Key : ItemInstance
{
    private BoxCollider _boxCollider;
    //[Header("Key ID")]
    [SerializeField] string _keyID;

    private Quaternion equipRotOffset = Quaternion.Euler(-30, 80, 0);

    [Header("Reaction SFX")]
    [SerializeField] private SoundDataSO _failedSFX;
    [SerializeField] private SoundDataSO _successSFX;
    public override SoundDataSO InitialSFX => null;
    public override SoundDataSO FailedSFX { get => _failedSFX; }
    public override SoundDataSO CancelSFX => null;
    public override SoundDataSO SuccessSFX { get => _successSFX; }

    public string GetKeyID()
    {
        return this.item.itemID;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Initialize();

        _rb = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();

        this.item.itemID = _keyID; 
    }

    public override void Interact()
    {
        if (IInteractable.Instigator != null)
        {
            IInteractable.Instigator.GetComponent<PlayerController>().GetItemHotbar().AddItem(this.gameObject);
        }
    }

    public override void Use(GameObject user)
    {
        TryUse(user);
        // Get a reference to the player controller script
        PlayerController pc = user.GetComponent<PlayerController>();

        // interaction range for the key
        float interactRange = 10.0f;

        // Raycast from the player's camera position straight ahead and see if we hit something
        if (Physics.Raycast(user.GetComponent<PlayerController>().GetPlayerCamera().transform.position, 
            user.GetComponent<PlayerController>().GetPlayerCamera().transform.forward,
            out RaycastHit hit, interactRange, user.GetComponent<PlayerController>().IgnorePlayerMask))
        {
            // On successful raycast, see if the object hit has the DoorController script attached
            DoorController dc = hit.collider.gameObject.GetComponent<DoorController>();

            // If so, check if the key's ID is equal to the door's ID
            // Set the door's canInteract to true, effectively unlocking the door for the player to open/close
            if (dc != null)
            {
                if (this.item.itemID == dc.DoorID && !dc.CanInteract)
                {
                    dc.CanInteract = true;
                    dc.ToggleDoor();
                    Destroy(this.gameObject);
                }
            }
        }
    }

    public override void AttachToParent(GameObject parent)
    {
        base.AttachToParent(parent);
        this.transform.localRotation = equipRotOffset;
        DisableRigidBodyCollisions();
    }

    public override void DetachFromParent(GameObject parent)
    {
        base.DetachFromParent(parent);
    }

    public override void DisableRigidBodyCollisions()
    {
        base.DisableRigidBodyCollisions();
        _boxCollider.enabled = false;
    }

    public override void EnableRigidBodyCollisions()
    {
        base.EnableRigidBodyCollisions();
        _boxCollider.enabled = true;
    }
}
