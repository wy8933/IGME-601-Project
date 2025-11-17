using UnityEngine;
using ItemSystem;
using AudioSystem;

public class Mop : ItemInstance
{
    private CapsuleCollider _capsuleCollider;
    private CleaningManager _cleaningManager;

    private Vector3 equipPosOffset = new Vector3(0, 0, -0.5f);
    private Quaternion equipRotOffset = Quaternion.Euler(90, 270, 90);

    [Header("Reaction SFX")]
    [SerializeField] private SoundDataSO _failedSFX;
    [SerializeField] private SoundDataSO _successSFX;
    public override SoundDataSO InitialSFX => null;
    public override SoundDataSO FailedSFX { get => _failedSFX; }
    public override SoundDataSO CancelSFX => null;
    public override SoundDataSO SuccessSFX { get => _successSFX; }

    private void Start()
    {
        Initialize();

        _rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(player != null)
        {
            _cleaningManager = player.GetComponent<CleaningManager>();
        }
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

        if(_cleaningManager != null)
        {
            _cleaningManager.ToggleCleaningMode();
        }
    }

    public override void AttachToParent(GameObject parent)
    {
        base.AttachToParent(parent);
        this.transform.localPosition = equipPosOffset;
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
        _capsuleCollider.enabled = false;
    }

    public override void EnableRigidBodyCollisions()
    {
        base.EnableRigidBodyCollisions();
        _capsuleCollider.enabled = true;
    }
}
