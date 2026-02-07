using UnityEngine;

namespace KevinTests.Rooms
{
    /// <summary>
    /// Kevin Tests: room switching using Room and AdventureCameraController. Main game uses Scripts/RoomManager with RoomDefinition.
    /// </summary>
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
}
