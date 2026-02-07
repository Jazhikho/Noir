using UnityEngine;

namespace KevinTests.Rooms
{
    /// <summary>
    /// Kevin Tests: room switching using RoomDefinition and AdventureCameraController. Single source of truth for room data is RoomDefinition.
    /// </summary>
    public class RoomManager : MonoBehaviour
    {
        public RoomDefinition currentRoom;
        public AdventureCameraController cameraController;

        /// <summary>
        /// Switches the active room: deactivates current room root, activates new room root, updates camera bounds.
        /// </summary>
        public void EnterRoom(RoomDefinition newRoom)
        {
            if (currentRoom != null)
            {
                if (currentRoom.roomRoot != null)
                    currentRoom.roomRoot.SetActive(false);
                if (currentRoom.background != null)
                    currentRoom.background.SetActive(false);
            }

            currentRoom = newRoom;

            if (currentRoom != null)
            {
                if (currentRoom.roomRoot != null)
                    currentRoom.roomRoot.SetActive(true);
                if (currentRoom.background != null)
                    currentRoom.background.SetActive(true);
                BoxCollider2D bounds = currentRoom.GetRoomBounds();
                if (cameraController != null && bounds != null)
                    cameraController.SetRoomBounds(bounds);
            }
        }
    }
}
