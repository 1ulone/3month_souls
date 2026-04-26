using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instances;
    public static bool ignoreThreshold = false;

    [SerializeField] private Transform defaultTarget;
    [SerializeField] private float cameraSpeed = 7.5f;
    [SerializeField] private Vector3 maxCameraThreshold, minCameraThreshold;

    public Transform target { get; private set; }
    private float camHeight;
    private float camWidth;

    private static readonly Vector3 defaultOffset = new Vector3(0, 8, -7.5f);
    private static readonly Vector3 defaultRotation = new Vector3(47.5f, 0, 0);

    private void Awake()
    {
        instances = this;
    }

    private void Start()
    {
        target = defaultTarget;
        ResetCameraPosition();
   
        camHeight = 2f * Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    private void FixedUpdate()
    {
        if (Time.timeScale == 0)
            return;

        if (maxCameraThreshold == Vector3.zero || minCameraThreshold == Vector3.zero)
            return;

        if (ignoreThreshold)
            return;

        Vector3 smoothCam = Vector3.Lerp(transform.position, target.position, cameraSpeed * Time.fixedUnscaledDeltaTime);
        Vector3 lockedCam = new Vector3(
                Mathf.Clamp(smoothCam.x, minCameraThreshold.x, maxCameraThreshold.x), smoothCam.y,
                Mathf.Clamp(smoothCam.z, minCameraThreshold.z, maxCameraThreshold.z));
        this.transform.position = lockedCam;
    }

    public void SetCameraThreshold(Vector2 min, Vector2 max)
    {
        minCameraThreshold = new Vector3(min.x + camWidth/2f, 0, min.y + camHeight/1.75f);
        maxCameraThreshold = new Vector3(max.x - camWidth/2f, 0, max.y - camHeight/1.75f);

        Vector3 lockedCam = new Vector3(
                Mathf.Clamp(target.position.x, minCameraThreshold.x, maxCameraThreshold.x), 0,
                Mathf.Clamp(target.position.z, minCameraThreshold.z, maxCameraThreshold.z));
        this.transform.position = lockedCam;
    }
    
    public void UpdateCameraPositionOnZeroTimeScale()
    {
        this.transform.position = target.position;
    }

    public void MoveCameraOffset(Vector3 offsetPos, Vector3 offsetRot)
    {
        Camera.main.transform.localPosition = offsetPos;
        Camera.main.transform.eulerAngles = offsetRot;
    }

    public void ResetCameraPosition()
    {
        Camera.main.transform.localPosition = defaultOffset;
        Camera.main.transform.eulerAngles = defaultRotation;
    }
}
