using System.Collections;
using UnityEngine;
using wine.util;

namespace wine.player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 5;
        [SerializeField] private float crossFadeTime = 1.25f;
        [SerializeField] private float dashTime = 1f;
        [SerializeField] private float dashSpeed = 10f;
        [SerializeField] private float dashCooldown = 1.5f;
        [SerializeField] private float comboWindow = 0.125f;
        [SerializeField] private Transform targetMesh;
        [SerializeField] private Transform pointer;
        [SerializeField] private Animator anim;
        [SerializeField] private LayerMask parryWindow;

        public Transform Pointer { get { return pointer; } }
        public Transform ActualMesh { get { return targetMesh; } }
        public bool onThrow { get; set; }
        public bool canBeHurt { get; set; }
        public bool onTransition { get; set; }
        public string state { get; set; }
        public string weapon { get; set; }

        private CharacterController controller;
        private InputController input;
        private PlayerStats stats;

        private float startTime;
        private bool canRoll, canAttack, canStillCombo;
        private int comboIndex;

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
            // shakeSource = GetComponent<CinemachineImpulseSource>();

            defaultLayer = LayerMask.NameToLayer("Player");
            invincible = LayerMask.NameToLayer("Invincible");

            state = "";
            weapon = "unarmed";

            canRoll = true;
            canAttack = true;
            canBeHurt = true;

            comboIndex = 0;
            ChangeAnim("idle");
        }

        private void Start()
        {
            stats = PlayerStats.instances;
        }

        private void Update()
        {
            if (onTransition)
            {
                dir = Vector2.zero;
                ChangeAnim("idle");
                return;
            }

            if (!canBeHurt)
            {
                dir = Vector2.zero;
                ChangeAnim("idle"); // TODO: change to hurt later
                if (startTime + stats.downtime/2 < Time.time)
                    EndHurt();

                return;
            }

            if (canStillCombo)
            {
                if (startTime + comboWindow < Time.time)
                {
                    canStillCombo = false;
                    comboIndex = 0;
                }
            }

            if (Time.timeScale == 0 || !controller.enabled)
                return;

            dir = input.Move();
            lookRotation = new Vector3(dir.x, 0, dir.y);

            if (onThrow)
                return;

            if (input.GetInput("roll") && canRoll) 
            {
                if (currentCoroutine != null && state != "roll")
                    StopCoroutine(currentCoroutine);
                currentCoroutine = StartCoroutine(Roll());
            }

            if (state == "roll")
                return; 

            if (input.GetInput("attack") && canAttack)
                Attack();

            if (state.Contains("attack"))
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
            if (onTransition)
                return;

            if (Time.timeScale == 0 || !controller.enabled)
                return;

            if (state == "roll")
            {
                controller.Move(new Vector3(rollingDir.normalized.x, -98.1f * Time.deltaTime, rollingDir.normalized.z) * (dashSpeed + stats.rollspeed) * Time.fixedUnscaledDeltaTime);
                return; 
            }

            if (state.Contains("attack"))
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
            comboIndex = 0;
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

        private void Attack()
        {
            startTime = 0;
            canAttack = false;
            canStillCombo = false;
            // targetMesh.LookAt(new Vector3(pointer.position.x, targetMesh.position.y, pointer.position.z));
            LookAtPointer();

            string attackTag = "attack_"+weapon+"_"+comboIndex;
            ChangeAnim(attackTag);
        }

        public void AttackComboAddon()
        {
            canAttack = true;

            if (comboIndex+1 > 1)
            {
                comboIndex = 0;
                return;
            }

            comboIndex++;
            canStillCombo = true;
            startTime = Time.time;
        }

        public void LookAtPointer()
        {
            Vector3 newPoint = new Vector3(pointer.position.x, 0, pointer.position.z);
            ActualMesh.LookAt(newPoint);
            ActualMesh.rotation = Quaternion.Euler(0, ActualMesh.eulerAngles.y, 0);
        }

        public void OnEndAttack()
        {
            ChangeAnim("idle");
            canRoll = true;
            canAttack = true;
        }

        private void EndHurt()
        {
            startTime = 0;
            canBeHurt = true;
            canAttack = true;
            canRoll = true;
        }

        private void ReenableController()
        {
            controller.enabled = true; 
            controller.Move(Vector3.zero);
        }

    }
}
