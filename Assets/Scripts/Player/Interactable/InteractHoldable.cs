using UnityEngine;
using wine.util.component;

namespace wine.player.interact
{
    public class InteractHoldable : MonoBehaviour, IInteractable 
    {
        [SerializeField] public float throwSpeed = 5;
        [SerializeField] public bool canBeShield = false;
        [SerializeField] public WeaponItemData weaponData = null;

        public BulletComponent bulletComponent { get; set; }
        public DestroyableObject destroyableComponent { get; set; }
        public Rigidbody rb { get; set; }
        public Transform realTransform { get; set; }
        public DamageComponent damageComponent { get; set; }
        public Collider col { get; set; }

        private ItemShadowComponent shadow;

        private void OnEnable()
        {
            realTransform = transform.parent;
            destroyableComponent = GetComponentInParent<DestroyableObject>();
            bulletComponent = GetComponentInParent<BulletComponent>();
            damageComponent = GetComponent<DamageComponent>();
            col = realTransform.GetComponent<Collider>();
            shadow = realTransform.GetComponent<ItemShadowComponent>();
            rb = GetComponentInParent<Rigidbody>();

            bulletComponent.doRotate = true;
            bulletComponent.enabled = false;
        }

        public void Interact(Transform other = null) 
        {
            if (other.TryGetComponent<PlayerCollisionTrigger>(out PlayerCollisionTrigger p))
            {
                p.ChangeHoldItem(this);
                if (weaponData != null)
                    wine.player.ui.InventoryUI.instances.AddItem(this.weaponData, true);
                if (shadow != null)
                    shadow.DisableShadow();
            }
        }

        public void Sling(Vector3 dir)
        {
            col.enabled = true;
            realTransform.rotation = Quaternion.Euler(0, 0, 90);
            bulletComponent.enabled = true;
            bulletComponent.Move(throwSpeed, new Vector3(dir.normalized.x, 0, dir.normalized.z), ()=> RevertBackMask(), 0.25f, true);
        }

        public void RevertBackMask()
        {
            realTransform.gameObject.layer = 3;
            this.gameObject.layer = 9;
        }
    }
}
