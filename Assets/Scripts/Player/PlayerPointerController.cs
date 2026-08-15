using UnityEngine;

namespace wine.player
{
    public class PlayerPointerController : MonoBehaviour
    {
        [SerializeField] private RectTransform renderTextureUI;
        [SerializeField] private Transform player;
        [SerializeField] private LayerMask floorMask;
        [SerializeField] private float maxDistance = 5;
        [SerializeField] private float followSpeed = 5;
        private Vector3 npos;

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }

        private void Update()
        {
            Vector2 mousePos = wine.util.InputController.instances.RawMouse();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            Plane playerPlane = new Plane(Vector3.up, player.transform.position);
            if (playerPlane.Raycast(ray, out float dist))
                npos = ray.GetPoint(dist);

            // transform.position = Vector3.Lerp(transform.position, player.position + Vector3.ClampMagnitude(npos - player.position, maxDistance), followSpeed * Time.deltaTime * 10.0f);
            transform.position = Vector3.Lerp(transform.position, npos, followSpeed * Time.deltaTime * 10.0f);
        }
    }
}
