using UnityEngine;

namespace wine.util
{
    public class CallOnDestroy : MonoBehaviour
    {
        public void Destroy()
        {
            Pool.instances.DestroyObject(this.gameObject);
        }
    }
}
