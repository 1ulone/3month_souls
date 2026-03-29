using UnityEngine;

public class InteractBonfire : MonoBehaviour, IInteractable 
{
    public void Interact()
    {
        BonfireUI.instances.ToggleUI();
    }
}
