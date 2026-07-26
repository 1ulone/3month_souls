using UnityEngine;

namespace wine.player.interact
{
    public class InteractHint : MonoBehaviour, IInteractable
    {
        [SerializeField] private string hintMessage; 

        public void Interact(Transform other = null)
        {
            wine.player.ui.HintUI.instances.SetText(hintMessage, this.gameObject);
        }
    }
}
