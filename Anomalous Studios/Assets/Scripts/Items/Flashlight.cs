using ItemSystem;
using UnityEngine;

public class Flashlight : ItemInstance
{
    private Rigidbody _rb;

    [Header("Item Mesh")]
    [SerializeField] GameObject Mesh;
    [Header("Spotlight")]
    [SerializeField] GameObject Spotlight;
    private Light _lightComponent;
    private bool _isOn = false;

    private Transform _cameraTransform;
    private Vector3 offset = new Vector3(0.3f, -0.3f, 0.3f);

    [Header("Battery")]
    [SerializeField] float Battery = 100.0f;
    private float _batteryDrainAmount;  

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //_rb = GetComponent<Rigidbody>();

        if (Spotlight)
        {
            Spotlight.SetActive(_isOn);
            _lightComponent = Spotlight.GetComponent<Light>();
        }


        _batteryDrainAmount = 2.0f;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        //if(_cameraTransform)
        //{
        //    transform.position = offset;
        //}

        if (_isEquipped)
        {
            Mesh.SetActive(_isEquipped);
        }

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
    }

    public override void Highlight()
    {
        // TODO: Replace with shader to highlight the item, or UI element to indicate it is interactable
        //Debug.Log("Highlighting Flashlight");
    }

    protected override void Interact()
    {
        Debug.Log("Interacted with Flashlight");

        if(Instigator != null)
        {
            Instigator.GetComponent<PlayerController>().AddItem(this.gameObject);
        }
    }

    public override void Use(GameObject user)
    {
        //TryUse(user); // Inherited from ItemInstance
        _isOn = !_isOn;
        Spotlight.SetActive(_isOn);
    }

    public override void AttachToParent(GameObject parent)
    {
        Debug.Log("Called Child AttachToParent()");
        _cameraTransform = parent.transform.GetChild(1).transform.GetChild(0).transform;
        
        transform.position = Vector3.zero;
        transform.position = offset;

        PickUp();

        GetComponent<CapsuleCollider>().enabled = false;
        //_rb.isKinematic = false;
        //_rb.detectCollisions = false;
        //_rb.useGravity = false;

        this.gameObject.transform.SetParent(_cameraTransform, false);
        
    }
}
