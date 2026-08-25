using UnityEngine;

namespace wine.util.component
{
    public class InfiniteRotateComponent : MonoBehaviour
    {
        [SerializeField] private bool unscaledTime = false;

        public Vector3 rotationSpeed = new Vector3(0, 100, 0);
        void Update() { transform.Rotate(rotationSpeed * (unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime)); } 
    }
}
