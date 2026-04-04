using System;
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
    [SerializeField] private GameObject plusMinIcon;

    [Header("Level Up Panel")]
    [SerializeField] private GameObject levelupPanel; 
    [SerializeField] private List<StatsValue> previewData;
    
    [Header("Exp and Level")]
    [SerializeField] private TextMeshProUGUI levelCurrent;
    [SerializeField] private TextMeshProUGUI levelNext;
    [SerializeField] private TextMeshProUGUI expHeld;
    [SerializeField] private TextMeshProUGUI expNeed;

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
    private int previewLevel, previewExp, previewNextLevelExp;
    private bool isOnUI, canMove;
    private CanvasGroup mainUI;
    private InputController input;
    private PlayerStats stats;

    private void Awake()
    {
        instances = this;
        isOnUI = false;
        mainUI = GetComponent<CanvasGroup>();

        levelupPanel.SetActive(false);
        plusMinIcon.SetActive(false);
        ToggleCanvasGroup(secondPanel, false);
        ToggleCanvasGroup(mainUI, false);
    }

    private void Start()
    {
        input = FindFirstObjectByType<InputController>();
        stats = PlayerStats.instances;
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
        if (!canMove)
            return;

        isOnUI = !isOnUI;
        if (isOnUI)
            Time.timeScale = 0;
        else 
            Time.timeScale = 1;

        StartCoroutine(OpenMainCanvas());
    }

    private IEnumerator OpenMainCanvas()
    {
        canMove = false;
        mainOptionsID = 0;
        MoveCursor(mainOptions[mainOptionsID].rectTransform);

        yield return new WaitForSecondsRealtime(uiDelay);
        ToggleCanvasGroup(secondPanel, false);
        ToggleCanvasGroup(mainUI, isOnUI);

        canMove = true;
    }

    private void Update()
    {
        if (!isOnUI || !canMove)
            return;

        Vector2 rawdir = input.move.ReadValue<Vector2>();
        Vector2Int dir = new Vector2Int((int)rawdir.x, (int)rawdir.y);
        if (dir.y != 0 || dir.x != 0)
        {
            if (levelupPanel.activeSelf)
                StartCoroutine(MoveOnLevelPanel(dir));
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

    private IEnumerator MoveOnLevelPanel(Vector2 dir)
    {
        canMove = false;

        if (levelOptionsID < 5)
        {
            if (dir.y > 0)
                levelOptionsID--; else 
            if (dir.y < 0)
                levelOptionsID++; else 
            if (dir.x > 0)
            {
                if (previewExp - previewNextLevelExp > 0)
                {
                    previewLevel++; 
                    previewData[levelOptionsID].value++;
                    previewExp -= previewNextLevelExp; 
                    previewNextLevelExp = stats.expFormula(previewLevel);
                }
            } else 
            if (dir.x < 0)
            {
                // 2 < 2 or 6 > 6
                //  
                if (stats.level+1 < previewLevel && previewData[levelOptionsID].value > previewData[levelOptionsID].baseValue)
                {
                    previewLevel--;
                    previewData[levelOptionsID].value--;
                    previewNextLevelExp = stats.expFormula(previewLevel); 
                    previewExp += previewNextLevelExp;
                }
            }
        } else 
        if (levelOptionsID >= 5)
        {
            if (dir.x > 0 || dir.x < 0)
            {
                if (levelOptionsID == 5)
                    levelOptionsID = 6; else 
                if (levelOptionsID == 6)
                    levelOptionsID = 5;
            }
            if (dir.y > 0)
                levelOptionsID = 4;
            if (dir.y < 0)
                levelOptionsID = 0;
        }

        if (levelOptionsID > previewData.Count - 1)
            levelOptionsID = 0;
        if (levelOptionsID < 0)
            levelOptionsID = previewData.Count - 1;

        yield return new WaitForSecondsRealtime(uiDelay);

        RefreshLevelUI();
        MoveCursor(previewData[levelOptionsID].gui.rectTransform);
        if (levelOptionsID == 5 || levelOptionsID == 6)
            plusMinIcon.SetActive(false); else 
        if (plusMinIcon.activeSelf == false)
            plusMinIcon.SetActive(true);
        
        canMove = true;
    }

    private IEnumerator OpenLevelPanel()
    {
        previewLevel = stats.level+1;
        previewExp = stats.exp;
        previewNextLevelExp = stats.nextRequiredLevel;

        previewData[0].value = stats.vitality;
        previewData[1].value = stats.strength;
        previewData[2].value = stats.constitution;
        previewData[3].value = stats.dexterity;
        previewData[4].value = stats.poise;

        previewData[0].baseValue = stats.vitality;
        previewData[1].baseValue = stats.strength;
        previewData[2].baseValue = stats.constitution;
        previewData[3].baseValue = stats.dexterity;
        previewData[4].baseValue = stats.poise;

        RefreshLevelUI();

        ToggleCanvasGroup(secondPanel, true);
        plusMinIcon.SetActive(true);
        levelupPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(uiDelay);
        levelOptionsID = 0;
        MoveCursor(previewData[0].gui.rectTransform);
    }

    private void RefreshLevelUI()
    {
        previewData[0].gui.text = previewData[0].value.ToString();
        previewData[1].gui.text = previewData[1].value.ToString();
        previewData[2].gui.text = previewData[2].value.ToString();
        previewData[3].gui.text = previewData[3].value.ToString();
        previewData[4].gui.text = previewData[4].value.ToString();

        levelCurrent.text = stats.level.ToString();
        levelNext.text = previewLevel.ToString();
        expHeld.text = "Exp Held : " + previewExp.ToString();
        expNeed.text = "Exp Needed : " + previewNextLevelExp.ToString();

        /* NOTE:
         * 0 = vit
         * 1 = str
         * 2 = con
         * 3 = dex
         * 4 = poi */

        health.text = stats.previewHealth(previewData[0].value, previewData[4].value, previewData[2].value).ToString();
        damage.text = stats.previewDamage(previewData[1].value, previewData[3].value).ToString();
        defense.text = stats.previewDefense(previewData[2].value, previewData[4].value).ToString();
        speed.text = stats.previewSpeed(previewData[3].value).ToString();
        rollspeed.text = Math.Round(stats.previewRollspeed(previewData[3].value, previewData[2].value), 2).ToString();
        downtime.text = Math.Round(stats.previewDowntime(previewData[4].value, previewData[3].value), 2).ToString();
        knockforce.text = Math.Round(stats.previewKnockforce(previewData[4].value, previewData[0].value, previewData[3].value), 2).ToString();
    }

    private IEnumerator CloseLevelPanel(bool accept)
    {
        if (accept)
        {
            stats.levelUp(
                previewLevel,
                previewData[0].value,
                previewData[1].value,
                previewData[2].value,
                previewData[3].value,
                previewData[4].value
            );
        }

        ToggleCanvasGroup(secondPanel, false);
        plusMinIcon.SetActive(false);
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
    public TextMeshProUGUI gui;
    [HideInInspector] public int value;
    [HideInInspector] public int baseValue;
}
