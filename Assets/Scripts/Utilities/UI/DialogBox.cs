using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

namespace wine.util.ui 
{
    public class DialogBox : MonoBehaviour
    {
        public static DialogBox instances;
        public static bool waitForResponse = false;

        [SerializeField] private TextMeshProUGUI textMesh;
        [SerializeField] private GameObject dialogBox;
        [SerializeField] private ResponseBox[] responseBoxes;
        [SerializeField] private RectTransform responseCursor;

        private Coroutine currentCoroutine;
        private int currentCursorIndex;
        private int maxActiveResponseBox;
        private bool isCursorMoving;
        private InputController input;

        public dialogMood nextMood { get; set; }

        private void Awake()
        {
            instances = this;
            textMesh.text = "";
            nextMood = dialogMood.neutral;
            dialogBox.SetActive(false);
            // responseCursor.gameObject.SetActive(false);
            maxActiveResponseBox = 0;
        }

        private void Start()
        {

            foreach(ResponseBox r in responseBoxes)
            {
                r.box.gameObject.SetActive(false);
                r.textMesh.text = "";
            }

            input = FindFirstObjectByType<InputController>();
        }

        public void Update()
        {
            if (!waitForResponse)
                return;

            Vector2 move = input.Move().normalized;
            if (move.y != 0)
                StartCoroutine(moveCursor(move));

            if (input.GetInput("interact"))
            {
                waitForResponse = false;
                nextMood = responseBoxes[currentCursorIndex].mood;
            }
        }

        private IEnumerator moveCursor(Vector2 move)
        {
            if (isCursorMoving)
                yield break;

            isCursorMoving = true;
            if (move.y > 0) //RIGHT
                currentCursorIndex++; else 
                    if (move.y < 0) //LEFT
                        currentCursorIndex--;

            if (currentCursorIndex > maxActiveResponseBox-1)
                currentCursorIndex = 0;

            if (currentCursorIndex < 0)
                currentCursorIndex = maxActiveResponseBox-1;

            yield return new WaitForSecondsRealtime(0.15f);
            responseCursor.position = responseBoxes[currentCursorIndex].box.rectTransform.localPosition;
            responseCursor.anchoredPosition = responseBoxes[currentCursorIndex].box.rectTransform.anchoredPosition;
            responseCursor.sizeDelta = responseBoxes[currentCursorIndex].box.rectTransform.sizeDelta;
            isCursorMoving = false;
        }

        public IEnumerator useDialogBox(string t, Transform speakerTarget, int spd = 30) 
        {
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);

            // dialogBox.transform.position = speakerTarget.position + (Vector3.up*3.5f);
            yield return new WaitForSecondsRealtime(1);
            dialogBox.SetActive(true);

            currentCoroutine = StartCoroutine(TypeDialog(t, spd)); 
            responseCursor.gameObject.SetActive(false);
        }

        private IEnumerator TypeDialog(string t, int spd)
        {
            textMesh.text = "";
            foreach(var c in t.ToCharArray())
            {
                textMesh.text += c;
                float cpm = c=='.'||c==','?5f/spd:1f/spd;
                yield return new WaitForSecondsRealtime(cpm);
            }

            currentCoroutine = null;
        }

        public IEnumerator exitDialog()
        {
            textMesh.text = "";
            yield return new WaitForSecondsRealtime(0.5f);
            dialogBox.SetActive(false);
            nextMood = dialogMood.neutral;
            responseCursor.gameObject.SetActive(true);
        }

        // NOTE: max of option will be 3  
        public IEnumerator openResponseBox(Response[] responseData)
        {
            waitForResponse = true;
            responseCursor.position = responseBoxes[0].box.transform.position;
            currentCursorIndex = 0;
            maxActiveResponseBox = 0;

            for (int i = 0; i < responseData.Length; i++)
            {
                responseBoxes[i].box.gameObject.SetActive(true);
                responseBoxes[i].textMesh.text = responseData[i].content;
                responseBoxes[i].mood = responseData[i].mood;
                maxActiveResponseBox++;
            }
            yield return new WaitForSecondsRealtime(0.5f);
            responseCursor.gameObject.SetActive(true);
            responseCursor.position = responseBoxes[currentCursorIndex].box.rectTransform.localPosition;
            responseCursor.anchoredPosition = responseBoxes[currentCursorIndex].box.rectTransform.anchoredPosition;
            responseCursor.sizeDelta = responseBoxes[currentCursorIndex].box.rectTransform.sizeDelta;
        }

        public IEnumerator exitResponseBox()
        {
            foreach(ResponseBox r in responseBoxes)
            {
                r.box.gameObject.SetActive(false);
                r.textMesh.text = "";
            }
            maxActiveResponseBox = 0;
            responseCursor.gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    [System.Serializable]
    public class ResponseBox 
    {
        public TextMeshProUGUI textMesh;
        public Image box; 
        public dialogMood mood;
    }
}
