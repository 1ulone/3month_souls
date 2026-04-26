using UnityEngine;

public class InteractHint : MonoBehaviour, IInteractable
{
    [SerializeField] private string hintMessage; 
        
    public void Interact(Transform other = null)
    {
        HintUI.instances.SetText(hintMessage, this.gameObject);
    }
}
