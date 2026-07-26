using UnityEngine;

namespace wine.player.interact
{
    public class InteractBonfire : MonoBehaviour, IInteractable 
    {
        public void Interact(Transform other = null)
        {
            wine.player.ui.BonfireUI.instances.ToggleUI();
        }
    }
}
