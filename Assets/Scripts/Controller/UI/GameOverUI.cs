using UnityEngine;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI instances;
    private CanvasGroup panel;

    private void Awake()
    {
        instances = this;
        panel = GetComponent<CanvasGroup>();
        panel.alpha = 0;
        panel.interactable = false;
        panel.blocksRaycasts = false;
    }

    public void StartPanel()
    {
        Time.timeScale = 0;
        panel.interactable = true;
        panel.blocksRaycasts = true;
        StartCoroutine(PanelNumerator());
    }

    private IEnumerator PanelNumerator()
    {
        bool transition = false;

        LeanTween.alphaCanvas(panel, 1, 0.5f)
            .setEaseInOutCubic()
            .setIgnoreTimeScale(true)
            .setOnComplete(()=> transition = true);

        yield return new WaitUntil(()=> transition == true);
    }

    public void OnRetry()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
