using UnityEngine;

/// <summary>
/// Controls camera behavior for 2D adventure games with support for static room views and follow modes.
/// Enforces a classic film aspect ratio (1.37:1) with letterboxing.
/// </summary>
public class AdventureCameraController : MonoBehaviour
{
    [Header("Aspect Ratio Settings")]
    // Width component of the target aspect ratio (1.37 for classic film noir look).
    [SerializeField] private float targetAspectWidth = 1.37f;
    
    // Height component of the target aspect ratio (typically 1.0).
    [SerializeField] private float targetAspectHeight = 1f;

    [Header("Room Mode (Static Camera)")]
    // If true, camera stays fixed at roomPosition. If false, camera follows the target.
    [SerializeField] private bool useRoomMode = true;
    
    // Fixed position for the camera in room mode.
    [SerializeField] private Vector3 roomPosition = new Vector3(0f, 0f, -10f);

    [Header("Follow Mode (For Wide Rooms)")]
    // Target transform to follow when not in room mode. If null, attempts to find Player tag.
    [SerializeField] private Transform target;
    
    // Speed at which camera smoothly follows the target.
    [SerializeField] private float followSpeed = 2f;
    
    // Horizontal offset from target position.
    [SerializeField] private float horizontalOffset = 0f;
    
    // Vertical offset from room position Y.
    [SerializeField] private float verticalOffset = 0f;

    [Header("Camera Bounds (Follow Mode Only)")]
    // If true, constrains camera X position between minX and maxX.
    [SerializeField] private bool useBounds = false;
    
    // Minimum X position for camera when bounds are enabled.
    [SerializeField] private float minX = -10f;
    
    // Maximum X position for camera when bounds are enabled.
    [SerializeField] private float maxX = 10f;

    private Camera cam;

    /// <summary>
    /// Caches camera reference and applies aspect ratio enforcement on initialization.
    /// </summary>
    private void Awake()
    {
        cam = GetComponent<Camera>();
        
        if (cam == null)
        {
            Debug.LogError($"AdventureCameraController on {gameObject.name}: No Camera component found. This script requires a Camera component.", this);
            return;
        }
        
        EnforceAspectRatio();
    }

    /// <summary>
    /// Initializes camera position and finds target if needed.
    /// </summary>
    private void Start()
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
            else
            {
                Debug.LogWarning($"AdventureCameraController on {gameObject.name}: No target assigned and no GameObject with 'Player' tag found. Camera will not follow anything.", this);
            }
        }
    }

    /// <summary>
    /// Updates camera position each frame. Uses LateUpdate to ensure smooth following after player movement.
    /// </summary>
    private void LateUpdate()
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

    /// <summary>
    /// Smoothly follows the target transform with optional bounds clamping.
    /// </summary>
    private void FollowTarget()
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

    /// <summary>
    /// Adds black bars (letterboxing/pillarboxing) to enforce the target aspect ratio.
    /// Preserves the classic film noir look regardless of screen resolution.
    /// </summary>
    private void EnforceAspectRatio()
    {
        if (cam == null) return;

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

    /// <summary>
    /// Sets a new room position. If in room mode, immediately moves camera to the new position.
    /// </summary>
    /// <param name="newPosition">The new room position for the camera.</param>
    public void SetRoomPosition(Vector3 newPosition)
    {
        roomPosition = newPosition;
        
        if (useRoomMode)
        {
            transform.position = roomPosition;
        }
    }

    /// <summary>
    /// Immediately snaps the camera to a new room position without interpolation.
    /// Updates roomPosition and forces camera to that position regardless of mode.
    /// </summary>
    /// <param name="newPosition">The position to snap to.</param>
    public void SnapToRoom(Vector3 newPosition)
    {
        roomPosition = newPosition;
        transform.position = roomPosition;
    }

    /// <summary>
    /// Toggles between room mode (static) and follow mode (tracking target).
    /// </summary>
    /// <param name="follow">If true, enables follow mode. If false, enables room mode.</param>
    public void SetFollowMode(bool follow)
    {
        useRoomMode = !follow;
    }

    /// <summary>
    /// Called when values change in the Inspector. Re-applies aspect ratio enforcement.
    /// </summary>
    private void OnValidate()
    {
        if (cam != null)
        {
            EnforceAspectRatio();
        }
    }

    /// <summary>
    /// Draws camera bounds in the Scene view when this object is selected.
    /// Only visible in follow mode with bounds enabled.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!useBounds || useRoomMode) return;

        Gizmos.color = Color.yellow;
        const float GIZMO_HEIGHT = 10f;
        Vector3 center = new Vector3((minX + maxX) / 2f, roomPosition.y, 0f);
        Vector3 size = new Vector3(maxX - minX, GIZMO_HEIGHT, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
}