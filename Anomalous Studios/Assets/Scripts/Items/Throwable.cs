using UnityEngine;
using ItemSystem;
using AudioSystem;

public class Throwable : ItemInstance
{
    private CapsuleCollider _capsuleCollider;
    private float _positionOffset = 1.5f;
    private bool _isThrown = false;

    [Header("Rulekeeper")]
    [SerializeField] private GameObject _ruleKeeper;
    private EnemyBehavior _enemyBehavior;

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

    [Header("Sound Data")]
    [SerializeField] private SoundDataSO _breakSO;
    private float _audioCooldownTime = 0.5f;
    private float lastPlayTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();

        _rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        if(_ruleKeeper != null)
        {
            _enemyBehavior = _ruleKeeper.GetComponent<EnemyBehavior>();
        }
    }
    public override void Use(GameObject user)
    {
        TryUse(user);

        Throw(user);
    }

    /// <summary>
    /// Logic for when player throws this item
    /// </summary>
    /// <param name="parent"></param>
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

        _isThrown = true;

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

    private void OnCollisionEnter(Collision collision)
    {
        if(_isThrown && _enemyBehavior != null)
        {
            // when item breaks, set RuleKeeper's target location to this item's location
            MakeNoise makeNoise = new MakeNoise();
            makeNoise.target = this.transform.position;

            Debug.Log("distract rulekeeper");

            // play item's break sound effect
            PlaySound(_breakSO);
            // destroy breakable thrown game object
            Destroy(this.gameObject); 
        }
    }

    /// <summary>
    /// Plays an audio sound and prevents audio clip from spamming
    /// </summary>
    /// <param name="sd">Sound Data Scriptable Object</param>
    public void PlaySound(SoundDataSO sd)
    {
        if (Time.time - lastPlayTime >= _audioCooldownTime)
        {
            if (AudioManager.Instance)
            {
                AudioManager.Instance.Play(sd, this.transform.position);
            }
            lastPlayTime = Time.time;
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
        _capsuleCollider.enabled = false;
    }

    public override void EnableRigidBodyCollisions()
    {
        base.EnableRigidBodyCollisions();
        _capsuleCollider.enabled = true;
    }
}
