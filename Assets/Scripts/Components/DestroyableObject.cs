using UnityEngine;

public class DestroyableObject : MonoBehaviour
{
    [SerializeField] private GameObject brokenObject;
    [SerializeField] private int health = 2;
    [SerializeField] private LayerMask hurtTo;
    
    public int Health { get { return health; } }
    private BulletComponent projectile;
    private KnockbackComponent knockback;

    private void Awake()
    {
        brokenObject.SetActive(false);
        TryGetComponent(out projectile);
        knockback = GetComponent<KnockbackComponent>();
    }

    public void TakeDamage(int i, Vector3 dir, float mass = 0, float force = 0)
    {
        health -= i;

        if (health <= 0)
        {
            brokenObject.SetActive(true);
            brokenObject.transform.SetParent(null);

            if (dir != Vector3.zero)
            {
                Rigidbody[] rbs = brokenObject.GetComponentsInChildren<Rigidbody>();
                Vector3 aforce = dir.normalized * ((force*2) / mass);

                foreach(Rigidbody r in rbs)
                    r.AddForce(aforce, ForceMode.Impulse);
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
