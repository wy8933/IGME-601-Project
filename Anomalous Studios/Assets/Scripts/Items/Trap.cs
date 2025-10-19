using UnityEngine;
using ItemSystem;
using AudioSystem;
using System.Collections;

public class Trap : ItemInstance
{
    private BoxCollider _boxCollider;

    [Header("Trap Slow Duration")]
    [SerializeField] private float _slowDuration = 4.0f;

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
    void Update()
    {

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

    private void PlaceTrap(GameObject parent)
    {
        Vector3 newPos = parent.transform.position + parent.transform.forward * _dropDistanceOffset;
        transform.position = newPos;
        this.gameObject.transform.parent = null;

        PlayerController pc = parent.GetComponent<PlayerController>();

        if (pc != null)
        {
            pc.GetItemHotbar().DropItem();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // replace player with rulekeeper
        if(collision.gameObject.tag == "Player")
        {
            GameObject gameObject = collision.gameObject;

            this.gameObject.GetComponent<Renderer>().enabled = false;
            StartCoroutine(ApplySlowdown(gameObject));
        }
    }

    private IEnumerator ApplySlowdown(GameObject obj)
    {
        // Access rulekeeper walk speed here and change it temporarily
        PlayerActions pa = obj.GetComponent<PlayerActions>();

        if(pa != null)
        {
            pa.SetWalkSpeed(0.5f);
            Debug.Log("Walk Speed: " + pa.GetWalkSpeed());

            yield return new WaitForSeconds(_slowDuration);

            pa.SetWalkSpeed(3.0f);
            Debug.Log("Walk Speed: " + pa.GetWalkSpeed());

            Destroy(this.gameObject);
        }
    }
}
