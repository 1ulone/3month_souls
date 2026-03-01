using UnityEngine;

public class EDeadState : EBaseState 
{
    public EDeadState(EnemyBaseController e, EnemyData data) : base(e, data) {}

    public override void Enter()
    {
        e.gameObject.SetActive(false);
    }
}
