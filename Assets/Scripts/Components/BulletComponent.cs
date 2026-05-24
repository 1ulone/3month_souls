using UnityEngine;
using System;

public class BulletComponent : MonoBehaviour
{
    private float speed, lastSpeed, delayTime;
    private Vector3 direction, lastVelocity;
    private Rigidbody rb;
    private Action onCollideEvent; 

    public float health { get; set; }
    public float Speed { get { return speed; } }

    [SerializeField] public bool doRotate = false;
    [SerializeField] public bool destroyOnCollide = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (doRotate)
            transform.Rotate((rb.linearVelocity * 10f) * Time.deltaTime);

        lastVelocity = rb.linearVelocity;
        if (speed == 0)
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.deltaTime * 1.5f);
            return;
        }  

        rb.linearVelocity = speed * direction.normalized * (Time.deltaTime * 100);
    }

    public void OnCollisionEnter(Collision other)
    {
        if (speed == 0)
            return;

        health --;
        if (health <= 0)
        {
            Pool.instances.DestroyObject(this.gameObject); 
        } else 
        if (health > 0)
        {
            Invoke("delayedCollideEvent", delayTime);
            speed = 0;

            Vector3 surfaceNormal = other.contacts[0].normal;
            Vector3 bounceDirection = Vector3.Reflect(lastVelocity.normalized, surfaceNormal);
            Vector3 randomizedBounce = bounceDirection + UnityEngine.Random.insideUnitSphere * 2f;

            if (Vector3.Dot(randomizedBounce, surfaceNormal) <= 0)
                randomizedBounce = bounceDirection;

            rb.linearVelocity = new Vector3(randomizedBounce.x, 0, randomizedBounce.z).normalized * (lastSpeed*1.5f);
            rb.useGravity = true;
        }
    }

    private void delayedCollideEvent()
        => onCollideEvent?.Invoke();

    public void Move(float speed, Vector3 direction, Action onCollideEvent = null, float delayTime = 0)
    {   
        this.speed = speed;
        this.direction = direction;
        this.onCollideEvent = onCollideEvent;
        this.delayTime = delayTime;

        lastSpeed = speed;
    }
}
