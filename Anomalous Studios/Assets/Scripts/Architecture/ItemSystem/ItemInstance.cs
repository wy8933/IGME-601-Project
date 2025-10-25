using AudioSystem;
using UnityEngine;

namespace ItemSystem
{
    public enum ItemUseResult { Success, OnCooldown, NoCharges, Failed }

    [System.Serializable]
    public abstract class ItemInstance: MonoBehaviour, IInteractable
    {
        protected Transform _cameraTransform;
        protected float _dropDistanceOffset = 1.5f;
        protected Rigidbody _rb;

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

        public abstract SoundDataSO InitialSFX { get; }
        public abstract SoundDataSO FailedSFX { get; }
        public abstract SoundDataSO CancelSFX { get; }
        public abstract SoundDataSO SuccessSFX { get; }

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
            GetComponent<HighlightTarget>().IsHighlighted = true;
        }
        public void RemoveHighlight()
        {
            GetComponent<HighlightTarget>().IsHighlighted = false;
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
        }

        public virtual void AttachToParent(GameObject parent)
        {
            _cameraTransform = parent.GetComponent<PlayerController>()._itemContainerTransform;

            PickUp();

            this.gameObject.transform.SetParent(_cameraTransform, false);
            transform.localPosition = Vector3.zero; 
            transform.localRotation = Quaternion.Euler(90, 0, 0);
        }

        public virtual void DetachFromParent(GameObject parent)
        {
            Vector3 newPos = parent.transform.position + parent.transform.forward * _dropDistanceOffset;
            transform.position = newPos;
            this.gameObject.transform.parent = null;
            CanInteract = true;
            _pickedUp = false;
        }

        public virtual void DisableRigidBodyCollisions()
        {
            _rb.isKinematic = true;
            _rb.detectCollisions = false;
            _rb.useGravity = false;
        }

        public virtual void EnableRigidBodyCollisions()
        {
            _rb.isKinematic = false;
            _rb.detectCollisions = true;
            _rb.useGravity = true;
        }
    }
}