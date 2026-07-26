using UnityEngine;
using UnityEngine.AI;

namespace wine.util.component
{
    public class KnockbackComponent : MonoBehaviour
    {
        private Vector3 impact;
        private CharacterController controller;
        private NavMeshAgent agent;
        private Rigidbody rb;

        public Vector3 word { get { return impact; }}

        private float oSpeed, oAcc;
        private bool stopKnock;

        private void Start() 
        {
            TryGetComponent(out controller);
            TryGetComponent(out agent);
            if (agent == null)
                TryGetComponent(out rb);
        }

        public void StartKnock(Vector3 dir, float mass, float force)
        {
            stopKnock = false;
            dir.Normalize();
            if (dir.y < 0) 
                dir.y = -dir.y;

            Vector3 aforce = dir.normalized * (force / mass);

            if (rb != null)
                rb.AddForce(aforce, ForceMode.Impulse);
            else 
                impact += aforce;

            if (agent != null)
                agent.updateRotation = false;
        }

        public void ForceStopKnock()
        {
            impact = Vector3.zero;
            stopKnock = true;

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (agent != null)
            {
                agent.updateRotation = true;
                if (agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                }
            }
        }

        private void FixedUpdate()
        {
            if (stopKnock)
                return;

            if (rb != null)
                return;

            if (impact.magnitude > 0.2) 
            {
                if (controller != null)
                    controller.Move(impact * Time.deltaTime);
                else if (agent != null)
                    agent.Move(impact * Time.deltaTime);

                impact = Vector3.Lerp(impact, Vector3.zero, 5f*Time.deltaTime);
            } else 
                if (impact != Vector3.zero) 
                {
                    impact = Vector3.zero;
                    if (agent != null)
                        agent.updateRotation = true;
                }
        }
    }
}
