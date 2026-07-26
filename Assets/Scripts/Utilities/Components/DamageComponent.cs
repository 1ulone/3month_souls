using UnityEngine;

namespace wine.util.component
{
    public class DamageComponent : MonoBehaviour
    {
        public int damage { get; set; }
        public bool parryAble { get; set; }
        public bool destroyOnEnd = false;

        private BulletComponent projectile;
        private KnockbackComponent knockback;
        // private EnemyBaseController enemy;

        private void Awake()
        {
            damage = 5;

            TryGetComponent(out projectile);

            if (projectile != null)
                parryAble = true;
        }

        // public void InitHitbox(int damage, KnockbackComponent knockback = null, EnemyBaseController enemy = null)
        public void InitHitbox(int damage, KnockbackComponent knockback = null)
        {
            this.damage = damage;
            this.knockback = knockback;
            // this.enemy = enemy;
        }

        public void ParryCallback(Vector3 pos, int damage)
        {
            Vector3 dir = transform.position - pos;
            if (projectile == null)
            {
                // knockback.StartKnock(dir, enemy.Data.mass, enemy.Data.force/2);
                // enemy.GetHurt(damage*2, true);
            } else {
                projectile.Move(projectile.Speed, dir);
                projectile.gameObject.layer = 11;
            }
        }
    }
}
