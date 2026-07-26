using UnityEngine;
using TMPro;

namespace wine.player.ui
{
    public class HintUI : MonoBehaviour
    {
        public static HintUI instances; 

        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private CanvasGroup panel;
        private GameObject currentHint;

        private void Awake()
        {
            instances = this;
            panel.alpha = 0;
        }

        public void SetText(string msg, GameObject currentHint)
        {
            if (panel.alpha == 1)
                return;

            this.currentHint = currentHint;
            this.currentHint.layer = 0;
            text.text = msg;
            LeanTween.alphaCanvas(panel, 1, 0.5f);
            Invoke("ExitHint", 5.0f);
        } 

        public void ExitHint()
        {
            this.currentHint.layer = 9;
            LeanTween.alphaCanvas(panel, 0,0.5f);
        }
    }
}
