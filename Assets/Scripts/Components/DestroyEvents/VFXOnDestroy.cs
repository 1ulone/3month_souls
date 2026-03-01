using UnityEngine;
using UnityEngine.VFX;

public class VFXOnDestroy : MonoBehaviour
{
    [SerializeField] private float timeToDeath = 5f;
    private float startTime;

    private void OnEnable()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        if (Time.time >= startTime + timeToDeath)
            Pool.instances.DestroyObject(this.gameObject);
    }
}
