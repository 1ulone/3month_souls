using System.Collections;
using UnityEngine;

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
    [SerializeField] private Transform targetMesh;
    [SerializeField] private Transform pointer;
    [SerializeField] private Animator anim;
    [SerializeField] private PlayerUI ui;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask invincible;

    public Transform Pointer { get { return pointer; } }
    
    private InputController input;
    private CharacterController controller;
    private HitflashComponent hitflash;

    private string state;
    private float startTime;
    private bool canRoll, canAttack, canBeHurt;
    private int health;

    private Coroutine currentCoroutine;
    private Vector2 dir;
    private Vector3 rollingDir;
    private Vector3 lookRotation;
    private LayerMask defaultLayer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<InputController>();
        hitflash = GetComponent<HitflashComponent>();

        defaultLayer = this.gameObject.layer;
        health = maxHealth;
        ui.UpdateHealthUI(health, maxHealth);
        
        state = "";

        canRoll = true;
        canAttack = true;
        canBeHurt = true;

        ChangeAnim("idle");
    }

    private void Update()
    {
        if (!canBeHurt)
        {
            if (startTime + hurtCooldown < Time.time)
                EndHurt();
        }

        if (Time.timeScale == 0)
            return;

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
            targetMesh.rotation = Quaternion.RotateTowards(targetMesh.rotation, trot, rotationSpeed * Time.deltaTime * 50);
        } else 
        if (dir == Vector2.zero)
        {
            ChangeAnim("idle");
        }
    }

    private void FixedUpdate()
    {
        if (Time.timeScale == 0)
            return;

        if (state == "roll")
        {
            controller.Move(rollingDir.normalized * dashSpeed * Time.deltaTime);
            return; 
        }

        if (state == "attack")
            return;

        Vector3 move = transform.right * dir.x + transform.forward * dir.y;
        controller.Move(move * movementSpeed * Time.deltaTime);
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
        this.gameObject.layer = invincible;

        rollingDir = targetMesh.forward;
        if (dir != Vector2.zero)
        {
            rollingDir = lookRotation;
            targetMesh.rotation = Quaternion.LookRotation(lookRotation, Vector3.up);
        }

        ChangeAnim("roll");
        startTime = Time.time;

        while(startTime + dashTime > Time.time)
            yield return null;

        this.gameObject.layer = defaultLayer;
        ChangeAnim("idle");

        yield return new WaitForSeconds(dashCooldown);
        canRoll = true;
        canAttack = true;
        currentCoroutine = null;
    }

    private IEnumerator Attack()
    {
        canAttack = false;
        targetMesh.LookAt(new Vector3(pointer.position.x, 0, pointer.position.z));
        ChangeAnim("attack");
        startTime = Time.time;

        while(startTime + attackTime > Time.time)
            yield return null;

        ChangeAnim("idle");
        
        yield return new WaitForSeconds(attackCooldown);
        canRoll = true;
        canAttack = true;
        currentCoroutine = null;
    }

    private void GetHurt(int damage)
    {
        if (state == "roll")
            return;

        startTime = Time.time;
        health -= damage;
        canBeHurt = false;

        ui.UpdateHealthUI(health, maxHealth);
    
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
        // Debug.Log("ay");

        if (((1<<other.gameObject.layer) & enemyLayer) != 0)
        {
            if (other.TryGetComponent<DamageComponent>(out DamageComponent d))
                GetHurt(d.damage);
        }
    }
}
