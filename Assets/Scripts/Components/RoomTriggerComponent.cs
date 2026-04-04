using UnityEngine;

public class RoomTriggerComponent : MonoBehaviour
{
    [SerializeField] private int roomID;

    private Vector2 newMinThreshold;
    private Vector2 newMaxThreshold;
    private BoxCollider triggerBox;

    private void Awake()
    {
        triggerBox = GetComponent<BoxCollider>();

        newMinThreshold = new Vector2(triggerBox.bounds.min.x, triggerBox.bounds.min.z);
        newMaxThreshold = new Vector2(triggerBox.bounds.max.x, triggerBox.bounds.max.z);
    }

    public void TriggerBoundingBox()
    {
        if (GameController.roomID == roomID)
            return;

        CameraController.instances.SetCameraThreshold(newMinThreshold, newMaxThreshold);
        GameController.roomID = roomID;
    }
}
