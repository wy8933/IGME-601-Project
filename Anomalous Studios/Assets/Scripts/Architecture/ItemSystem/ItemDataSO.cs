using UnityEngine;

namespace ItemSystem
{
    public enum ItemType { SingleUse, Tool, Other}

    [CreateAssetMenu(fileName = "ItemDefinitionSO", menuName = "ItemSystem/ItemDefinitionSO")]
    public abstract class ItemDataSO  : ScriptableObject
    {
        [Tooltip("Definitions")]
        public string itemName;
        public int itemID;
        public Sprite itemIcon;

        [Header("Use Flags")]
        public bool hasDurability = false;
        public int durability = 1;
        public float cooldownSeconds = 0f;

        /// <summary>
        /// Write the logic that you want the item to function, like this one is a item that simply prints a message
        /// </summary>
        /// <param name="user"> Who used the Item</param>
        /// <param name="instance">The reference of the item instance</param>
        /// <returns></returns>
        public abstract bool Use(GameObject user, ref ItemInstance instance);
    }
}
