using UnityEngine;
using wine.util.component;
using wine.util;
using wine.player.ui;

namespace wine.player.interact
{
    public class PlayerCollisionTrigger : MonoBehaviour
    {
        [SerializeField] public LayerMask roomBoundLayer;
        [SerializeField] private Transform holdPoint;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private GameObject interactGUI;
        [SerializeField] private LayerMask interactLayer;
        [SerializeField] private PlayerAnimationTrigger animTrigger; 
        [SerializeField] private float mass = 0.3f;

        private IInteractable interactable;
        private InteractHoldable holdedObject;
        private HitflashComponent hitflash;
        private KnockbackComponent knockback;
        private PlayerController controller;
        private CharacterController ccon;
        private PlayerStats stats;
        private PlayerThrowTrajectoryHint hintController;

        private float startTime;

        public bool isHoldingItem() { return holdedObject == null; }
        private bool onAim;

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
            hintController = GetComponent<PlayerThrowTrajectoryHint>();

            ccon = GetComponent<CharacterController>();
            hitflash = GetComponent<HitflashComponent>();
            knockback = GetComponent<KnockbackComponent>();
        }

        private void Start()
        {
            stats = PlayerStats.instances;
            hintController.hint.enabled = false;
            Invoke("InitializeCamera", 1.0f);

            animTrigger.throwEvent = () => 
            {
                if (holdedObject.weaponData != null)
                    InventoryUI.instances.InstantDequipDiscard(holdedObject.weaponData);

                holdedObject.rb.constraints = RigidbodyConstraints.None;
                holdedObject.Sling((controller.Pointer.position - transform.position).normalized);
                holdedObject.damageComponent.enabled = true;
                holdedObject.realTransform.parent = null;
                holdedObject = null;
                interactable = null;
                controller.weapon = "unarmed";
            };
        }

        private void Update()
        {
            // NOTE: INTERACT 
            if (interactable != null && InputController.instances.GetInput("interact"))
            {
                interactable.Interact(this.transform);
                interactable = null;
                interactGUI.SetActive(false);
            }

            if (holdedObject == null)
                return;

            // NOTE: THROW SHIT
            if (InputController.instances.GetInput("sling", InputType.hold))
            {
                hintController.hint.enabled = true;
                controller.LookAtPointer();
                onAim = true;
            }

            if (InputController.instances.GetInput("sling", InputType.release) && onAim)
            {
                onAim = false;
                hintController.hint.enabled = false;

                controller.onThrow = true;
                controller.ChangeAnim("throw");
            }

            // NOTE: ADD HOLDED ITEM INTO INVENTORY
            if (InputController.instances.GetInput("addItem"))
            {
                string wtag = holdedObject.realTransform.name;
                holdedObject.weaponData.onEquip = ()=> 
                {
                    GameObject hw = Pool.instances.CreateObject(wtag, Vector3.zero, Vector3.zero);
                    ChangeHoldItem(hw.GetComponentInChildren<InteractHoldable>());
                };

                InventoryUI.instances.InstantDequipDiscard(holdedObject.weaponData, false);
                GameObject hobj = holdedObject.realTransform.gameObject;
                holdedObject = null;

                Destroy(hobj); // NOTE: for now just destroy it
                controller.weapon = "unarmed";
            }
        }

        public void ChangeHoldItem(InteractHoldable holdObject)
        {
            if (holdedObject != null)
            {
                holdedObject.RevertBackMask();
                holdedObject.realTransform.SetParent(null);
                holdedObject.rb.constraints = RigidbodyConstraints.None;
                holdedObject.rb.linearVelocity = Vector3.zero;
                holdedObject.rb.useGravity = true;
                holdedObject.col.enabled = true;

                controller.weapon = "unarmed";
            }

            holdedObject = holdObject;
            holdedObject.gameObject.layer = 11;
            holdedObject.realTransform.gameObject.layer = 11;
            holdedObject.damageComponent.damage = stats.damage/2;
            holdedObject.damageComponent.enabled = false;

            holdedObject.col.enabled = false;
            holdedObject.bulletComponent.enabled = false;

            holdedObject.realTransform.SetParent(holdPoint.transform);
            holdedObject.realTransform.localPosition = Vector3.zero;
            holdedObject.realTransform.localRotation = holdedObject.weaponData != null ? Quaternion.Euler(holdedObject.weaponData.onHoldRotation) : Quaternion.identity;

            holdedObject.rb.useGravity = false;
            holdedObject.rb.linearVelocity = Vector3.zero;
            holdedObject.rb.constraints = RigidbodyConstraints.FreezeRotation;

            if (holdedObject.weaponData != null)
            {
                // NOTE: for now weapon data will always be == sword
                controller.weapon = "sword";
            } else {
                controller.weapon = "unarmed";
            }
        }

        private void GetHurt(int damage)
        {
            if (!controller.canBeHurt)
                return;

            if (controller.state == "roll")
                return;

            startTime = Time.time;
            PlayerUI.instances.HealthOnDamaged(damage);

            StartCoroutine(hitflash.FlashesCoroutine());
            TimeManager.instances.HitStop(0.25f);
        }

        // NOTE: Camera and Bounding Box Controller
        private void InitializeCamera()
        {
            Collider[] checkBounding = Physics.OverlapBox(transform.position, Vector3.one, Quaternion.identity, roomBoundLayer);
            if (checkBounding.Length > 0)
            {
                if (checkBounding[0].TryGetComponent<RoomTriggerComponent>(out RoomTriggerComponent rtc))
                    rtc.TriggerBoundingBox();
            }
        }

        public void MoveRoom(BoxCollider boxCollider, bool isHorizontal)
        {
            Vector3 newPos = new Vector3(
                    isHorizontal ? (controller.ActualMesh.forward.x > 0 ? boxCollider.bounds.max.x : boxCollider.bounds.min.x) : transform.position.x,
                    transform.position.y,
                    isHorizontal ? transform.position.z : (controller.ActualMesh.forward.z > 0 ? boxCollider.bounds.max.z : boxCollider.bounds.min.z)
                    );

            ccon.Move(Vector3.zero);
            ccon.enabled = false;

            transform.position = newPos + (controller.ActualMesh.forward * 2.5f);
            Invoke("ReenableController", 0.2f);
            InitializeCamera();
        }

        // NOTE: Collision Trigger 
        private void OnTriggerEnter(Collider other)
        {
            if (((1<<other.gameObject.layer) & enemyLayer) != 0)
            {
                if (other.TryGetComponent<DamageComponent>(out DamageComponent d))
                {
                    if (d.damage == 0)
                        return;

                    knockback.StartKnock(transform.position - other.transform.position, mass, stats.knockforce/4);
                    if (d.destroyOnEnd)
                        Pool.instances.DestroyObject(d.gameObject);
                    if (holdedObject != null && holdedObject.canBeShield)
                    {
                        holdedObject.destroyableComponent.TakeDamage(d.damage, Vector3.zero);
                        if (holdedObject.destroyableComponent.Health <= 0)
                        {
                            GameObject hb = holdedObject.realTransform.gameObject;
                            holdedObject = null;
                            Pool.instances.DestroyObject(hb);
                        }
                    } else {
                        GetHurt(d.damage);
                    }
                }
            }

            if (((1<<other.gameObject.layer) & interactLayer) != 0)
            {
                if (other.gameObject.TryGetComponent<InteractDoor>(out InteractDoor d))
                    d.EnterTransition();

                if (other.gameObject.TryGetComponent<InteractItem>(out InteractItem i))
                    i.Interact();

                if (other.gameObject.TryGetComponent<IInteractable>(out IInteractable it))
                {
                    interactGUI.SetActive(true);
                    interactable = it;
                }
            }

            // if (((1<<other.gameObject.layer) & dodgeWindow) != 0)
            //     onPDodge = true;
        }

        private void OnControllerColliderHit(ControllerColliderHit other) 
        {
            if (((1<<other.gameObject.layer) & interactLayer) != 0)
            {
                if (other.gameObject.TryGetComponent<InteractDoor>(out InteractDoor d))
                    d.EnterTransition();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // if (((1<<other.gameObject.layer) & dodgeWindow) != 0)
            //     onPDodge = false;

            if (((1<<other.gameObject.layer) & interactLayer) != 0)
            {
                if (interactable != null)
                {
                    interactable = null;
                    interactGUI.SetActive(false);
                }

                // if (other.gameObject.TryGetComponent<InteractDoor>(out InteractDoor id))
                // {
                //     if (id.forChangingFloor)
                //         InitializeCamera();
                // }
            }
        }


    }
}
