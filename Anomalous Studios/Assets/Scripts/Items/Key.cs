using UnityEngine;
using ItemSystem;
using AudioSystem;

public class Key : ItemInstance
{
    private BoxCollider _boxCollider;

    [Header("Reaction SFX")]
    [SerializeField] private SoundDataSO _failedSFX;
    [SerializeField] private SoundDataSO _successSFX;
    public override SoundDataSO InitialSFX => null;
    public override SoundDataSO FailedSFX { get => _failedSFX; }
    public override SoundDataSO CancelSFX => null;
    public override SoundDataSO SuccessSFX { get => _successSFX; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Initialize();

        _rb = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
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
        PlayerController pc = user.GetComponent<PlayerController>();

        float interactRange = 10.0f;

        if (Physics.Raycast(user.GetComponent<PlayerController>().GetPlayerCamera().transform.position, 
            user.GetComponent<PlayerController>().GetPlayerCamera().transform.forward,
            out RaycastHit hit, interactRange, user.GetComponent<PlayerController>().IgnorePlayerMask))
        {
            DoorController dc = hit.collider.gameObject.GetComponent<DoorController>();
            if (dc != null)
            {
                if (this.item.itemID == dc.DoorID && !dc.CanInteract)
                {
                    dc.CanInteract = true;
                }
            }
        }
    }

    public override void AttachToParent(GameObject parent)
    {
        base.AttachToParent(parent);
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
