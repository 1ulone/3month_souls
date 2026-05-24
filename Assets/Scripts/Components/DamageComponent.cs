using UnityEngine;

public class DamageComponent : MonoBehaviour
{
    public int damage { get; set; }
    public bool parryAble { get; set; }
    public bool destroyOnEnd = false;

    public KnockbackComponent knockback { get; set; }
    public EnemyBaseController enemy { get; set; }
    private BulletComponent projectile;

    private void Awake()
    {
        damage = 1;
        TryGetComponent(out projectile);
        if (projectile != null)
            parryAble = true;
    }

    public void ParryCallback(Vector3 pos, int damage)
    {
        Vector3 dir = transform.position - pos;
        if (projectile == null)
        {
            knockback.StartKnock(dir, enemy.Data.mass, enemy.Data.force/2);
            enemy.GetHurt(damage*2, true);
        } else {
            projectile.Move(projectile.Speed, dir);
            projectile.gameObject.layer = 11;
        }
    }
}
