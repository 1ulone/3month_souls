namespace wine.enemy
{
    public class RangedAttackState : EAttackState
    {
        public RangedAttackState(EnemyBaseController e, EnemyData data) : base(e, data) {}

        public override void Enter()
        {
            //Shoot shit here
            e.onEndAttack = false;
            e.ChangeAnimation("attack");
        }
    }
}
