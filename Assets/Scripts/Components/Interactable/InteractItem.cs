using UnityEngine;

public class InteractItem : MonoBehaviour 
{
    [SerializeField] private ItemData data;

    public void Interact()
    {
        InventoryUI.instances.AddItem(data);
        Destroy(this.gameObject); // HACK:<- just destroy it... maybe if there is a case where an item spawn. then maybe we'll use pool
    }
}
