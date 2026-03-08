using UnityEngine;

public class CallOnDestroy : MonoBehaviour
{
    public void Destroy()
    {
        Pool.instances.DestroyObject(this.gameObject);
    }
}
