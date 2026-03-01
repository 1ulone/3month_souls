using UnityEngine;

public class EPatrolState : EBaseState 
{
    protected float walkpointWaittime = 0.2f;
    protected float walkpointTime;
    protected bool setWalkPoint;
    protected Vector3 walkPoint;

    public EPatrolState(EnemyBaseController e, EnemyData data) : base(e, data) {}

    public override void Enter()
    {
        timer = Time.time;
        e.ChangeAnimation("walk");
        e.agent.speed = data.movementSpeed;
        setWalkPoint = false;
    }

    public override void Exit()
    {
        e.ChangeVelocity(Vector3.zero);
    }

    public override void Logic()
    {
        base.Logic();
        if (timer + data.patrolLength < Time.time)
            e.ChangeState(e.Idle);

        if (data.onChase)
            e.ChangeState(e.Chase);

        if (!setWalkPoint)  
        {
            if (walkpointTime + walkpointWaittime < Time.time)
                SetNextWalkPoint();  
        }

        Vector3 distanceWalktoPoint = e.transform.position - walkPoint;

        if (distanceWalktoPoint.magnitude < 1f || data.onWall)
        {
            if (setWalkPoint)
            {
                e.ChangeAnimation("idle");
                walkpointTime = Time.time;
            }

            setWalkPoint = false;
            e.ChangeVelocity(Vector3.zero);
        }
    }

    public override void FixedLogic()
    {
        if (setWalkPoint) 
            e.agent.SetDestination(walkPoint);
    }

    protected virtual void SetNextWalkPoint()
    {
        float rZ = Random.Range(-data.walkRange, data.walkRange);
        float rX = Random.Range(-data.walkRange, data.walkRange);

        Vector3 potentialWalkPoint = new Vector3(e.transform.position.x + rX, e.transform.position.y, e.transform.position.z + rZ);

        Vector3 direction = potentialWalkPoint - e.transform.position;
        float distance = direction.magnitude;

        if (Physics.Raycast(e.transform.position, direction.normalized, distance, data.collider))
        {
            return;
        }

        if (Physics.Raycast(potentialWalkPoint, -e.transform.up, 2f, data.collider))
        {
            walkPoint = potentialWalkPoint;
            e.ChangeAnimation("walk");
            walkpointTime = 0;
            setWalkPoint = true;
        }
    }
}
