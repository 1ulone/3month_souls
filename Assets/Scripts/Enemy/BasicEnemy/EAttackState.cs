using UnityEngine;

public class EAttackState : EBaseState 
{
    public EAttackState(EnemyBaseController e, EnemyData data) : base(e, data) {}

    public override void Enter()
    {
        e.ChangeVelocity(Vector3.zero);
        e.ChangeFacingDirection(e.GetPlayer().position);
        e.onEndAttack = false;
        e.ChangeAnimation("attack");
		// e.AttackEvent();
    }

    public override void Exit()
    {
        e.ChangeVelocity(Vector3.zero);
        e.onEndAttack = false;
    }

    public override void Logic()
    {
        base.Logic();
        if (e.onEndAttack)
            e.ChangeState(e.Cooldown);
    }

}
