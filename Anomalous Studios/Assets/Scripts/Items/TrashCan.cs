using UnityEngine;
using ItemSystem;

public class TrashCan : MonoBehaviour
{
    private CapsuleCollider _capsuleCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
     void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Garbage garbageScript = collision.gameObject.GetComponent<Garbage>();

        if (garbageScript)
        {
            if (collision.gameObject.CompareTag(garbageScript.Tag))
            {
                Debug.Log("Garbage Collected!");
                Destroy(collision.gameObject);
            }
        }
        
    }
}
