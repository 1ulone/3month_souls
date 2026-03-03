using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instances;
    public static float hybridTime;

    [SerializeField] private Volume volume;
    [SerializeField] private VolumeProfile defaultVolume;
    [SerializeField] private VolumeProfile slowVolume;

    private float hitstopTime;
    private Coroutine currentCoroutine;
    private bool onSlow;

    private void Awake()
    {
        instances = this;
        hybridTime = Time.deltaTime;
    }

    public void HitStop(float waittime)
    {
        hitstopTime = waittime;
        currentCoroutine = StartCoroutine(HitStopCoroutine());
    }

    public void SlowTime()
    {
        if (onSlow)
            return; 

        // StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(SlowTimeCoroutine());
    }

    private IEnumerator HitStopCoroutine()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(hitstopTime);

        Time.timeScale = 1;
        currentCoroutine = null;
    }

    private IEnumerator SlowTimeCoroutine()
    {
        onSlow = true;
        Time.timeScale = 0.25f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        hybridTime = Time.fixedUnscaledDeltaTime;
        volume.profile = slowVolume;
        yield return new WaitForSecondsRealtime(10f);

        Time.timeScale = 1;
        currentCoroutine = null;
        Time.fixedDeltaTime = 0.02f;
        hybridTime = Time.deltaTime;
        volume.profile = defaultVolume;
        onSlow = false;
    }
}
