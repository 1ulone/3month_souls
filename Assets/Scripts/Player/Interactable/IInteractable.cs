using UnityEngine;

namespace wine.player.interact
{
    public interface IInteractable 
    {
        public void Interact(Transform other = null) {}
    }
}
