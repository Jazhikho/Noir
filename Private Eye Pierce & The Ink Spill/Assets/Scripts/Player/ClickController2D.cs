using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// Handles click-to-move and interaction in 2D. Converts screen clicks to world position, clamps to walk bounds,
/// and notifies hovered IInteractable (e.g. Interactable, DoorInteractable) on click. Drives PointClickController (Pierce) when assigned.
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
    /// World units to inset walk bounds from left and right edges. Reduces how close Pierce can walk to objects at room edges.
    /// Tune in Inspector (e.g. 0.5–1) if movement feels tight or Pierce clips past props.
    /// </summary>
    [Tooltip("World units to inset from each side of walk bounds. Helps avoid getting stuck past objects.")]
    public float walkBoundsInsetX = 0.5f;

    /// <summary>
    /// Optional. When set, cursor switches to "use" when hovering any IInteractable (via AdventureHUDController.SetCursorType).
    /// </summary>
    public AdventureHUDController adventureHUDController;

    private IInteractable _hovered;
    private GameObject _hoveredObject;

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
        if (UIController.IsPaused) return;

        Vector2 screenPos = Mouse.current != null ? Mouse.current.position.ReadValue() : (Vector2)Input.mousePosition;
        Vector2 world = cam.ScreenToWorldPoint(screenPos);

        // Hover check for interactables first (so cursor and clicks work even when UI is under the pointer)
        Collider2D hitHover = Physics2D.OverlapPoint(world, interactableMask);
        IInteractable newHover = null;
        if (hitHover != null)
            newHover = hitHover.GetComponent<IInteractable>();

        if (_hovered != newHover)
        {
            if (_hoveredObject != null)
            {
                var prevFeedback = _hoveredObject.GetComponentInParent<HoverFeedback>();
                if (prevFeedback != null)
                    prevFeedback.NotifyHover(false);
            }

            _hovered?.OnHover(false);
            _hovered = newHover;
            _hovered?.OnHover(true);

            _hoveredObject = hitHover != null ? hitHover.gameObject : null;
            if (_hoveredObject != null)
            {
                var feedback = _hoveredObject.GetComponentInParent<HoverFeedback>();
                if (feedback != null)
                    feedback.NotifyHover(true);
            }

            if (adventureHUDController != null)
            {
                if (newHover == null)
                    adventureHUDController.SetCursorType(AdventureHUDController.CursorType.Main);
                else if (newHover is DoorInteractable door)
                    adventureHUDController.SetCursorTypeForDoor(door.doorDirection);
                else
                    adventureHUDController.SetCursorType(AdventureHUDController.CursorType.Interactable);
            }
        }

        bool clicked = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        if (clicked && _hovered != null)
        {
            _hovered.OnClick();
            return;
        }

        if (clicked && IsPointerOverBlockingUI(screenPos))
            return;

        if (clicked)
        {
            float targetX = world.x;
            if (walkBounds != null)
            {
                Bounds b = walkBounds.bounds;
                float inset = Mathf.Max(0f, walkBoundsInsetX);
                float minX = b.min.x + inset;
                float maxX = b.max.x - inset;
                if (minX <= maxX)
                    targetX = Mathf.Clamp(targetX, minX, maxX);
                else
                    targetX = Mathf.Clamp(targetX, b.min.x, b.max.x);
            }
            if (pierceController != null)
                pierceController.SetTargetX(targetX);
        }
    }

    /// <summary>
    /// Returns true if the given screen position is over UI that blocks raycasts. Uses the same position source as our click so world clicks are not wrongly ignored. Ensure the HUD cursor image has Raycast Target disabled so it does not block.
    /// </summary>
    private bool IsPointerOverBlockingUI(Vector2 screenPosition)
    {
        if (EventSystem.current == null) return false;

        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = screenPosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count > 0;
    }
}
