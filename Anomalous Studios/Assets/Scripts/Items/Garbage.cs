using UnityEngine;
using ItemSystem;
using AudioSystem;

public class Garbage : ItemInstance
{
    private BoxCollider _boxCollider;

    public string Tag = "Garbage";
    private float _positionOffset = 1.5f;

    [Header("Throw Force")]
    [SerializeField] private float _throwForwardForce = 5.0f;
    [SerializeField] private float _throwUpForce = 5.0f;

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

        Throw(user);
    }

    private void Throw(GameObject parent)
    {
        // Set initital position of garbage item 
        Vector3 newPos = parent.transform.position + parent.transform.forward * _positionOffset;
        transform.position = newPos;

        // Unparent garbage item
        this.gameObject.transform.parent = null;

        // Re-enable necessary settings 
        CanInteract = true;
        _pickedUp = false;
        EnableRigidBodyCollisions();

        // Calculate throwing forces
        Vector3 throwForwardDirection = parent.GetComponent<PlayerController>().GetPlayerCamera().transform.forward; 
        Vector3 throwForwardForce = throwForwardDirection.normalized * _throwForwardForce;

        Vector3 throwUpDirection = parent.GetComponent<PlayerController>().GetPlayerCamera().transform.up;
        Vector3 throwUpForce = throwUpDirection.normalized * _throwUpForce;

        // Apply throwing forces
        _rb.AddForce(throwForwardForce, ForceMode.Impulse);
        _rb.AddForce(throwUpForce, ForceMode.Impulse);

        // Update ItemHotbar 
        IInteractable.Instigator.GetComponent<ItemHotbar>().OnThrown();
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
