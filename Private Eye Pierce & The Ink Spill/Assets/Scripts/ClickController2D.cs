using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// Handles click-to-move and interaction in 2D. Converts screen clicks to world position, clamps to walk bounds,
/// and notifies hovered IInteractable (e.g. doors, WalkToInteractable) on click. Drives PointClickController (Pierce) when assigned.
/// </summary>
public class ClickController2D : MonoBehaviour
{
    /// <summary>
    /// Layer mask for interactable colliders (doors, walk-to objects).
    /// </summary>
    public LayerMask interactableMask;

    /// <summary>
    /// Camera used for screen-to-world. If unset, falls back to Camera.main in Start.
    /// </summary>
    public Camera cam;

    /// <summary>
    /// Pierce (player) controller that receives click-to-move targets. If unset, finds PointClickController in scene at Start.
    /// </summary>
    public PointClickController pierceController;

    /// <summary>
    /// Current room's floor collider; clamps walk target X to this bounds.
    /// </summary>
    public Collider2D walkBounds;

    /// <summary>
    /// Optional. When set, cursor switches to Squawk "use" icon when hovering doors.
    /// </summary>
    public CursorController cursorController;

    private IInteractable _hovered;

    /// <summary>
    /// Caches camera and pierce controller. Uses Camera.main and FindFirstObjectByType PointClickController if not assigned.
    /// </summary>
    private void Start()
    {
        if (cam == null)
            cam = Camera.main;
        if (pierceController == null)
            pierceController = FindFirstObjectByType<PointClickController>();
        if (pierceController != null)
            pierceController.inputHandledExternally = true;
    }

    /// <summary>
    /// Handles hover state, cursor changes, and left-click: either triggers the hovered interactable or sets mover target X.
    /// </summary>
    private void Update()
    {
        if (cam == null) return;

        // Ignore clicks over UI (prevents weirdness later when you add dialogue UI)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Vector2 world = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Hover check for interactables
        Collider2D hitHover = Physics2D.OverlapPoint(world, interactableMask);
        IInteractable newHover = null;
        if (hitHover != null)
            newHover = hitHover.GetComponent<IInteractable>();

        if (_hovered != newHover)
        {
            _hovered?.OnHover(false);
            _hovered = newHover;
            _hovered?.OnHover(true);
            if (cursorController != null)
            {
                if (newHover is DoorInteractable)
                    cursorController.ChangeCursor(CursorController.CursorName.use);
                else
                    cursorController.ChangeCursor(CursorController.CursorName.main);
            }
        }

        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        // If we clicked an interactable, do that instead of moving
        if (_hovered != null)
        {
            _hovered.OnClick();
            return;
        }

        // Otherwise: click-anywhere move (X only), clamped to walk bounds
        float targetX = world.x;

        if (walkBounds != null)
        {
            Bounds b = walkBounds.bounds;
            targetX = Mathf.Clamp(targetX, b.min.x, b.max.x);
        }

        if (pierceController != null)
            pierceController.SetTargetX(targetX);
    }
}
