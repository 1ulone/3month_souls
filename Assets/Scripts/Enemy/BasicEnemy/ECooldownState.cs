using UnityEngine;

public class ECooldownState : EBaseState 
{
    protected int circDirection;

    public ECooldownState(EnemyBaseController e, EnemyData data) : base(e, data) {}

    public override void Enter()
    {
        timer = Time.time;
        e.agent.speed = data.movementSpeed;
        e.ChangeAnimation("walk");
        circDirection = Random.value > 0.5f ? 1 : -1;
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

    public override void FixedLogic()
    {
        if (detectedPlayer == null)
            return;

        float circleStep = 1.5f;

        Vector3 target = new Vector3(detectedPlayer.position.x, e.transform.position.y, detectedPlayer.position.z);

        Vector3 targetDir = (e.transform.position - target).normalized; 
        Vector3 tangent = Vector3.Cross(targetDir, Vector3.up).normalized;
        Vector3 idealPosition = target + (targetDir * data.cooldownStepoff);

        e.agent.SetDestination(idealPosition + (tangent * circleStep * circDirection));

    }
}
