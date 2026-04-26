using UnityEngine;

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
        director.setCustomAction(1, ()=> FindFirstObjectByType<PlayerController>().MoveRoom(boxCollider, isHorizontal));
    }

    public void EnterTransition()
    {
        if (forChangingFloor)
            return;

        if (!InventoryUI.hasKeyItem(keyNeeded) && keyNeeded != null)
            return;

        StartCoroutine(director.PlayScene());
    }
}
