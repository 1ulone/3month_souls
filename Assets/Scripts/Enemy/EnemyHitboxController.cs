using UnityEngine;

public class EnemyHitboxController : MonoBehaviour
{
    [SerializeField] private EnemyBaseController enemy;
    [SerializeField] private KnockbackComponent knockback;

    public void ParryCallback(Vector3 pos, int damage)
    {
        Vector3 dir = transform.position - pos;
        knockback.StartKnock(dir, enemy.Data.mass, enemy.Data.force/2);
        enemy.GetHurt(damage*2, true);
    }
}
