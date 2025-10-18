using UnityEngine;
using ItemSystem;
using AudioSystem;

public class Garbage : ItemInstance
{
    private BoxCollider _boxCollider;

    public string Tag = "Garbage";
    public float launchForce = 1000.0f;
    public Transform launchPoint;


    [Header("Reaction SFX")]
    [SerializeField] private SoundDataSO _failedSFX;
    [SerializeField] private SoundDataSO _successSFX;
    public override SoundDataSO InitialSFX => null;
    public override SoundDataSO FailedSFX { get => _failedSFX; }
    public override SoundDataSO CancelSFX => null;
    public override SoundDataSO SuccessSFX { get => _successSFX; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();

        _rb = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    public void Update()
    {
        UpdateLocation();
    }

    public override void Use(GameObject user)
    {
        TryUse(user);

        Throw();
    }

    private void Throw()
    {
        this.gameObject.transform.parent = null;
        CanInteract = true;
        _pickedUp = false;
        EnableRigidBodyCollisions();
        Debug.Log("throwing garbage");
        launchPoint = this.transform;
        _rb.AddForce(launchPoint.forward * launchForce, ForceMode.Impulse);
    }

    public override void Interact()
    {
        if (IInteractable.Instigator != null)
        {
            IInteractable.Instigator.GetComponent<PlayerController>().GetItemHotbar().AddItem(this.gameObject);
        }
    }

    private void UpdateLocation()
    {
        if (_cameraTransform && _pickedUp)
        {
            transform.localPosition = _itemCamPosOffset;
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
