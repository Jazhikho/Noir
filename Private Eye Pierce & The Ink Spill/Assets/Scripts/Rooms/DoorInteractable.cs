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
    /// Starts a room transition to targetRoomId when the player reaches this door.
    /// </summary>
    public void OnClick()
    {
        if (RoomManager.Instance == null)
        {
            Debug.LogError("[DoorInteractable] RoomManager.Instance is null. Is there a RoomManager in the scene with no duplicate?", this);
            return;
        }
        RoomManager.Instance.PrepareTransitionToDoor(targetRoomId, targetSpawn, transform);
    }

    /// <summary>
    /// Called when the cursor hovers over or leaves this door. Optional visual feedback can be added.
    /// </summary>
    /// <param name="isHovering">True when hovering, false when leaving.</param>
    public void OnHover(bool isHovering)
    {
    }
}
