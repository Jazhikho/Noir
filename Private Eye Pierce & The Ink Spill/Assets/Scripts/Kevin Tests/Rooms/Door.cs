using UnityEngine;
using System.Collections.Generic;

public class Door : MonoBehaviour
{
    public Room targetRoom;
    public string targetEntryPointName = "Left";
    public KevinTests.Rooms.RoomManager roomManager;

    [SerializeField] private float teleportCooldown = 0.25f;
    private static readonly Dictionary<int, float> nextAllowed = new();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && roomManager != null && targetRoom != null)
        {
            int key = other.GetInstanceID();
            float now = Time.time;
            if (nextAllowed.TryGetValue(key, out float gate) && now < gate) return;
            nextAllowed[key] = now + teleportCooldown;

            Transform spawnPoint = targetRoom.GetEntryPoint(targetEntryPointName);
            if (spawnPoint == null)
            {
                Debug.LogWarning("Spawn point '" + targetEntryPointName + "' not found in room '" + targetRoom.roomID + "'");
                return;
            }

            var rb = other.attachedRigidbody;
            if (rb != null)
                rb.position = spawnPoint.position;
            else
                other.transform.position = spawnPoint.position;

            roomManager.EnterRoom(targetRoom);
        }
    }
}
