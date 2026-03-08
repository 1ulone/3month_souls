using UnityEngine;

public class EnemyAnimationTrigger : MonoBehaviour
{
    [SerializeField] protected EnemyBaseController e; 
    private GameObject dodgeWindow; 

    public void TriggerHitbox()
    {
        e.AttackEvent();
    }

    public void OnEndAttack()
    {
        e.onEndAttack = true;
    }

    public void CreateDodgeWindow()
    {
        dodgeWindow = Pool.instances.CreateObject("dodgeWindow", transform.position + new Vector3(0, 0.5f, 0) + (e.transform.forward.normalized), Vector3.zero);
    }

    public void DeleteDodgeWindow()
    {
        if (dodgeWindow == null)
            return;

        Pool.instances.DestroyObject(dodgeWindow);
    }
}
