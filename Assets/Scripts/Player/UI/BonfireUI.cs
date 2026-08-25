using System;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

using wine.util;
using wine.util.ui;

namespace wine.player.ui
{
    public class BonfireUI : MonoBehaviour
    {
        public static BonfireUI instances;
        private const float uiDelay = 0.15f;

        [SerializeField] private CinemachineCamera dialogCamera;

        [SerializeField] private CanvasGroup firstPanel;
        [SerializeField] private CanvasGroup secondPanel;
        [SerializeField] private Transform[] mainOptions;
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

        [Header("Dialog Data")]
        [SerializeField] private GameObject dialogPanel; 
        [SerializeField] private DialogData placeholderDialog;
        [SerializeField] private GameObject placeholderNPCobj;
        [SerializeField] private GameObject placeholderPlayerGet;

        private int mainOptionsID, levelOptionsID;
        private int previewLevel, previewExp, previewNextLevelExp;
        private bool isOnUI, canMove;
        private InputController input;
        private PlayerStats stats;

        private void Awake()
        {
            instances = this;
            isOnUI = false;

            dialogPanel.SetActive(false);
            levelupPanel.SetActive(false);
            plusMinIcon.SetActive(false);

            secondPanel.alpha = 0;
            secondPanel.interactable = false;
            secondPanel.blocksRaycasts = false;

            firstPanel.alpha = 0;
            firstPanel.interactable = false;
            firstPanel.blocksRaycasts = false;

            // ToggleCanvasGroup(secondPanel, false);
            // ToggleCanvasGroup(mainUI, false);
        }

        private void Start()
        {
            input = InputController.instances;
            stats = PlayerStats.instances;
            canMove = true;

            mainOptionsID = 0;
            cursor.enabled = false;
            MoveCursor(mainOptions[mainOptionsID]);
        }

        // private void MoveCursor(RectTransform dest)
        private void MoveCursor(Transform dest)
        {
            cursor.rectTransform.position = dest.position;
            // cursor.rectTransform.sizeDelta = dest.sizeDelta;
        }

        private void ToggleCanvasGroup(CanvasGroup cgt, bool b)
        {
            cgt.alpha = b ? 1 : 0;
            cgt.interactable = b;
            cgt.blocksRaycasts = b;
        }

        private IEnumerator OpenCanvas(CanvasGroup cgt)
        {
            cgt.alpha = 1;
            canMove = false;
            yield return PanelTransition.beginTransition(cgt.GetComponent<RectTransform>(), open: true);

            canMove = true;
            mainOptionsID = 0;
            cursor.enabled = true;
            MoveCursor(mainOptions[mainOptionsID]);
        }

        private IEnumerator CloseCanvas(CanvasGroup cgt)
        {
            canMove = false;
            mainOptionsID = 0;
            cursor.enabled = false;
            yield return PanelTransition.beginTransition(cgt.GetComponent<RectTransform>(), open: false);

            cgt.alpha = 0;
            canMove = true;
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

            StartCoroutine(ToggleBonfire());
        }

        private IEnumerator ToggleBonfire()
        {
            canMove = false;
            ToggleCanvasGroup(secondPanel, false);

            yield return new WaitForSecondsRealtime(uiDelay);
            if (isOnUI)
                yield return OpenCanvas(firstPanel);
            else 
                yield return CloseCanvas(firstPanel);
                
            canMove = true;
        }

        private void Update()
        {
            if (!isOnUI || !canMove)
                return;

            Vector2 rawdir = input.Move();
            Vector2Int dir = new Vector2Int((int)rawdir.x, (int)rawdir.y);
            if (dir.y != 0 || dir.x != 0)
            {
                if (levelupPanel.activeSelf)
                    StartCoroutine(MoveOnLevelPanel(dir));
                else
                    StartCoroutine(MoveOnMainUI(dir.y));
            }

            if (input.GetInput("interact"))
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
                        case 1 : { OpenDialogPanel(); } break;
                        case 2 : { ToggleUI(); } break;
                        case 3 : { ToggleUI(); } break;
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
            MoveCursor(mainOptions[mainOptionsID]);

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

            yield return OpenCanvas(secondPanel);
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

            levelCurrent.text = "Level " + stats.level.ToString();
            levelNext.text = previewLevel.ToString();
            expHeld.text = previewExp.ToString();
            expNeed.text = previewNextLevelExp.ToString();

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

            yield return CloseCanvas(secondPanel);
            plusMinIcon.SetActive(false);
            levelupPanel.SetActive(false);

            yield return new WaitForSecondsRealtime(uiDelay);
            mainOptionsID = 0;
            MoveCursor(mainOptions[mainOptionsID]);
            cursor.enabled = true;
        }

        private void OpenDialogPanel()
        {
            //Change Camera

            StartCoroutine(TypeOutDialog(placeholderDialog));

            // placeholderPlayerGet.transform.position = placeholderNPCobj.transform.position + new Vector3(-0.5f, 0, -1);
            // CameraController.instances.UpdateCameraPositionOnZeroTimeScale();
        }

        // NOTE: for dialog and shit :
        // Position (0.8, 1.75f, -0.75f)
        // RotationEuler (25, -45, 0)
        private IEnumerator TypeOutDialog(DialogData data)
        {
            canMove = false;
            float transitionTime = 1.5f;
            yield return FadeTransitionUI.instances.FadeInOut(true, time: transitionTime);

            ToggleCanvasGroup(firstPanel, false);
            ToggleCanvasGroup(secondPanel, true);

            cursor.enabled = false;
            placeholderPlayerGet.transform.position = placeholderNPCobj.transform.position + new Vector3(-0.5f, 0, -1);
            DialogBox.instances.setSpeakerIdentity(placeholderDialog.potrait, placeholderDialog.speakerName);
            dialogCamera.Priority = 2;

            yield return FadeTransitionUI.instances.FadeInOut(false, time:transitionTime);

            yield return new WaitForSecondsRealtime(0.1f);

            dialogPanel.GetComponent<RectTransform>().localScale = Vector3.zero;
            dialogPanel.SetActive(true);

            yield return PanelTransition.beginTransition(dialogPanel.GetComponent<RectTransform>(), true);

            foreach(Dialog d in data.dialogs)
            {
                if (DialogBox.instances.nextMood != dialogMood.neutral && d.responsesToWhatMood != DialogBox.instances.nextMood)
                    continue;

                yield return DialogBox.instances.useDialogBox(d.content, this.transform);
                if (d.playerResponses.Length > 0)
                {
                    yield return DialogBox.instances.openResponseBox(d.playerResponses);
                    yield return new WaitUntil(()=> DialogBox.waitForResponse == false);
                    yield return DialogBox.instances.exitResponseBox();
                }
                else {
                    yield return new WaitUntil(()=> input.GetInput("interact"));
                }

                if (d.resetCurrentMood)
                    DialogBox.instances.nextMood = dialogMood.neutral;

                if (d.endOfDialogue)
                    break;
            }
            yield return DialogBox.instances.exitDialog();
            yield return PanelTransition.beginTransition(dialogPanel.GetComponent<RectTransform>(), false);
            dialogPanel.SetActive(false);

            // CameraController.instances.ResetCameraPosition();
            yield return FadeTransitionUI.instances.FadeInOut(true, time: transitionTime);
            dialogCamera.Priority = 0;
            cursor.gameObject.SetActive(true);
            cursor.enabled = true;
            MoveCursor(mainOptions[0]);
            ToggleCanvasGroup(firstPanel, true);
            ToggleCanvasGroup(secondPanel, false);
            yield return FadeTransitionUI.instances.FadeInOut(false, time: transitionTime);
            canMove = true;

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
}
