using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ClickController2D : MonoBehaviour
{
    public LayerMask interactableMask;

    public Camera cam;
    public PlayerMover2D mover;

    // Assign this to your current room's Floor collider (BoxCollider2D is fine)
    public Collider2D walkBounds;
    /// <summary>Optional. When set, cursor switches to Squawk "use" icon when hovering doors (Squawk/Graphics/UI cursor textures).</summary>
    public CursorController cursorController;

    private IInteractable _hovered;

    private void Update()
    {
        if (cam == null) cam = Camera.main;

        // Ignore clicks over UI (prevents weirdness later when you add dialogue UI)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Vector2 world = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Hover check for interactables
        var hitHover = Physics2D.OverlapPoint(world, interactableMask);
        var newHover = hitHover ? hitHover.GetComponent<IInteractable>() : null;

        if (_hovered != newHover)
        {
            _hovered?.OnHover(false);
            _hovered = newHover;
            _hovered?.OnHover(true);
            if (cursorController != null)
                cursorController.ChangeCursor(newHover is DoorInteractable ? CursorController.CursorName.use : CursorController.CursorName.main);
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
            var b = walkBounds.bounds;
            targetX = Mathf.Clamp(targetX, b.min.x, b.max.x);
        }

        mover.SetTargetX(targetX);
    }
}
