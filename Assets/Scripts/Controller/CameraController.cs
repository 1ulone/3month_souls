using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instances;
    public static bool ignoreThreshold = false;

    [SerializeField] private Transform defaultTarget;
    [SerializeField] private float cameraSpeed = 7.5f;
    [SerializeField] private Vector3 maxCameraThreshold, minCameraThreshold;

    public Transform target { get; private set; }
    // private const int zValue = -10;
    private float camHeight;
    private float camWidth;

    private void Awake()
    {
        instances = this;
    }

    private void Start()
    {
        target = defaultTarget;

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
                Mathf.Clamp(smoothCam.x, minCameraThreshold.x, maxCameraThreshold.x), 0,
                Mathf.Clamp(smoothCam.z, minCameraThreshold.z, maxCameraThreshold.z));
        this.transform.position = lockedCam;
    }

    public void SetCameraThreshold(Vector2 min, Vector2 max)
    {
        minCameraThreshold = new Vector2(min.x + camWidth/2, min.y + camHeight/2);
        maxCameraThreshold = new Vector2(max.x - camWidth/2, max.y - camHeight/2);
    }
}
