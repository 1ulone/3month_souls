using UnityEngine;

public class TimerOnDestroy : MonoBehaviour
{
    [SerializeField] private float timeToDeath = 5f;
    [SerializeField] private bool usePool = true;
    private float startTime;

    private void OnEnable()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        if (Time.time >= startTime + timeToDeath)
        {
            if (usePool)
                Pool.instances.DestroyObject(this.gameObject);
            else 
                this.gameObject.SetActive(false);
        }
    }
}

/*
using UnityEngine;

public class ParticleOnDestroy : MonoBehaviour
{
    private ParticleSystem system;
    private bool onInit = false;

    private void OnDisable()
    {
        onInit = false;
    }

    private void OnEnable()
    {
        system = GetComponent<ParticleSystem>();
        system.Play();
        onInit = true;
    }

    private void Update()
    {
        if (system.isStopped && onInit)
            Pool.instances.DestroyObject(this.gameObject);
    }
}
*/
