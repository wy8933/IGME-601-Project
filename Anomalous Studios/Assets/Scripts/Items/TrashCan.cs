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

    private void OnCollisionEnter(Collision collision)
    {
        Garbage garbageScript = collision.gameObject.GetComponent<Garbage>();

        if (garbageScript)
        {
            if (collision.gameObject.CompareTag(garbageScript.Tag))
            {
                int count;
                int.TryParse(VariableConditionManager.Instance.Get("Trash"), out count);
                string value = (count+1).ToString();

                VariableConditionManager.Instance.Set("Trash", value);
                Destroy(collision.gameObject);
            }
        }
        
    }
}
