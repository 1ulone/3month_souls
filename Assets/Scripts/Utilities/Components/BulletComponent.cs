using UnityEngine;
using System;

namespace wine.util.component
{
    public class BulletComponent : MonoBehaviour
    {
        private float speed, delayTime, lastSpeed;
        private bool afterBounce, canBounce;
        private Vector3 direction, lastVelocity;
        private Rigidbody rb;
        private Action onCollideEvent; 
        private DestroyableObject destroyableComponent;

        public float Speed { get { return speed; } }

        [SerializeField] public bool doRotate = false;
        [SerializeField] public bool destroyOnCollide = true;
        [SerializeField] public float bounceMultiplier = 5.0f;
        [SerializeField] private float bounceWindowTimer = 0.25f;
        [SerializeField] private float rotationSpeed = 2.5f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            TryGetComponent(out destroyableComponent);
        }

        private void FixedUpdate()
        {
            if (doRotate)
                transform.Rotate(new Vector3(10.0f * rotationSpeed * Time.deltaTime, 0, 0));

            lastVelocity = rb.linearVelocity;
            if (speed == 0 || afterBounce)
            {
                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.deltaTime * 1.5f);
                return;
            }  

            rb.linearVelocity = speed * direction.normalized * (Time.deltaTime * 100);
        }

        public void OnCollisionEnter(Collision other)
        {
            if (!canBounce)
                return;

            if (rb.useGravity)
                return; 

            if (afterBounce && speed == 0)
                speed = lastSpeed;

            if (speed == 0)
                return;

            speed = 0;
            rb.linearVelocity = Vector3.zero;
            destroyableComponent.TakeDamage(1, Vector3.zero);
            Invoke("delayedCollideEvent", delayTime);

            Vector3 surfaceNormal = other.contacts[0].normal;
            Vector3 reflectDirection = Vector3.Reflect(lastVelocity.normalized, surfaceNormal);

            rb.linearVelocity = new Vector3(reflectDirection.x, 0, reflectDirection.z).normalized * lastSpeed * bounceMultiplier;
            afterBounce = true;

            Invoke("removeBounceWindow", bounceWindowTimer);

            // NOTE: add "stucked state" on high durability weapon
            // add bounce only if durability is < than a threshold (1/3 of max durability)
        }

        private void removeBounceWindow()
        {
            rb.useGravity = true;
            rb.freezeRotation = true;
            doRotate = false;
        }

        private void delayedCollideEvent()
            => onCollideEvent?.Invoke();

        private void enableBounce()
            => canBounce = true;

        public void Move(float speed, Vector3 direction, Action onCollideEvent = null, float delayTime = 0, bool isBounceable = false)
        {   
            if (isBounceable)
            {
                canBounce = false;
                Invoke("enableBounce", 0.05f);

                rb.freezeRotation = false;
                doRotate = true;
                afterBounce = false;
            }

            this.speed = speed;
            this.direction = direction;
            this.onCollideEvent = onCollideEvent;
            this.delayTime = delayTime;

            lastSpeed = speed;
        }
    }
}
