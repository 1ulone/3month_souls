using UnityEngine;

namespace wine.player.interact
{
    public class InteractItem : MonoBehaviour 
    {
        [SerializeField] private ItemData data;

        public void Interact()
        {
            wine.player.ui.InventoryUI.instances.AddItem(data);
            Destroy(this.gameObject); // HACK:<- just destroy it... maybe if there is a case where an item spawn. then maybe we'll use pool
        }
    }
}
