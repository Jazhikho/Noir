using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    public string targetRoomId;
    public string targetSpawn = "Left"; // "Left" or "Right"

    public void OnClick()
    {
        RoomManager.Instance.PrepareTransitionToDoor(targetRoomId, targetSpawn, transform);
    }

    public void OnHover(bool isHovering)
    {
        // Optional: change sprite tint, show outline, swap cursor, etc.
    }
}
