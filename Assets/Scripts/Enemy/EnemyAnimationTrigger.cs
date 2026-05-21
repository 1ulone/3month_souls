using UnityEngine;

public class EnemyAnimationTrigger : MonoBehaviour
{
    [SerializeField] protected EnemyBaseController e; 
    [SerializeField] protected GameObject parryHint;
    [SerializeField] private GameObject dodgeWindow; 

    private void Awake()
    {
        parryHint.SetActive(false);
        dodgeWindow.SetActive(false);
    }

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
        // dodgeWindow = Pool.instances.CreateObject("dodgeWindow", transform.position + new Vector3(0, 0.5f, 0) + (e.transform.forward.normalized), Vector3.zero);
        dodgeWindow.SetActive(true);
        dodgeWindow.transform.position = transform.position + new Vector3(0, 0.5f, 0) + e.transform.forward.normalized;
        // parryHint.SetActive(true);
        e.canBeHurt = false;
    }

    public void DeleteDodgeWindow()
    {
        if (dodgeWindow == null)
            return;

        e.canBeHurt = true;
        parryHint.SetActive(false);
        dodgeWindow.SetActive(false);
        // Pool.instances.DestroyObject(dodgeWindow);
    }
}
