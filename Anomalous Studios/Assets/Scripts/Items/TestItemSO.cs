using UnityEngine;
using ItemSystem;

[CreateAssetMenu(fileName = "TestItemSO", menuName = "ItemSystem/TestItemSO")]
public class TestItemSO : ItemDefinitionSO
{
    // You can add as much variable as you want

    [TextArea] public string message = "Used the item!";
    public bool includeUserName = true;

    /// <summary>
    /// Write the logic that you want the item to function, like this one is a item that simply prints a message
    /// </summary>
    /// <param name="user"> Who used the Item</param>
    /// <param name="instance">The reference of the item instance</param>
    /// <returns></returns>
    public override bool Use(GameObject user, ref ItemInstance instance)
    {
        string who = includeUserName && user != null ? $"[{user.name}] " : "";
        Debug.Log($"{who}{message}");


        // Return true so ItemInstance.TryUse() knows the use is succesful
        return true;
    }
}
