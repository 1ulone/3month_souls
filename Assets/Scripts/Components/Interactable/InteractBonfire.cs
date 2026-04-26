using UnityEngine;

public class InteractBonfire : MonoBehaviour, IInteractable 
{
    public void Interact(Transform other = null)
    {
        BonfireUI.instances.ToggleUI();
    }
}
