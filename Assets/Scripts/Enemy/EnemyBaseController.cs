using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class EnemyBaseController : MonoBehaviour
{
    [SerializeField] protected Animator anim;
    [SerializeField] protected EnemyData data;
    [SerializeField] public Transform groundCheck;
    [SerializeField] protected TextMeshPro info;
    [SerializeField] protected bool drawGizmosDebug = false;

    protected const float hurtTime = 1f;
    protected float startHurtTime;
    protected float maxHealth;

    protected Transform hurtbox;
    protected HitflashComponent hitflash;

    protected EIdleState idle;
    protected EPatrolState patrol;
    protected EChaseState chase;
    protected EAttackState attack;
    protected ECooldownState cooldown;
    protected EDeadState dead;

    public EBaseState state { get; protected set; } 
    public EIdleState Idle { get { return idle; } }
    public EPatrolState Patrol { get { return patrol; } }
    public EChaseState Chase { get { return chase; } }
    public EAttackState Attack { get { return attack; } }
    public ECooldownState Cooldown { get { return cooldown; } }
    public EDeadState Dead { get { return dead; } }

    public NavMeshAgent agent { get; protected set; }

    public int animState { get; protected set; }
    public int prevAnimState { get; protected set; }
    public int FacingDirection { get; protected set; }
    public float health { get; protected set; }
    public bool onEndAttack { get; set; }

    public bool onHurt { get; protected set; }
    public bool canGetHurt { get; set; }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        hitflash = GetComponent<HitflashComponent>();
        data = Instantiate(data);

        maxHealth = data.health;
        health = maxHealth;
        FacingDirection = 1;
        canGetHurt = true;

        Initialize();

        // info.enabled = false;
    }

    protected virtual void Initialize()
    {
        idle = new EIdleState(this, data);
        patrol = new EPatrolState(this, data);
        chase = new EChaseState(this, data);
        attack = new EAttackState(this, data);
        cooldown = new ECooldownState(this, data);
        dead = new EDeadState(this, data);

        state = idle;
    }

    protected virtual void Update()
    {
        if (Time.timeScale == 0)
            return;

        if (onHurt)
        {
            if (Time.time >= startHurtTime + hurtTime)
            {
                if (onEndAttack) { onEndAttack = false; }
                startHurtTime = 0;
                onHurt = false;
                ChangeState(idle);
            }
        }

        state.Logic();
    }
    
    protected virtual void FixedUpdate()
    {
        if (Time.timeScale == 0)
            return;

        state.FixedLogic();
        state.PhysicsLogic();
    }

    public void ChangeState(EBaseState newState)
    {
        if (newState == state)
            return;

        state.Exit();
        state = newState;
        state.Enter();

        info.text = state.ToString().Replace("cone.enemy.","");
    }

    public void ChangeAnimation(string tag)
    {
        if (onHurt)
            return;

        if (!anim.HasState(0, Animator.StringToHash(tag)))
            return;

        animState = Animator.StringToHash(tag);
        if (animState != prevAnimState)
        {
            anim.CrossFade(animState, 0, 0);
            prevAnimState = animState;
        }

    }

    public void ChangeVelocity(Vector3 v)
    {
        if (v == Vector3.zero)
            agent.ResetPath();
        agent.velocity = v;
    }

    public void ChangeFacingDirection(Vector3 target)
    {
        transform.LookAt(target);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (((1<<other.gameObject.layer) & data.hurtForEnemy) != 0)
        {
            if (other.TryGetComponent<DamageComponent>(out DamageComponent d))
                GetHurt(d.damage);
        }
    }

    public void GetHurt(int damage)
    {
        if (!canGetHurt)
            return;

        if (onHurt)
            return;

        ChangeAnimation("hurt");
        onHurt = true;
        health -= damage;
        startHurtTime = Time.time;

        StartCoroutine(hitflash.FlashesCoroutine());
        TimeManager.instances.HitStop(0.075f);
    }

    public virtual void AttackEvent() 
    {
        Transform vfx = Pool.instances.CreateObject("enemyAttack",transform.position + new Vector3(0, 0.5f, 0) + (transform.forward.normalized), Vector3.zero).transform;
        vfx.LookAt(transform.forward);
        vfx.GetComponent<DamageComponent>().damage = data.damage;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmosDebug)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.chaseRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.attackRadius);
    }

    public Transform GetPlayer()
    {
        return FindFirstObjectByType<PlayerController>().gameObject.transform;
    }
}

//NOTE: 
//add screenshake
//add blood explosion for enemy death
//add blood splatter for hurt (vfx)
