using UnityEngine;
using ItemSystem;
using AudioSystem;
using System.Collections;

public class Trap : ItemInstance
{
    private BoxCollider _boxCollider;
    
    [Header("Trap Slow Duration")]
    [SerializeField] private float _slowDuration = 4.0f;

    [Header("Trap Slow Amount")]
    [SerializeField] private float _slowAmount = 1.0f;

    private bool _isSet;

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

        _isSet = false;
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

        PlaceTrap(user);
    }

    /// <summary>
    /// Method called when player places this trap item in the game world 
    /// </summary>
    /// <param name="parent"></param>
    private void PlaceTrap(GameObject parent)
    {
        PlayerController pc = parent.GetComponent<PlayerController>();

        if (pc != null)
        {
            Vector3 newPos = parent.transform.position + parent.transform.forward * _dropDistanceOffset;
            transform.position = newPos;
            this.gameObject.transform.parent = null;

            pc.GetItemHotbar().DropItem();

            _isSet = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "RuleKeeper" && _isSet)
        {
            GameObject gameObject = collision.gameObject;

            this.gameObject.GetComponent<Renderer>().enabled = false;
            StartCoroutine(ApplySlowdown(gameObject));
        }
    }

    /// <summary>
    /// Applies a temporary slowdown to the target game object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private IEnumerator ApplySlowdown(GameObject obj)
    {
        // Access rulekeeper walk speed here and change it temporarily
        EnemyBehavior eb = obj.GetComponent<EnemyBehavior>();

        if(eb != null)
        {
            eb.Speed = _slowAmount;
            Debug.Log("Walk Speed: " + eb.Speed);

            yield return new WaitForSeconds(_slowDuration);

            eb.Speed = eb.WalkSpeed;
            Debug.Log("Walk Speed: " + eb.Speed);
            
            Destroy(this.gameObject);
        }
    }
}
