using UnityEngine;

public class AdventureCameraController : MonoBehaviour
{
    [Header("Aspect Ratio Settings")]
    public float targetAspectWidth = 1.37f;
    public float targetAspectHeight = 1f;

    [Header("Room Mode (Static Camera)")]
    public bool useRoomMode = true;
    public Vector3 roomPosition = new Vector3(0f, 0f, -10f);

    [Header("Follow Mode (For Wide Rooms)")]
    public Transform target;
    public float followSpeed = 2f;
    public float horizontalOffset = 0f;
    public float verticalOffset = 0f;

    [Header("Camera Bounds (Follow Mode Only)")]
    public bool useBounds = false;
    public float minX = -10f;
    public float maxX = 10f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        EnforceAspectRatio();
    }

    void Start()
    {
        if (useRoomMode)
        {
            transform.position = roomPosition;
        }
        else if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (useRoomMode)
        {
            transform.position = roomPosition;
            return;
        }

        if (target != null)
        {
            FollowTarget();
        }
    }

    void FollowTarget()
    {
        Vector3 desiredPosition = new Vector3(
            target.position.x + horizontalOffset,
            roomPosition.y + verticalOffset,
            roomPosition.z
        );

        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        }

        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        transform.position = smoothedPosition;
    }

    // Adds black bars to enforce the 1.37:1 classic film aspect ratio
    void EnforceAspectRatio()
    {
        float targetAspect = targetAspectWidth / targetAspectHeight;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            Rect rect = cam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0f;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            cam.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0f;
            cam.rect = rect;
        }
    }

    public void SetRoomPosition(Vector3 newPosition)
    {
        roomPosition = newPosition;
        if (useRoomMode)
        {
            transform.position = roomPosition;
        }
    }

    public void SnapToRoom(Vector3 newPosition)
    {
        roomPosition = newPosition;
        transform.position = roomPosition;
    }

    public void SetFollowMode(bool follow)
    {
        useRoomMode = !follow;
    }

    void OnValidate()
    {
        if (cam != null)
        {
            EnforceAspectRatio();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!useBounds || useRoomMode) return;

        Gizmos.color = Color.yellow;
        float height = 10f;
        Vector3 center = new Vector3((minX + maxX) / 2f, roomPosition.y, 0f);
        Vector3 size = new Vector3(maxX - minX, height, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
}