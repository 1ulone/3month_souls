using UnityEngine;
using Unity.Cinemachine;

namespace wine.core 
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController instances;

        [SerializeField] public CinemachineCamera currentCamera; 
        [SerializeField] public CinemachineCamera cutsceneCamera; 
        [SerializeField] public CinemachineCamera uiCamera; 

        private CinemachineBrain brain;

        private void Awake()
        {
            instances = this;
            brain = GetComponent<CinemachineBrain>();
        }

        public void ChangeCamera(CinemachineCamera newCam)
        {
            brain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.Linear;
            currentCamera.Priority = 0;
            currentCamera.enabled = false;

            newCam.Priority = 1;
            newCam.enabled = true;
            Invoke("resetBlendType", 0.2f);

            currentCamera = newCam;
        }

        private void resetBlendType()
            => brain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.Cut;
    }
}
