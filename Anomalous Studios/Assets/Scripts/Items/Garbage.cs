using UnityEngine;
using ItemSystem;

public class Garbage : ItemInstance
{
    private Transform _cameraTransform;
    private Vector3 _itemCamPosOffset = new Vector3(0.3f, -0.3f, 0.3f);
    private float _dropDistanceOffset = 1.5f;
    private Rigidbody _rb;
    private BoxCollider _boxCollider;

    public string Tag = "Garbage";

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
        _cameraTransform = parent.transform.GetChild(1).transform.GetChild(0).transform;

        PickUp();
        DisableRigidBodyCollisions();

        this.gameObject.transform.SetParent(_cameraTransform, false);
        transform.localPosition = _itemCamPosOffset;
        transform.localRotation = Quaternion.Euler(90, 0, 0);
    }

    public override void DetachFromParent(GameObject parent)
    {
        Vector3 newPos = parent.transform.position + parent.transform.forward * _dropDistanceOffset;
        transform.position = newPos;
        this.gameObject.transform.parent = null;
        CanInteract = true;
        _pickedUp = false;
    }

    public override void DisableRigidBodyCollisions()
    {
        _boxCollider.enabled = false;
        _rb.isKinematic = true;
        _rb.detectCollisions = false;
        _rb.useGravity = false;
    }

    public override void EnableRigidBodyCollisions()
    {
        _boxCollider.enabled = true;
        _rb.isKinematic = false;
        _rb.detectCollisions = true;
        _rb.useGravity = true;
    }
}
