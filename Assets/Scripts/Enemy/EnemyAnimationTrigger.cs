using UnityEngine;

public class EnemyAnimationTrigger : MonoBehaviour
{
    [SerializeField] protected EnemyBaseController e; 

    public void TriggerHitbox()
    {
        e.AttackEvent();
    }

    public void OnEndAttack()
    {
        e.onEndAttack = true;
    }
}
