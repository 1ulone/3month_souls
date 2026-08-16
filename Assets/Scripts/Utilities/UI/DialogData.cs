using UnityEngine;

namespace wine.util.ui
{
    [CreateAssetMenu(fileName = "New Dialog Data", menuName = "Data/DialogData")]
    public class DialogData : ScriptableObject 
    {
        [SerializeField] public RuntimeAnimatorController potrait;
        [SerializeField] public string speakerName;
        [SerializeField] public Dialog[] dialogs;
    }

    [System.Serializable]
    public class Dialog 
    {
        [TextArea] public string content;
        public dialogMood responsesToWhatMood;
        public Response[] playerResponses;

        public bool resetCurrentMood;
        public bool endOfDialogue;
    }

    [System.Serializable]
    public class Response 
    {
        [TextArea] public string content;
        public dialogMood mood;
    }

    public enum dialogMood 
    {
        neutral,
        positive, 
        negative,
        important,
    }
}
