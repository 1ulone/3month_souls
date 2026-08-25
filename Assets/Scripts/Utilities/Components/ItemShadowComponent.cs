using UnityEngine;

namespace wine.util.component
{
    public class ItemShadowComponent : MonoBehaviour
    {
        [SerializeField] private GameObject shadowObject;
        [SerializeField] private LayerMask colliderLayer = 6;

        public void DisableShadow()
        {
            shadowObject.SetActive(false);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (shadowObject.activeSelf)
                return;

            if (((1<<other.gameObject.layer) & colliderLayer) != 0)
            {
                if (Physics.Raycast(transform.position, Vector3.down, 1.0f, colliderLayer))
                {
                    shadowObject.SetActive(true);
                    shadowObject.transform.localPosition = transform.up * -0.018f;
                    shadowObject.transform.up = Vector3.up;
                }
            }
        }
    }
}
