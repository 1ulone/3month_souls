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

    // public void StartTransition(Action startEvt = null, Action processEvt = null, Action endEvt = null)
    //     => StartCoroutine(FadeTransition(startEvt, processEvt, endEvt));

    public IEnumerator FadeInOut(bool isFadeIn)
    {
        bool fadeOnEnd = false;

        LeanTween.alphaCanvas(fadeGroup, isFadeIn ? 1 : 0, 0.5f).setEaseInCirc().setIgnoreTimeScale(true).setOnComplete(()=> { fadeOnEnd = true; });
        yield return new WaitUntil(()=> fadeOnEnd == true);
    }


    //
    // private IEnumerator FadeTransition(Action startEvt, Action processEvt, Action endEvt)
    // {
    //     bool endFade = false;
    //     // if (startEvt != null) { yield return startEvt; }
    //     startEvt.Invoke();
    //
    //     isTransitioning = true;
    //     Time.timeScale = 0;
    //
    //     LeanTween.alphaCanvas(fadeGroup, 1, 0.5f).setEaseInCirc().setIgnoreTimeScale(true).setOnComplete(()=> { endFade = true; });
    //     yield return new WaitUntil(()=> endFade == true);
    //
    //     yield return new WaitForSecondsRealtime(0.15f);
    //     // if (processEvt != null) { yield return processEvt; }
    //     processEvt.Invoke();
    //     yield return new WaitForSecondsRealtime(0.15f);
    //
    //     LeanTween.alphaCanvas(fadeGroup, 0, 0.5f).setEaseInCirc().setIgnoreTimeScale(true).setOnComplete(()=> { endFade = false; });
    //     yield return new WaitUntil(()=> endFade == false);
    //
    //     Time.timeScale = 1;
    //
    //     yield return new WaitForSecondsRealtime(0.05f);
    //     // if (endEvt != null) { yield return endEvt; }
    //     endEvt.Invoke();
    //     isTransitioning = false;
    // }
}
