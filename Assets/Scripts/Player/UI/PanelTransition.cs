using UnityEngine;
using System.Collections;

namespace wine.player.ui
{
    public class PanelTransition : MonoBehaviour
    {
        public static IEnumerator beginTransition(RectTransform panel, bool open = false, float speed = 0.25f)
        {
            bool phaseOne = false;

            panel.localScale = open ? Vector3.zero : Vector3.one;
            LeanTween.scale(panel, new Vector3(1, 0, 1), speed/2).setIgnoreTimeScale(true).setEase(LeanTweenType.linear);
            LeanTween.scale(panel, open ? Vector3.one : Vector3.zero, speed/2).setDelay(speed/4).setOnComplete(()=> phaseOne = true).setIgnoreTimeScale(true).setEase(LeanTweenType.linear);
            yield return new WaitUntil(()=> phaseOne);
        }
    }
}
