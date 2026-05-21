using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPointerController : MonoBehaviour
{
    [SerializeField] private RectTransform renderTextureUI;

    [SerializeField] private Transform player;
    [SerializeField] private float maxDistance = 5;
    [SerializeField] private float followSpeed = 5;
    [SerializeField] private LayerMask floorMask;

    private void FixedUpdate()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(renderTextureUI, mousePos, null, out Vector2 localPoint))
        {
            Rect rect = renderTextureUI.rect;

            // Convert local point to Viewport coordinates (0 to 1)
            float viewportX = (localPoint.x - rect.x) / rect.width;
            float viewportY = (localPoint.y - rect.y) / rect.height;
            Vector2 viewportPos = new Vector2(viewportX, viewportY);

            Ray ray = Camera.main.ViewportPointToRay(viewportPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f, floorMask))
            {
                Vector3 offset = new Vector3((hit.point.x - player.position.x) / 2, 0, (hit.point.z - player.position.z) / 2);
                transform.position = Vector3.Lerp(transform.position, player.position + Vector3.ClampMagnitude(offset, maxDistance), followSpeed * Time.fixedUnscaledDeltaTime);
            }
        }   }
}

