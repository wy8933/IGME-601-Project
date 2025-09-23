using ItemSystem;
using UnityEngine;
using UnityEngine.UIElements;

public class Flashlight : ItemInstance
{   
    [Header("Spotlight")]
    [SerializeField] GameObject Spotlight;
    private Light _lightComponent;
    private bool _isOn = false;

    [Header("Battery")]
    [SerializeField] float Battery = 100.0f;
    private float _batteryDrainAmount;

    private Transform _cameraTransform;
    private Vector3 offset = new Vector3(0.3f, -0.3f, 0.3f);
    private Rigidbody _rb;
    private CapsuleCollider _capsuleCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Initialize();

        if (Spotlight)
        {
            Spotlight.SetActive(_isOn);
            _lightComponent = Spotlight.GetComponent<Light>();
        }

        _batteryDrainAmount = 2.0f;

        _rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (_isOn)
        {
            if (Battery > 0)
            {
                Battery -= _batteryDrainAmount * Time.deltaTime;
            }
            else
            {
                _lightComponent.intensity = 0;
                _isOn = false;
            }

            Debug.Log("Battery: " + Battery);
        }

        UpdateLocation();
    }

    public override void Highlight()
    {
        // TODO: Replace with shader to highlight the item, or UI element to indicate it is interactable
        //Debug.Log("Highlighting Flashlight");
    }

    protected override void Interact()
    {
        if(Instigator != null)
        {
            Instigator.GetComponent<PlayerController>().AddItem(this.gameObject);
        }
    }

    public override void Use(GameObject user)
    {
        TryUse(user); 
        _isOn = !_isOn;
        Spotlight.SetActive(_isOn);
    }

    public override void AttachToParent(GameObject parent)
    {
        Debug.Log("Called Child AttachToParent()");

        _cameraTransform = parent.transform.GetChild(1).transform.GetChild(0).transform;
        
        PickUp();
        DisableRigidBodyCollisions();

        this.gameObject.transform.SetParent(_cameraTransform, false);
        transform.localPosition = offset;
        transform.localRotation = Quaternion.Euler(90, 0, 0);
    }

    public override void DetachFromParent()
    {
        this.gameObject.transform.parent = null;
        canInteract = true;
        _pickedUp = false;
    }

    public override void DisableRigidBodyCollisions()
    {
        _capsuleCollider.enabled = false;
        _rb.isKinematic = true;
        _rb.detectCollisions = false;
        _rb.useGravity = false;
    }

    public override void EnableRigidBodyCollisions(GameObject parent)
    {
        transform.position = parent.transform.rotation 
                  * new Vector3(parent.transform.position.x, parent.transform.position.y, parent.transform.position.z + 0.1f);

        _capsuleCollider.enabled = true;
        _rb.isKinematic = false;
        _rb.detectCollisions = true;
        _rb.useGravity = true;
    }

    private void UpdateLocation()
    {
        if (_cameraTransform && _pickedUp)
        {
            transform.localPosition = offset;
        }
    }

    public override void Equip()
    {
        base.Equip();
    }

    public override void UnEquip()
    {
        base.UnEquip();
        _isOn = false; 
        Spotlight.SetActive(false);
    }
}
