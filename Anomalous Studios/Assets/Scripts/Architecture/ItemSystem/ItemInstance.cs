using UnityEngine;

namespace ItemSystem
{
    public enum ItemUseResult { Success, OnCooldown, NoCharges, Failed }

    [System.Serializable]
    public abstract class ItemInstance: MonoBehaviour, IInteractable
    {
        [SerializeField] private float _holdTime = 0.0f;

        [Header("Item Mesh")]
        [SerializeField] protected GameObject Mesh;

        public ItemDataSO item;
        public int durabilityLeft;
        public float lastUseTime;

        [Tooltip("Is the Item useable, is there still remaining durability")]
        public bool isUseable;
        private bool _canInteract = true;

        public bool IsEmpty => item == null;
        public bool IsOnCooldown => item != null && (Time.time - lastUseTime) < item.cooldownSeconds;

        protected bool _isEquipped = false;
        protected bool _pickedUp = false;

        public float HoldTime { get => _holdTime; }
        public bool CanInteract { get => _canInteract; set => _canInteract = value; }

        /// <summary>
        /// Initialize durability when create/assign the instance.
        /// </summary>
        public void Initialize()
        {
            if (item == null) { durabilityLeft = 0; lastUseTime = -9999f; return; }
            durabilityLeft = Mathf.Max(1, item.durability);
            lastUseTime = -9999f;
            isUseable = true;
        }

        /// <summary>
        /// Attempts to use the item and handles cooldown & durability reduce.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Result of the Use</returns>
        public ItemUseResult TryUse(GameObject user)
        {
            if (IsEmpty) return ItemUseResult.Failed;
            if (durabilityLeft <= 0) return ItemUseResult.NoCharges;
            if (IsOnCooldown) return ItemUseResult.OnCooldown;

            var useSuccessful = item.Use(user, this);
            if (!useSuccessful) return ItemUseResult.Failed;

            lastUseTime = Time.time;
            if (item.hasDurability)
            {
                durabilityLeft--;
                if (durabilityLeft < 0) durabilityLeft = 0;

                if (durabilityLeft == 0) 
                {
                    isUseable = false;
                }
            }
            return ItemUseResult.Success;
        }

        public void Highlight()
        {
            // TODO: Replace with shader to highlight the item
        }

        public void RemoveHighlight()
        {
            // TODO: Remove the shader effect from the item
        }

        public abstract void Interact();

        public void PickUp()
        {
            _canInteract = false;
            _pickedUp = true;
            Equip();
        }

        public virtual void Equip()
        {
            _isEquipped = true;
            Mesh.SetActive(true);
        }

        public virtual void UnEquip()
        {
            _isEquipped = false;
            Mesh.SetActive(false);
        }

        public virtual void Use(GameObject user)
        {
            //Debug.Log("Called Parent Use()");
        }

        public virtual void AttachToParent(GameObject parent)
        {
            //Debug.Log("Called Parent AttachToParent()");
        }

        public virtual void DetachFromParent(GameObject parent)
        {

        }

        public virtual void DisableRigidBodyCollisions()
        {

        }

        public virtual void EnableRigidBodyCollisions()
        {

        }
    }
}