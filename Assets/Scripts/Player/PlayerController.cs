using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float rotationSpeed = 5;
    [SerializeField] private float crossFadeTime = 1.25f;
    [SerializeField] private float dashTime = 1f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashCooldown = 1.5f;
    [SerializeField] private float attackTime = 1f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float mass = 0.3f;
    [SerializeField] private Transform targetMesh;
    [SerializeField] private Transform pointer;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private GameObject interactGUI;
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask parryWindow;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] public LayerMask roomBoundLayer;

    public Transform Pointer { get { return pointer; } }
    public Transform ActualMesh { get { return targetMesh; } }
    public bool canBeHurt { get; set; }
    
    private CharacterController controller;
    private InputController input;
    private HitflashComponent hitflash;
    private KnockbackComponent knockback;
    private IInteractable interactable;
    private InteractHoldable holdedObject;
    private PlayerStats stats;
    private CinemachineImpulseSource shakeSource;

    private string state;
    private float startTime;
    private float vintake;
    private bool canRoll, canAttack;
    private int health;

    private Coroutine currentCoroutine;
    private Vector2 dir;
    private Vector3 rollingDir;
    private Vector3 lookRotation;
    private int defaultLayer;
    private int invincible;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<InputController>();
        hitflash = GetComponent<HitflashComponent>();
        knockback = GetComponent<KnockbackComponent>();
        shakeSource = GetComponent<CinemachineImpulseSource>();

        defaultLayer = LayerMask.NameToLayer("Player");
        invincible = LayerMask.NameToLayer("Invincible");
        interactGUI.SetActive(false);

        state = "";

        canRoll = true;
        canAttack = true;
        canBeHurt = true;
        vintake = 0;

        ChangeAnim("idle");
    }

    private void Start()
    {
        stats = PlayerStats.instances;
        health = stats.health;

        PlayerUI.instances.UpdateHealthUI(health, stats.health);
        PlayerUI.instances.UpdateExpUI(stats.exp);

        Invoke("InitializeCamera", 1.0f);
    }

    private void InitializeCamera()
    {
        Collider[] checkBounding = Physics.OverlapBox(transform.position, Vector3.one, Quaternion.identity, roomBoundLayer);
        if (checkBounding.Length > 0)
        {
            if (checkBounding[0].TryGetComponent<RoomTriggerComponent>(out RoomTriggerComponent rtc))
                rtc.TriggerBoundingBox();
        }
    }

    private void Update()
    {
        if (FadeTransitionUI.isTransitioning)
        {
            dir = Vector2.zero;
            ChangeAnim("idle");
            return;
        }

        if (input.inventory.WasPressedThisFrame())
            InventoryUI.instances.ToggleInventory();

        if (!canBeHurt)
        {
            dir = Vector2.zero;
            ChangeAnim("idle"); // TODO: change to hurt later
            if (startTime + stats.downtime/2 < Time.time)
                EndHurt();

            return;
        }

        if (Time.timeScale == 0 || !controller.enabled)
        {
            ChangeAnim("idle");
            return;
        }
        
        // NOTE: INTERACT 
        if (interactable != null && input.interact.WasPressedThisFrame())
        {
            interactable.Interact(this.transform);
            interactable = null;
            interactGUI.SetActive(false);
        }
    
        // NOTE: THROW SHIT
        if (holdedObject != null && input.sling.WasPressedThisFrame())
        {
            holdedObject.rb.constraints = RigidbodyConstraints.None;
            holdedObject.Sling(pointer.transform.position - transform.position);
            holdedObject.damageComponent.enabled = true;
            holdedObject.realTransform.parent = null;
            holdedObject = null;
        }

        if (input.heal.IsInProgress() && holdedObject == null && PlayerStats.instances.currentVessel > 1.0f && health < stats.health)
        {
            if (vintake >= 1.0f)
            {
                vintake = 0.4f;
                PlayerStats.instances.ControlVessel(-1);

                health += 1;
                PlayerUI.instances.UpdateHealthUI(health, stats.health);
            } else { vintake += 1 - Mathf.Sqrt(1-Mathf.Pow(Time.deltaTime*17.5f, 2)); } 
        }

        if (input.heal.WasReleasedThisFrame())
            vintake = 0;

        dir = input.move.ReadValue<Vector2>();
        lookRotation = new Vector3(dir.x, 0, dir.y);

        if (input.roll.WasPressedThisFrame() && canRoll) 
        {
            if (currentCoroutine != null && state != "roll")
                StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(Roll());
        }
        
        if (state == "roll")
            return; 

        if (input.attack.WasPressedThisFrame() && canAttack)
            currentCoroutine = StartCoroutine(Attack());

        if (state == "attack")
            return;

        if (dir != Vector2.zero)
        {
            ChangeAnim("walk");
            Quaternion trot = Quaternion.LookRotation(lookRotation, Vector3.up);
            targetMesh.rotation = Quaternion.RotateTowards(targetMesh.rotation, trot, rotationSpeed * Time.unscaledDeltaTime * 50);
        } else 
        if (dir == Vector2.zero)
        {
            ChangeAnim("idle");
        }
    }

    private void FixedUpdate()
    {
        if (FadeTransitionUI.isTransitioning)
            return;

        if (Time.timeScale == 0 || !controller.enabled)
            return;

        if (state == "roll")
        {
            controller.Move(new Vector3(rollingDir.normalized.x, -98.1f * Time.deltaTime, rollingDir.normalized.z) * (dashSpeed + stats.rollspeed) * Time.fixedUnscaledDeltaTime);
            return; 
        }

        if (state == "attack")
            return;

        Vector3 move = transform.right * dir.x + transform.forward * dir.y;
        move.y = -98.1f * Time.deltaTime;
        controller.Move(move * stats.speed * Time.fixedUnscaledDeltaTime);
    }

    public void ChangeAnim(string newState)
    {
        if (state != newState)
            anim.CrossFade(newState, crossFadeTime);
        state = newState;
    }

    private IEnumerator Roll()
    {
        canRoll = false;
        // if (onPDodge)
        //     TimeManager.instances.SlowTime();
        //
        // onPDodge = false;
        this.gameObject.layer = invincible;

        rollingDir = targetMesh.forward;
        if (dir != Vector2.zero)
        {
            rollingDir = lookRotation;
            targetMesh.rotation = Quaternion.LookRotation(lookRotation, Vector3.up);
        }

        ChangeAnim("roll");

        float timer = 0f;
        while (timer < dashTime)
        {
            timer += TimeManager.instances.onSlow ? Time.unscaledDeltaTime : Time.deltaTime;
            yield return null;
        }

        this.gameObject.layer = defaultLayer;
        ChangeAnim("idle");

        yield return new WaitForSecondsRealtime(dashCooldown);
        canRoll = true;
        canAttack = true;
        currentCoroutine = null;
    }

    private IEnumerator Attack()
    {
        canAttack = false;
        targetMesh.LookAt(new Vector3(pointer.position.x, targetMesh.position.y, pointer.position.z));
        ChangeAnim("attack");

        float timer = 0f;
        while (timer < attackTime)
        {
            timer += TimeManager.instances.onSlow ? Time.unscaledDeltaTime : Time.deltaTime;
            yield return null;
        }

        ChangeAnim("idle");
        
        yield return new WaitForSecondsRealtime(attackCooldown);
        canRoll = true;
        canAttack = true;
        currentCoroutine = null;
    }

    private void GetHurt(int damage)
    {
        if (!canBeHurt)
            return;

        if (state == "roll")
            return;

        startTime = Time.time;
        int tdamage = stats.defense - damage > 0 ? 0 : stats.defense - damage;
        health -= Mathf.Abs(tdamage);
        Debug.Log(tdamage);
        canBeHurt = false;

        if (health <= 0)
            GameOverUI.instances.StartPanel();

        shakeSource.GenerateImpulse();
        PlayerUI.instances.UpdateHealthUI(health, maxHealth);

        // EFFECTS
        StartCoroutine(hitflash.FlashesCoroutine());
        TimeManager.instances.HitStop(0.25f);
    }

    private void EndHurt()
    {
        startTime = 0;
        canBeHurt = true;
        canAttack = true;
        canRoll = true;
    }

    public void MoveRoom(BoxCollider boxCollider, bool isHorizontal)
    {
        Vector3 newPos = new Vector3(
            isHorizontal ? (ActualMesh.forward.x > 0 ? boxCollider.bounds.max.x : boxCollider.bounds.min.x) : transform.position.x,
            transform.position.y,
            isHorizontal ? transform.position.z : (ActualMesh.forward.z > 0 ? boxCollider.bounds.max.z : boxCollider.bounds.min.z)
        );

        controller.Move(Vector3.zero);
        controller.enabled = false;

        transform.position = newPos + (ActualMesh.forward * 2.5f);
        Invoke("ReenableController", 0.2f);
        InitializeCamera();
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

    private void ReenableController()
        { controller.enabled = true; controller.Move(Vector3.zero); }

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

            if (other.gameObject.TryGetComponent<InteractDoor>(out InteractDoor id))
            {
                if (id.forChangingFloor)
                    InitializeCamera();
            }
        }
    }
}
