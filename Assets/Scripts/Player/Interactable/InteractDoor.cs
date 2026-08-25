using UnityEngine;
using wine.cutscene;

namespace wine.player.interact
{
    public class InteractDoor : MonoBehaviour
    {
        [SerializeField] private bool isHorizontal;
        [SerializeField] private KeyItemData keyNeeded; 
        [SerializeField] public bool forChangingFloor;

        private BoxCollider boxCollider;
        private CutsceneDirector director;

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();

            if (keyNeeded != null)
                boxCollider.isTrigger = false;
            else 
                boxCollider.isTrigger = true;

            director = GetComponent<CutsceneDirector>();
            director.setCustomAction(
                1, ()=> FindFirstObjectByType<PlayerCollisionTrigger>().MoveRoom(boxCollider, isHorizontal) 
            );
        }

        public void EnterTransition()
        {
            if (forChangingFloor)
                return;

            if (!wine.player.ui.InventoryUI.hasKeyItem(keyNeeded) && keyNeeded != null)
                return;

            StartCoroutine(director.PlayScene());
        }
    }
}
