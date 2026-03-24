using UnityEngine;
using UnityEngine.AI;

public class KnockbackComponent : MonoBehaviour
{
    private Vector3 impact;
    private Vector3 impactDir;
    private CharacterController controller;
    private NavMeshAgent agent;

    private void Awake() 
    {
        if (TryGetComponent<CharacterController>(out CharacterController c))
            controller = c;
        if (TryGetComponent<NavMeshAgent>(out NavMeshAgent a))
            agent = a;
    }

    public void StartKnock(Vector3 dir, float mass, float force)
    {
        impactDir = dir;
        impactDir.Normalize();
        if (impact.y < 0) 
            impact.y = -impact.y;
        impact += impactDir.normalized * force / mass;
    }

    private void FixedUpdate()
    {
        if (controller != null)
        {
            if (impact.magnitude > 0.2) 
                controller.Move(impact * Time.deltaTime);
        } else 
        if (agent != null)
        {
            if (impact.magnitude > 0.2) 
                agent.Move(impact * Time.deltaTime);
        }
        impact = Vector3.Lerp(impact, Vector3.zero, 5*Time.deltaTime);
    }
}
