using System.Collections;
using UnityEngine;
using CameraShake;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float movementSpeed = 7;
    [SerializeField] private float rotationSpeed = 5;
    [SerializeField] private float crossFadeTime = 1.25f;
    [SerializeField] private float dashTime = 1f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashCooldown = 1.5f;
    [SerializeField] private float attackTime = 1f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float hurtCooldown = 2f;
    [SerializeField] private float mass = 0.3f;
    [SerializeField] private float force = 3f;
    [SerializeField] private Transform targetMesh;
    [SerializeField] private Transform pointer;
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask dodgeWindow;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] public LayerMask roomBoundLayer;

    public Transform Pointer { get { return pointer; } }
    public Transform ActualMesh { get { return targetMesh; } }
    
    private CharacterController controller;
    private InputController input;
    private HitflashComponent hitflash;
    private KnockbackComponent knockback;
    private IInteractable interactable;
    private PlayerStats stats;

    private string state;
    private float startTime;
    private bool canRoll, canAttack, canBeHurt;
    private bool onPDodge;
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

        defaultLayer = LayerMask.NameToLayer("Player");
        invincible = LayerMask.NameToLayer("Invincible");


        state = "";

        canRoll = true;
        canAttack = true;
        canBeHurt = true;

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
            if (startTime + stats.downtime < Time.time)
                EndHurt();

            return;
        }

        if (Time.timeScale == 0 || !controller.enabled)
        {
            ChangeAnim("idle");
            return;
        }
        
        if (interactable != null && input.interact.WasPressedThisFrame())
        {
            interactable.Interact();
            interactable = null;
        }

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
            controller.Move(rollingDir.normalized * (dashSpeed + stats.rollspeed) * Time.fixedUnscaledDeltaTime);
            return; 
        }

        if (state == "attack")
            return;

        Vector3 move = transform.right * dir.x + transform.forward * dir.y;
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
        if (onPDodge)
            TimeManager.instances.SlowTime();

        onPDodge = false;
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
        targetMesh.LookAt(new Vector3(pointer.position.x, 0, pointer.position.z));
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
        if (state == "roll")
            return;

        startTime = Time.time;
        health -= Mathf.Abs(damage - stats.defense);
        canBeHurt = false;

        CameraShaker.Shake(new PerlinShake(ShakeParams.instances.HurtSShake));
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

    private void OnTriggerEnter(Collider other)
    {
        if (((1<<other.gameObject.layer) & enemyLayer) != 0)
        {
            if (other.TryGetComponent<DamageComponent>(out DamageComponent d))
            {
                knockback.StartKnock(transform.position - other.transform.position, mass, stats.knockforce);
                GetHurt(d.damage);
            }
        }

        if (((1<<other.gameObject.layer) & interactLayer) != 0)
        {
            if (other.gameObject.TryGetComponent<InteractDoor>(out InteractDoor d))
                d.EnterTransition(this, controller, input);

            if (other.gameObject.TryGetComponent<InteractItem>(out InteractItem i))
                i.Interact();

            if (other.gameObject.TryGetComponent<IInteractable>(out IInteractable it))
                interactable = it;
        }

        if (((1<<other.gameObject.layer) & dodgeWindow) != 0)
            onPDodge = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1<<other.gameObject.layer) & dodgeWindow) != 0)
            onPDodge = false;

        if ((((1<<other.gameObject.layer) & interactLayer) != 0) && interactable != null)
            interactable = null;
    }
}
