using UnityEngine;
using wine.util.component;

namespace wine.enemy
{
    public class RangedEnemyBaseController : EnemyBaseController 
    {
        [SerializeField] public float aimTime = 2;
        [SerializeField] public float bulletSpeed = 2.5f;
        [SerializeField] public string bulletPrefab;

        protected override void Initialize()
        {
            idle = new EIdleState(this, data);
            patrol = new EPatrolState(this, data);
            chase = new EChaseState(this, data);
            attack = new EAttackState(this, data);
            cooldown = new ECooldownState(this, data);
            dead = new EDeadState(this, data);

            state = idle;
        }

        public override void AttackEvent()
        {
            BulletComponent bc = wine.util.Pool.instances.CreateObject(bulletPrefab, transform.position + (transform.forward * 2), Vector3.zero).GetComponent<BulletComponent>();
            GameObject p = GameObject.FindWithTag("Player").gameObject;
            Vector3 targetDir = p.transform.position - transform.position;

            // hitbox = bc.GetComponent<DamageComponent>();
            // hitbox.knockback = hitbox.GetComponent<KnockbackComponent>();
            // hitbox.enemy = null;

            bc.Move(bulletSpeed, targetDir);
            bc.GetComponent<DamageComponent>().damage = data.damage;
        }
    }
}
