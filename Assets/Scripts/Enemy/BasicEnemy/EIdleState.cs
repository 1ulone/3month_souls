using UnityEngine;

public class EIdleState : EBaseState 
{
    public EIdleState(EnemyBaseController e, EnemyData data) : base(e, data) {}

    public override void Enter() 
    {
        timer = Time.time;
        e.ChangeVelocity(Vector3.zero);
        e.ChangeAnimation("idle");
    }

    public override void Logic()
    {
        base.Logic();
        if (timer + data.idleLength < Time.time)
            e.ChangeState(e.Patrol);
        if (data.onChase)
            e.ChangeState(e.Chase);
    }
}
