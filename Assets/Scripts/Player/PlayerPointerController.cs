using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPointerController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float maxDistance = 5;
    [SerializeField] private float followSpeed = 5;
    [SerializeField] private LayerMask floorMask;

    private void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, floorMask))
        {
            Vector3 offset = new Vector3((hit.point.x - player.position.x) / 2, 0, (hit.point.z - player.position.z) / 2);
            transform.position = Vector3.Lerp(transform.position, player.position + Vector3.ClampMagnitude(offset, maxDistance), followSpeed * Time.deltaTime);
        }
    }
}

