using UnityEngine;

namespace wine.player 
{
    public class PlayerThrowTrajectoryHint : MonoBehaviour
    {
        [SerializeField] public LineRenderer hint;
        [SerializeField] private LayerMask colMask;
        [SerializeField] private float maxDistance = 25.0f;
        [SerializeField] private float hitOffset = 0.1f;
        [SerializeField] private float yLevel = 0.1f;
        private PlayerController controller;

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
        }

        private void FixedUpdate()
        {
            if (!hint.enabled)
                return;

            hint.positionCount = 1;
            hint.SetPosition(0, new Vector3(transform.position.x, yLevel, transform.position.z));

            Vector3 currPos = transform.position;
            Vector3 currDir = (controller.Pointer.position - transform.position).normalized;

            for (int i = 0; i < 3; i++)
            {
                if (Physics.Raycast(currPos, currDir, out RaycastHit hit, maxDistance, colMask))
                {
                    hint.positionCount++;
                    hint.SetPosition(hint.positionCount - 1, new Vector3(hit.point.x, yLevel, hit.point.z));

                    currDir = Vector3.Reflect(currDir, hit.normal);
                    currPos = hit.point + currDir * hitOffset;
                } else {
                    Vector3 nextPos = currPos + currDir * maxDistance;

                    hint.positionCount++;
                    hint.SetPosition(hint.positionCount - 1, new Vector3(nextPos.x, yLevel, nextPos.z));
                }
            }
        }
    }
}
