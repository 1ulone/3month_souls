using UnityEngine;
using wine.util.component;
using wine.util;
using wine.player.ui;

namespace wine.player.interact
{
    public class PlayerCollisionTrigger : MonoBehaviour
    {
        [SerializeField] public LayerMask roomBoundLayer;
        [SerializeField] private LayerMask interactLayer;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private GameObject interactGUI;
        [SerializeField] private Transform holdPoint;
        [SerializeField] private float mass = 0.3f;

        private IInteractable interactable;
        private InteractHoldable holdedObject;
        private HitflashComponent hitflash;
        private KnockbackComponent knockback;
        private PlayerController controller;
        private CharacterController ccon;
        private PlayerStats stats;

        private float startTime;

        public bool isHoldingItem() { return holdedObject == null; }

        private void Awake()
        {
            controller = GetComponent<PlayerController>();

            ccon = GetComponent<CharacterController>();
            hitflash = GetComponent<HitflashComponent>();
            knockback = GetComponent<KnockbackComponent>();
        }

        private void Start()
        {
            stats = PlayerStats.instances;
            Invoke("InitializeCamera", 1.0f);
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

            // NOTE: THROW SHIT
            if (holdedObject != null && InputController.instances.GetInput("sling"))
            {
                holdedObject.rb.constraints = RigidbodyConstraints.None;
                holdedObject.Sling(controller.Pointer.transform.position - transform.position);
                holdedObject.damageComponent.enabled = true;
                holdedObject.realTransform.parent = null;
                holdedObject = null;
            }

        }

        public void ChangeHoldItem(InteractHoldable holdObject)
        {
            if (holdedObject != null)
            {
                holdedObject.RevertBackMask();
                holdedObject.realTransform.SetParent(null);
                holdedObject.rb.linearVelocity = Vector3.zero;
                holdedObject.rb.useGravity = true;
                holdedObject.rb.constraints = RigidbodyConstraints.None;
            }

            holdedObject = holdObject;
            holdedObject.gameObject.layer = 11;
            holdedObject.realTransform.gameObject.layer = 11;
            holdedObject.damageComponent.damage = stats.damage/2;
            holdedObject.damageComponent.enabled = false;

            holdObject.realTransform.SetParent(holdPoint.transform);
            holdObject.realTransform.localPosition = Vector3.zero;

            holdedObject.rb.linearVelocity = Vector3.zero;
            holdedObject.rb.useGravity = false;
            holdedObject.rb.constraints = RigidbodyConstraints.FreezeRotation;
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
