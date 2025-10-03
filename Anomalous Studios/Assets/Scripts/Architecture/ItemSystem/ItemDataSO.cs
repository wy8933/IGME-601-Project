using UnityEngine;

namespace ItemSystem
{
    public enum ItemType { SingleUse, Tool, Other}

    [CreateAssetMenu(fileName = "ItemDefinitionSO", menuName = "ItemSystem/ItemDefinitionSO")]
    public abstract class ItemDataSO  : ScriptableObject
    {
        [Header("Definitions")]
        [Tooltip("The name of the item")]
        public string itemName;
        [Tooltip("The id of the item")]
        public string itemID;
        [Tooltip("The icon of the item that will show in UI")]
        public Sprite itemIcon;

        [Header("Use Flags")]
        [Tooltip("Does the item still have durability left")]
        public bool hasDurability = false;
        [Tooltip("The remaining durability")]
        public int durability = 1;
        [Tooltip("Time cooldown between each item use")]
        public float cooldownSeconds = 0f;

        /// <summary>
        /// Write the logic that you want the item to function, like this one is a item that simply prints a message
        /// </summary>
        /// <param name="user"> Who used the Item</param>
        /// <param name="instance">The reference of the item instance</param>
        /// <returns></returns>
        public abstract bool Use(GameObject user, ItemInstance instance);
    }
}
