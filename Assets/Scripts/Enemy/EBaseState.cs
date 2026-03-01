using UnityEngine;

public class EBaseState 
{
    protected EnemyBaseController e;
    protected EnemyData data;
    protected float timer;
    protected Transform detectedPlayer;

    public EBaseState(EnemyBaseController e, EnemyData data)
    {
        this.e = e;
        this.data = data;
    }

    public virtual void Enter() {}
    public virtual void Exit() {}

    public virtual void Logic() 
    {
        if (e.health <= 0)
            e.ChangeState(e.Dead);
    }

    public virtual void FixedLogic() {}

    public virtual void PhysicsLogic() 
    {
        data.onGround = Physics.CheckBox(e.groundCheck.position, new Vector3(1, 1, 1) / 2f, Quaternion.identity, data.collider);
        data.onAttack = Physics.CheckSphere(e.transform.position, data.attackRadius, data.player);
        data.onWall = Physics.Raycast(e.transform.position, e.transform.forward, 2f, data.collider);

        if (!data.checkChase)
        {
            data.onChase = false;
            return;
        }

        Collider[] ps = Physics.OverlapSphere(e.transform.position, data.chaseRadius, data.player);
        Collider p = ps.Length > 0 ? ps[0] : null;
        if (p != null)
        {
            float distance = Mathf.Abs(Vector3.Distance(e.transform.position, p.transform.position));
            Vector3 dir = p.transform.position - e.transform.position;
            RaycastHit[] hits = Physics.RaycastAll(e.transform.position, new Vector3(dir.x, 0.5f, dir.z), distance);

            // Debug.DrawRay(e.transform.position, new Vector3(dir.x, 0.5f, dir.z), Color.red);
            foreach(RaycastHit hit in hits)
            {
                if ((data.collider & (1 << hit.collider.gameObject.layer)) != 0)
                { 
                    data.onChase = false; 
                    detectedPlayer = null;
                    break;
                } else 
                {
                    data.onChase = true;
                    detectedPlayer = p.transform;
                }
            }
        }
        else 
        {
            detectedPlayer = null;
            data.onChase = false;
        }

    }
}
