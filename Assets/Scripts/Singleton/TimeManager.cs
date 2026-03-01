using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instances;

    private float hitstopTime;

    private void Awake()
    {
        instances = this;
    }

    public void HitStop(float waittime)
    {
        hitstopTime = waittime;
        StartCoroutine(HitStopCoroutine());
    }

    private IEnumerator HitStopCoroutine()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(hitstopTime);

        Time.timeScale = 1;
    }
}
