using UnityEngine;

public class EChaseState : EBaseState
{
    public EChaseState(EnemyBaseController e, EnemyData data) : base(e, data) {}

    public override void Enter()
    {
        e.ChangeFacingDirection(e.GetPlayer().position);
        e.agent.speed = data.chaseSpeed;
        e.ChangeAnimation("walk");
    }

    public override void Exit()
    {
        e.ChangeVelocity(Vector3.zero);
    }

    public override void Logic()
    {
        base.Logic();
        e.ChangeFacingDirection(e.GetPlayer().position);
        if (!data.onChase)
            e.ChangeState(e.Idle);

        if (data.onAttack)
            e.ChangeState(e.Attack);
    }

    public override void FixedLogic()
    {
        if (detectedPlayer != null)
            e.agent.SetDestination(detectedPlayer.position);
    }
}
