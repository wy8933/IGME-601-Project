using UnityEngine;
using ItemSystem;
using AudioSystem;
using System.Collections.Generic;

public class Trap : ItemInstance
{
    private BoxCollider _boxCollider;

    private float _slowDuration = 1.0f;

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
    }

    private void OnCollisionEnter(Collision collision)
    {
        // change player to rulekeeper
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("Trap collision detected with player!");
            GameObject gameObject = collision.gameObject;

            StartCoroutine(ApplySlowdown(gameObject));
        }
    }

    private IEnumerator ApplySlowdown(GameObject obj)
    {
        
        yield return new WaitForSeconds(_slowDuration);
    }
}
