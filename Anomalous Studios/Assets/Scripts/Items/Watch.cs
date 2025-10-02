using ItemSystem;
using UnityEngine;

public class Watch : ItemInstance
{
    private Transform _cameraTransform;
    private Vector3 _itemCamPosOffset = new Vector3(0.3f, -0.3f, 0.3f);
    private float _dropDistanceOffset = 1.5f;
    private Rigidbody _rb;
    private BoxCollider _boxCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();

        _rb = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void Highlight()
    {
        // TODO: Replace with shader to highlight the item, or UI element to indicate it is interactable
        //Debug.Log("Highlighting Watch");
    }

    protected override void Interact()
    {
        if (Instigator != null)
        {
            Instigator.GetComponent<PlayerController>().AddItem(this.gameObject);
        }
    }
}
