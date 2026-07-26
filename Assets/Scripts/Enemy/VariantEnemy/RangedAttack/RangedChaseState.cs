using UnityEngine;

namespace wine.enemy
{
    public class RangedChaseState : EChaseState 
    {
        protected RangedEnemyBaseController ranged;
        public RangedChaseState(EnemyBaseController e, EnemyData data, RangedEnemyBaseController ranged) : base(e, data) 
        {
            this.ranged = ranged;
        }

        public override void Enter()
        {
            e.ChangeFacingDirection(e.GetPlayer().position);
            e.agent.speed = 0;
            timer = Time.time;
            e.ChangeAnimation("walk");
        }

        public override void Logic()
        {
            if (Time.time > timer + ranged.aimTime)
            {
                e.ChangeState(e.Attack);
            }
        }

        public override void FixedLogic() {}
    }
}
