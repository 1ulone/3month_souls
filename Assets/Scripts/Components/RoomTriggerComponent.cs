using UnityEngine;
using Unity.Cinemachine;

public class RoomTriggerComponent : MonoBehaviour
{
    [SerializeField] private string roomID;

    private Vector2 newMinThreshold;
    private Vector2 newMaxThreshold;
    private BoxCollider triggerBox;
    private CinemachineConfiner3D boundController;

    private void Awake()
    {
        triggerBox = GetComponent<BoxCollider>();
        roomID = transform.name;
        boundController = FindFirstObjectByType<CinemachineConfiner3D>();
    }

    public void TriggerBoundingBox()
    {
        if (GameController.roomID == roomID)
            return;

        boundController.BoundingVolume = triggerBox;
        GameController.roomID = roomID;
    }
}
