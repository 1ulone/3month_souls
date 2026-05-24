using UnityEngine;

public class EnemyAnimationTrigger : MonoBehaviour
{
    [SerializeField] protected EnemyBaseController e; 
    [SerializeField] protected GameObject parryHint;

    private void Awake()
    {
        parryHint.SetActive(false);
    }

    public void TriggerHitbox()
    {
        e.AttackEvent();
    }

    public void OnEndAttack()
    {
        e.onEndAttack = true;
    }

    public void CreateParryWindow()
    {
        if (e.hitbox == null)
            return;

        e.hitbox.parryAble = true; 
        e.canBeHurt = false;
    }

    public void DeleteParryWindow()
    {
        if (e.hitbox == null)
            return;

        e.hitbox.parryAble = false; 
        e.canBeHurt = true;
    }
}
