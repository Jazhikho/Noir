using UnityEngine;
using UnityEngine.UI;

public class AdventureCameraController : MonoBehaviour
{
    [Header("Aspect Ratio Settings")]
    public float targetAspectWidth = 1.37f;
    public float targetAspectHeight = 1f;

    [Header("Picture Window")]
    /// <summary>When true, the game view is drawn in a smaller rect (e.g. half size) with black bars on the sides and top/bottom.</summary>
    public bool pictureWindowMode = false;
    /// <summary>Width and height of the viewport in 0–1. e.g. 0.5 = half the screen each way (centered).</summary>
    public float pictureWindowScale = 0.5f;

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
    private Camera _letterboxClearCamera;

    void Awake()
    {
        cam = GetComponent<Camera>();
        ApplyViewport();
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

        if (pictureWindowMode)
            ConstrainOverlayCanvasesToPictureWindow();
    }

    /// <summary>
    /// Overlay Canvases draw on the full screen and hide the black bars. Switch them to Screen Space - Camera
    /// with this camera so they only draw inside the picture window (same as OfficeFloor).
    /// </summary>
    private void ConstrainOverlayCanvasesToPictureWindow()
    {
        if (cam == null) return;

        Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas canvas in allCanvases)
        {
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                continue;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = 100f;
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
        Vector3 desiredPosition = GetDesiredPosition();

        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        transform.position = smoothedPosition;
    }

    Vector3 GetDesiredPosition()
    {
        if (target == null) return roomPosition;

        Vector3 desiredPosition = new Vector3(
            target.position.x + horizontalOffset,
            roomPosition.y + verticalOffset,
            roomPosition.z
        );

        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        }

        return desiredPosition;
    }

    /// <summary>
    /// Applies viewport: picture-window mode (centered smaller rect + black bars) or aspect-ratio letterboxing.
    /// Unity only clears the camera's viewport rect, so we add a full-screen clear camera when in picture-window mode
    /// so the bar regions are black in every scene (main menu and game).
    /// </summary>
    void ApplyViewport()
    {
        if (pictureWindowMode)
        {
            EnsureLetterboxClearCamera();
            float margin = (1f - pictureWindowScale) * 0.5f;
            cam.rect = new Rect(margin, margin, pictureWindowScale, pictureWindowScale);
            cam.backgroundColor = Color.black;
            if (cam.clearFlags == CameraClearFlags.Skybox)
                cam.clearFlags = CameraClearFlags.SolidColor;
        }
        else
        {
            DisableLetterboxClearCamera();
            EnforceAspectRatio();
        }
    }

    /// <summary>
    /// Creates or enables a camera that clears the entire screen to black (depth below main cam). Required because Unity only clears the viewport rect.
    /// </summary>
    private void EnsureLetterboxClearCamera()
    {
        if (_letterboxClearCamera != null)
        {
            _letterboxClearCamera.gameObject.SetActive(true);
            _letterboxClearCamera.rect = new Rect(0f, 0f, 1f, 1f);
            _letterboxClearCamera.depth = cam.depth - 1f;
            return;
        }

        GameObject clearGo = new GameObject("LetterboxClearCamera");
        clearGo.transform.SetParent(transform.parent, false);

        _letterboxClearCamera = clearGo.AddComponent<Camera>();
        _letterboxClearCamera.rect = new Rect(0f, 0f, 1f, 1f);
        _letterboxClearCamera.depth = cam.depth - 1f;
        _letterboxClearCamera.clearFlags = CameraClearFlags.SolidColor;
        _letterboxClearCamera.backgroundColor = Color.black;
        _letterboxClearCamera.cullingMask = 0;
        _letterboxClearCamera.orthographic = cam.orthographic;
        _letterboxClearCamera.orthographicSize = cam.orthographicSize;
        _letterboxClearCamera.nearClipPlane = cam.nearClipPlane;
        _letterboxClearCamera.farClipPlane = cam.farClipPlane;
        _letterboxClearCamera.useOcclusionCulling = false;
    }

    private void DisableLetterboxClearCamera()
    {
        if (_letterboxClearCamera != null)
            _letterboxClearCamera.gameObject.SetActive(false);
    }

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

    public void SetRoomBounds(BoxCollider2D bounds)
    {
        if (bounds == null)
        {
            useBounds = false;
            return;
        }

        Bounds b = bounds.bounds;
        float roomMinX = b.min.x;
        float roomMaxX = b.max.x;

        float halfViewWidth = GetCameraHalfWidthWorld();
        float allowedMin = roomMinX + halfViewWidth;
        float allowedMax = roomMaxX - halfViewWidth;

        // Room is wider than camera view - use follow mode with bounds
        if (allowedMin <= allowedMax)
        {
            minX = allowedMin;
            maxX = allowedMax;
            useBounds = true;
            useRoomMode = false;
        }
        // Room fits on screen - use static room mode
        else
        {
            float roomCenterX = (roomMinX + roomMaxX) * 0.5f;
            minX = roomCenterX;
            maxX = roomCenterX;
            useBounds = false;
            useRoomMode = true;
        }

        roomPosition = new Vector3(b.center.x, b.center.y, roomPosition.z);

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }

        SnapToTarget();
    }

    // Instantly moves camera to correct position without lerping
    public void SnapToTarget()
    {
        if (useRoomMode)
        {
            transform.position = roomPosition;
        }
        else
        {
            transform.position = GetDesiredPosition();
        }
    }

    private float GetCameraHalfWidthWorld()
    {
        if (cam == null) cam = GetComponent<Camera>();
        float orthoSize = cam != null ? cam.orthographicSize : 8f;
        float aspect = targetAspectWidth / targetAspectHeight;
        return orthoSize * aspect;
    }

    void OnValidate()
    {
        if (cam != null)
        {
            ApplyViewport();
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
