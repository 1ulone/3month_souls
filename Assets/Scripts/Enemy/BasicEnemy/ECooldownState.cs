using UnityEngine;

public class ECooldownState : EBaseState 
{
    public ECooldownState(EnemyBaseController e, EnemyData data) : base(e, data) {}

    public override void Enter()
    {
        timer = Time.time;
        e.ChangeAnimation("idle");
    }

    public override void Logic()
    {
        if (timer + data.cooldownLength < Time.time)
        {
            if (data.onAttack)
                e.ChangeState(e.Attack); else 
            if (data.onChase)
                e.ChangeState(e.Chase);
            else 
                e.ChangeState(e.Patrol);
        }
    }
}
