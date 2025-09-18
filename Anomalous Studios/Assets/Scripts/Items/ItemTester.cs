using ItemSystem;
using UnityEngine;

public class ItemTester : MonoBehaviour
{
    public ItemDataSO item;

    private ItemInstance _instance;

    void Start()
    {
        _instance.item = item;
        _instance.Initialize();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _instance.TryUse(gameObject);
            Debug.Log("Durability Left:"+_instance.durabilityLeft);
            Debug.Log("Is the item on cooldown?" + _instance.IsOnCooldown);
        }
    }
}
