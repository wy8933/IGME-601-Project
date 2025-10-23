using UnityEngine;
using ItemSystem;

[CreateAssetMenu(fileName = "MopSO", menuName = "ItemSystem/MopSO")]
public class MopSO : ItemDataSO
{
    // You can add as much variable as you want
    [Header("Extra variables for this item")]
    [TextArea] public string message = "Used the Mop!";
    public bool includeUserName = true;

    /// <summary>
    /// Write the logic that you want the item to function, like this one is a item that simply prints a message
    /// </summary>
    /// <param name="user"> Who used the Item</param>
    /// <param name="instance">The reference of the item instance</param>
    /// <returns></returns>
    public override bool Use(GameObject user, ItemInstance instance)
    {
        string who = includeUserName && user != null ? $"[{user.name}] " : "";
        //Debug.Log($"{who}{message} {itemID}");


        // Return true so ItemInstance.TryUse() knows the use is succesful
        return true;
    }
}
