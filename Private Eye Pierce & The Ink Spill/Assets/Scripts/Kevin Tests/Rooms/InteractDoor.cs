using UnityEngine;
using System.Collections.Generic;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class InteractDoor : MonoBehaviour
{
    [Header("Destination")]
    public Room targetRoom;
    public string targetEntryPointName = "Left";
    public KevinTests.Rooms.RoomManager roomManager;

    [Header("UI")]
    public GameObject glowEffect;  // Optional: assign a glow GameObject to show when player nearby

    [Header("Trigger Safety")]
    [Tooltip("Ignore door triggers for this long after teleport (seconds).")]
    public float teleportCooldown = 0.25f;

    // Per-player cooldown map (key = collider instance id)
    private static readonly Dictionary<int, float> nextAllowedTeleport = new();

    [Header("Exit / Clearance")]
    public bool useSpawnRightAsDirection = true;
    public Vector2 exitDirection = Vector2.right;
    public float exitClearance = 0.35f;
    public float exitLaunchSpeed = 6.5f;

    [Header("Ground Snap (optional)")]
    public LayerMask groundMask;
    public float groundSnapMaxDistance = 2.0f;
    public float groundPadding = 0.02f;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
    [Header("Input System (New)")]
    [Tooltip("Optional. If assigned, this action will be used for interact (performed on press).")]
    public InputActionReference interactAction;
#endif

    private bool playerInRange = false;
    private Collider2D cachedPlayerCol;

    private void OnEnable()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (interactAction != null) interactAction.action.Enable();
#endif
    }

    private void OnDisable()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (interactAction != null) interactAction.action.Disable();
#endif
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            cachedPlayerCol = other;
            if (glowEffect != null) glowEffect.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            cachedPlayerCol = null;
            if (glowEffect != null) glowEffect.SetActive(false);
        }
    }

    private void Update()
    {
        bool pressed = WasInteractPressed();
        if (pressed && playerInRange && cachedPlayerCol != null)
            TryTeleport(cachedPlayerCol);
    }

    private bool WasInteractPressed()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        // New Input System path
        if (interactAction != null)
        {
            return interactAction.action.WasPressedThisFrame();
        }
        else
        {
            // Fallback to common keys/buttons if no action is wired
            var kb = Keyboard.current;
            var gp = Gamepad.current;
            bool p = false;
            if (kb != null)
            {
                p |= kb.eKey.wasPressedThisFrame;
                p |= kb.enterKey.wasPressedThisFrame;
                p |= kb.spaceKey.wasPressedThisFrame;
                p |= kb.upArrowKey.wasPressedThisFrame;
            }
            if (gp != null)
            {
                p |= gp.buttonSouth.wasPressedThisFrame; // A/Cross
                p |= gp.startButton.wasPressedThisFrame;
            }
            return p;
        }
#else
        // Legacy Input Manager path
        return Input.GetButtonDown("Submit") || Input.GetKeyDown(KeyCode.E);
#endif
    }

    private void TryTeleport(Collider2D other)
    {
        if (roomManager == null || targetRoom == null) return;

        int key = other.GetInstanceID();
        float now = Time.time;
        if (nextAllowedTeleport.TryGetValue(key, out float gate) && now < gate) return;
        nextAllowedTeleport[key] = now + teleportCooldown;

        // Resolve spawn first
        Transform spawn = targetRoom.GetEntryPoint(targetEntryPointName);
        if (spawn == null)
        {
            Debug.LogWarning($"Spawn point '{targetEntryPointName}' not found in room '{targetRoom.roomID}'");
            return;
        }

        // Output a clear destination
        var rb = other.attachedRigidbody;
        var col = other.GetComponent<Collider2D>();

        Vector2 dir = useSpawnRightAsDirection ? (Vector2)spawn.right : exitDirection;
        if (dir.sqrMagnitude < 1e-6f) dir = Vector2.right;
        dir.Normalize();

        Vector3 dest = spawn.position;
        float forwardExtent = 0f;
        if (col != null)
        {
            Bounds b = col.bounds;
            Vector2 half = b.extents;
            forwardExtent = Mathf.Abs(half.x * dir.x) + Mathf.Abs(half.y * dir.y);
        }
        dest += (Vector3)(dir * (forwardExtent + exitClearance));

        if (groundMask.value != 0 && col != null)
        {
            float halfH = col.bounds.extents.y;
            Vector2 rayOrigin = new Vector2(dest.x, dest.y + halfH + 0.1f);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, halfH + groundSnapMaxDistance, groundMask);
            if (hit.collider != null)
                dest.y = hit.point.y + halfH + groundPadding;
        }

        // Move first
        if (rb != null)
        {
            rb.position = dest;
            rb.linearVelocity = dir * exitLaunchSpeed;
            rb.angularVelocity = 0f;
        }
        else
        {
            other.transform.position = dest;
        }

        Physics2D.SyncTransforms();

        // Then swap rooms (updates camera bounds)
        roomManager.EnterRoom(targetRoom);
    }
}
