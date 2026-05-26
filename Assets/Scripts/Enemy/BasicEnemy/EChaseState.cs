using UnityEngine;

public class EChaseState : EBaseState
{
    protected bool isInspecting;
    protected float randomTimeAdder;
    protected float desiredDistance; 
    protected int circDirection;

    public EChaseState(EnemyBaseController e, EnemyData data) : base(e, data) {}

    public override void Enter()
    {
        e.ChangeFacingDirection(e.GetPlayer().position);
        e.agent.speed = data.chaseSpeed;
        e.ChangeAnimation("walk");

        isInspecting = true;
        desiredDistance = data.chaseStepoff;
        randomTimeAdder = Random.Range(0, 0.15f);
        circDirection = Random.value > 0.5f ? 1 : -1;
        timer = Time.time;
    }

    public override void Exit()
    {
        e.ChangeVelocity(Vector3.zero);
    }

    public override void Logic()
    {
        base.Logic();
        e.ChangeFacingDirection(e.GetPlayer().position);

        if (Time.time >= timer + data.chaseInspect + randomTimeAdder)
        {
            isInspecting = !isInspecting;
            randomTimeAdder = Random.Range(0, 0.15f);
            circDirection = Random.value > 0.5f ? 1 : -1;
            timer = Time.time;

            e.agent.speed = isInspecting ? data.chaseSpeed / 2 : data.chaseSpeed;
        }

        if (!isInspecting)
            desiredDistance = Mathf.Lerp(desiredDistance, 0, data.chaseSpeed/2 * Time.deltaTime);
        else 
            desiredDistance = data.chaseStepoff;
        
        if (!data.onChase)
            e.ChangeState(e.Idle);

        if (data.onAttack)
            e.ChangeState(e.Attack);
    }

    public override void FixedLogic()
    {
        if (detectedPlayer == null)
            return;

        float circleStep = 1.5f;

        Vector3 target = new Vector3(detectedPlayer.position.x, e.transform.position.y, detectedPlayer.position.z);

        Vector3 targetDir = (e.transform.position - target).normalized; 
        Vector3 tangent = Vector3.Cross(targetDir, Vector3.up).normalized;
        Vector3 idealPosition = target + (targetDir * desiredDistance);

        e.agent.SetDestination(idealPosition + (tangent * circleStep * circDirection));
    }
}
