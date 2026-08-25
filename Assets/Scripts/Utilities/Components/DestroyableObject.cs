using UnityEngine;

namespace wine.util.component
{
    public class DestroyableObject : MonoBehaviour
    {
        [SerializeField] private GameObject brokenObject;
        [SerializeField] private int health = 2;
        [SerializeField] private LayerMask hurtTo;
        [SerializeField] private float explosionForce = 1.5f;

        public int Health { get { return health; } }
        private BulletComponent projectile;
        private KnockbackComponent knockback;

        private void Awake()
        {
            brokenObject.SetActive(false);
            TryGetComponent(out projectile);
            knockback = GetComponent<KnockbackComponent>();
        }

        public void TakeDamage(int i, Vector3 dir, float mass = 1.0f, float force = 1.0f)
        {
            health -= i;
            force = explosionForce;

            if (health <= 0)
            {
                brokenObject.SetActive(true);
                brokenObject.transform.SetParent(null);

                if (dir != Vector3.zero)
                {
                    Rigidbody[] rbs = brokenObject.GetComponentsInChildren<Rigidbody>();

                    foreach(Rigidbody r in rbs)
                    {
                        Vector3 noise = Random.insideUnitSphere * 15.0f;
                        Vector3 ndir = dir.normalized + new Vector3(noise.x, noise.y/4.0f, noise.z);
                        Vector3 aforce = noise.normalized * (force / r.mass);
                        r.AddForce(aforce, ForceMode.Impulse);
                    }
                }

                Pool.instances.DestroyObject(this.gameObject);
            } else
            if (health > 0)
            {
                if (dir == Vector3.zero)
                    return;

                knockback.StartKnock(dir, mass, force);
            }
        } 
    }
}
