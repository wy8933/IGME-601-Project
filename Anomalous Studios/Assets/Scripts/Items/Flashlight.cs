using AudioSystem;
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
    
    private CapsuleCollider _capsuleCollider;

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
    public void Update()
    {
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
        _isOn = !_isOn;
        Spotlight.SetActive(_isOn);
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
        _capsuleCollider.enabled = false;
    }

    public override void EnableRigidBodyCollisions()
    {
        base.EnableRigidBodyCollisions();
        _capsuleCollider.enabled = true;
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
