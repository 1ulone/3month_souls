using System;
using UnityEngine;
using System.Collections;
using wine.util;

namespace wine.cutscene
{
    public class CutsceneDirector : MonoBehaviour
    {
        public static CutsceneDirector instances;

        [SerializeField] private ActionScene[] actions;

        private BaseAction currentAction;
        private Transform currentObject;
        private Vector3 destination;

        private void Awake()
        {
            instances = this;
        }

        public IEnumerator PlayScene()
        {
            Time.timeScale = 0;
            InputController.instances.DisableInput();

            foreach(ActionScene s in actions)
                yield return s.action.coroutine;

            InputController.instances.EnableInput();
            Time.timeScale = 1;
        }

        public void setCustomAction(int id, Action evt, Func<bool> customUntilBoolean = null)
        {
            foreach(ActionScene s in actions)
            {
                if (s.action.type == actionType.customAction)
                {
                    if (s.action.customID == id)
                    {
                        s.action.customAction = evt;
                        s.action.customUntilBoolean = customUntilBoolean;
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class ActionScene
    {
        public string sceneTitle;
        public BaseAction action;
    }
}
