using UnityEngine;

/// <summary>
/// Door that triggers a room transition when clicked. Implements IInteractable for ClickController2D.
/// </summary>
public class DoorInteractable : MonoBehaviour, IInteractable
{
    /// <summary>
    /// Room ID to transition to (must match a RoomDefinition.roomId in RoomManager).
    /// </summary>
    public string targetRoomId;

    /// <summary>
    /// Spawn point in the target room: "Left" or "Right".
    /// </summary>
    public string targetSpawn = "Left";

    /// <summary>
    /// Optional. GameObject to show when the cursor hovers this door (e.g. glow). Shown/hidden in OnHover.
    /// </summary>
    public GameObject glowEffect;

    /// <summary>
    /// Starts a room transition to targetRoomId when the player reaches this door. Sets the player's walk target to this door (using the same controller as ClickController2D) so they move there; RoomManager triggers the transition on arrival.
    /// </summary>
    public void OnClick()
    {
        if (RoomManager.Instance == null)
        {
            Debug.LogError("[DoorInteractable] RoomManager.Instance is null. Is there a RoomManager in the scene with no duplicate?", this);
            return;
        }

        PointClickController pierce = UnityEngine.Object.FindFirstObjectByType<PointClickController>();
        if (pierce != null)
        {
            pierce.SetTargetX(transform.position.x);
        }

        RoomManager.Instance.PrepareTransitionToDoor(targetRoomId, targetSpawn, transform);
    }

    /// <summary>
    /// Called when the cursor hovers over or leaves this door. Toggles optional glowEffect if assigned.
    /// </summary>
    /// <param name="isHovering">True when hovering, false when leaving.</param>
    public void OnHover(bool isHovering)
    {
        if (glowEffect != null)
        {
            glowEffect.SetActive(isHovering);
        }
    }
}
