using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public Room currentRoom;
    public AdventureCameraController cameraController;

    public void EnterRoom(Room newRoom)
    {
        if (currentRoom != null)
            currentRoom.gameObject.SetActive(false);

        currentRoom = newRoom;

        if (currentRoom != null)
        {
            currentRoom.gameObject.SetActive(true);
            if (cameraController != null && currentRoom.roomBounds != null)
                cameraController.SetRoomBounds(currentRoom.roomBounds);
        }
    }
}
