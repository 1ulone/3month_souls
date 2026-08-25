using UnityEngine;
using Unity.Cinemachine;
using wine.core;

namespace wine.util.component
{
    public class RoomTriggerComponent : MonoBehaviour
    {
        [SerializeField] private string roomID;
        [SerializeField] private CinemachineCamera roomCamera;
        private GameObject room;

        private void Awake()
        {
            roomID = transform.name;
        }

        private void Start()
        {
            int separator = roomID.IndexOf("_");
            room = GameObject.Find("ROOM_"+roomID[(separator+1)..]);

            if (room.name != "ROOM_1")
            {
                room.SetActive(false);
                roomCamera.enabled = false;
            }
            else 
                GameController.currentRoom = room;
        }

        public void TriggerBoundingBox()
        {
            if (GameController.roomID == roomID)
                return;

            CameraController.instances.ChangeCamera(roomCamera);

            GameController.currentRoom.SetActive(false);

            GameController.roomID = roomID;
            GameController.currentRoom = room;

            room.SetActive(true);
        }
    }
}
