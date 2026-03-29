using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class BonfireUI : MonoBehaviour
{
    public static BonfireUI instances;
    private const float uiDelay = 0.15f;

    [SerializeField] private CanvasGroup secondPanel;
    [SerializeField] private Image[] mainOptions;
    [SerializeField] private Image cursor;

    [Header("Level Up Panel")]
    [SerializeField] private GameObject levelupPanel; 
    [SerializeField] private List<StatsValue> mainStats;

    [Header("Result Stats")]
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI damage;
    [SerializeField] private TextMeshProUGUI defense;
    [SerializeField] private TextMeshProUGUI speed;
    [SerializeField] private TextMeshProUGUI rollspeed;
    [SerializeField] private TextMeshProUGUI downtime;
    [SerializeField] private TextMeshProUGUI knockforce;
    // [SerializeField] private GameObject dialogPanel; 

    private int mainOptionsID, levelOptionsID;
    private bool isOnUI, canMove;
    private CanvasGroup mainUI;
    private InputController input;
    private PlayerStats stats;
    private Dictionary<string, TextMeshProUGUI> levelupOptions = new Dictionary<string, TextMeshProUGUI>();

    private void Awake()
    {
        instances = this;
        isOnUI = false;
        mainUI = GetComponent<CanvasGroup>();

        levelupOptions = new Dictionary<string, TextMeshProUGUI>();
        foreach(StatsValue sv in mainStats)
            levelupOptions.Add(sv.tag, sv.value);

        levelupPanel.SetActive(false);
        ToggleCanvasGroup(secondPanel, false);
        ToggleCanvasGroup(mainUI, false);
    }

    private void Start()
    {
        input = FindFirstObjectByType<InputController>();
        stats = FindFirstObjectByType<PlayerStats>();
        canMove = true;

        mainOptionsID = 0;
        MoveCursor(mainOptions[mainOptionsID].rectTransform);
    }

    private void MoveCursor(RectTransform dest)
    {
        cursor.rectTransform.position = dest.position;
        cursor.rectTransform.sizeDelta = dest.sizeDelta;
    }

    private void ToggleCanvasGroup(CanvasGroup cgt, bool b)
    {
        cgt.alpha = b ? 1 : 0;
        cgt.interactable = b;
        cgt.blocksRaycasts = b;
    }

    public void ToggleUI()
    {
        isOnUI = !isOnUI;
        if (isOnUI)
            Time.timeScale = 0;
        else 
            Time.timeScale = 1;

        mainOptionsID = 0;
        MoveCursor(mainOptions[mainOptionsID].rectTransform);
        ToggleCanvasGroup(secondPanel, false);
        ToggleCanvasGroup(mainUI, isOnUI);
    }

    private void Update()
    {
        if (!isOnUI || !canMove)
            return;

        Vector2 rawdir = input.move.ReadValue<Vector2>();
        Vector2Int dir = new Vector2Int(0, (int)rawdir.y);
        if (dir.y != 0)
        {
            if (levelupPanel.activeSelf)
                StartCoroutine(MoveOnLevelPanel(dir.y));
            else
                StartCoroutine(MoveOnMainUI(dir.y));
        }

        if (input.interact.WasPressedThisFrame())
        {
            if (levelupPanel.activeSelf)
            {
                switch(levelOptionsID)
                {
                    case 0: {} break; //vit
                    case 1: {} break; //str
                    case 2: {} break; //con
                    case 3: {} break; //dex
                    case 4: {} break; //poi
                    case 5: { StartCoroutine(CloseLevelPanel(false)); } break; //discard
                    case 6: { StartCoroutine(CloseLevelPanel(true)); } break; //accept
                }
            } 
            else 
            {
                switch(mainOptionsID)
                {
                    case 0 : { StartCoroutine(OpenLevelPanel()); } break;
                    case 1 : { OpenDialoguePanel(); } break;
                    case 2 : { ToggleUI(); } break;
                    default: {} break;
                }
            }
        }
    }

    private IEnumerator MoveOnMainUI(int ydir)
    {
        canMove = false;
        if (ydir > 0)
            mainOptionsID--;
        if (ydir < 0)
            mainOptionsID++;

        if (mainOptionsID > mainOptions.Length - 1)
            mainOptionsID = 0;
        if (mainOptionsID < 0)
            mainOptionsID = mainOptions.Length - 1;

        yield return new WaitForSecondsRealtime(uiDelay);
        MoveCursor(mainOptions[mainOptionsID].rectTransform);

        canMove = true;
    }

    private IEnumerator MoveOnLevelPanel(int ydir)
    {
        canMove = false;
        TextMeshProUGUI[] values = levelupOptions.Values.ToArray();

        if (ydir > 0)
            levelOptionsID--;
        if (ydir < 0)
            levelOptionsID++;

        if (levelOptionsID > values.Length - 1)
            levelOptionsID = 0;
        if (levelOptionsID < 0)
            levelOptionsID = values.Length - 1;


        yield return new WaitForSecondsRealtime(uiDelay);
        MoveCursor(values[levelOptionsID].rectTransform);
        
        canMove = true;
    }

    private IEnumerator OpenLevelPanel()
    {
        levelupOptions["vitality"].text = stats.vitality.ToString();
        levelupOptions["strength"].text = stats.strength.ToString();
        levelupOptions["constitution"].text = stats.constitution.ToString();
        levelupOptions["dexterity"].text = stats.dexterity.ToString();
        levelupOptions["poise"].text = stats.poise.ToString();

        health.text = stats.health.ToString();
        damage.text = stats.damage.ToString();
        defense.text = stats.defense.ToString();
        speed.text = stats.speed.ToString();
        rollspeed.text = Math.Round(stats.rollspeed, 2).ToString();
        downtime.text = Math.Round(stats.downtime, 2).ToString();
        knockforce.text = Math.Round(stats.knockforce, 2).ToString();

        ToggleCanvasGroup(secondPanel, true);
        levelupPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(uiDelay);
        levelOptionsID = 0;
        MoveCursor(levelupOptions["vitality"].rectTransform);
    }

    private IEnumerator CloseLevelPanel(bool accept)
    {
        ToggleCanvasGroup(secondPanel, false);
        levelupPanel.SetActive(false);

        yield return new WaitForSecondsRealtime(uiDelay);
        mainOptionsID = 0;
        MoveCursor(mainOptions[mainOptionsID].rectTransform);
    }

    private void OpenDialoguePanel()
    {

    }
}

[System.Serializable]
public class StatsValue
{
    public string tag;
    public TextMeshProUGUI value;
}

