using System;
using UnityEngine;
using System.Collections;

public class FadeTransitionUI : MonoBehaviour
{
    public static FadeTransitionUI instances;
    public static bool isTransitioning;
    private CanvasGroup fadeGroup;

    private void Awake()
        => instances = this;

    private void Start()
    {
        fadeGroup = GetComponent<CanvasGroup>(); 
        isTransitioning = false;
    }

    public void StartTransition(Action startEvt = null, Action processEvt = null, Action endEvt = null)
        => StartCoroutine(FadeTransition(startEvt, processEvt, endEvt));

    private IEnumerator FadeTransition(Action startEvt, Action processEvt, Action endEvt)
    {
        bool endFade = false;
        startEvt.Invoke();

        isTransitioning = true;
        Time.timeScale = 0;

        LeanTween.alphaCanvas(fadeGroup, 1, 0.5f).setEaseInCirc().setIgnoreTimeScale(true).setOnComplete(()=> { endFade = true; });
        yield return new WaitUntil(()=> endFade == true);

        yield return new WaitForSecondsRealtime(0.15f);
        processEvt.Invoke();
        yield return new WaitForSecondsRealtime(0.15f);

        LeanTween.alphaCanvas(fadeGroup, 0, 0.5f).setEaseInCirc().setIgnoreTimeScale(true).setOnComplete(()=> { endFade = false; });
        yield return new WaitUntil(()=> endFade == false);

        Time.timeScale = 1;

        yield return new WaitForSecondsRealtime(0.05f);
        endEvt.Invoke();
        isTransitioning = false;
    }
}
