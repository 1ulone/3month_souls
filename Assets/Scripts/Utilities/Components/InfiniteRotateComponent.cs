using UnityEngine;

namespace wine.util.component
{
    public class InfiniteRotateComponent : MonoBehaviour
    {
        public Vector3 rotationSpeed = new Vector3(0, 100, 0);
        void Update() { transform.Rotate(rotationSpeed * Time.deltaTime); } 
    }
}
