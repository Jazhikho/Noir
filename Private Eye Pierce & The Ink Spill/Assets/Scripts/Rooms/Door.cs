using UnityEngine;
using System.Collections.Generic;

public class Door : MonoBehaviour
{
    public RoomDefinition targetRoom;
    public string targetEntryPointName = "Left";

    [SerializeField] private float teleportCooldown = 0.25f;
    private static readonly Dictionary<int, float> nextAllowed = new();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || targetRoom == null) return;
        if (RoomManager.Instance == null)
        {
            Debug.LogError("[Door] RoomManager.Instance is null. Add a RoomManager to the scene.", this);
            return;
        }

        int key = other.GetInstanceID();
        float now = Time.time;
        if (nextAllowed.TryGetValue(key, out float gate) && now < gate) return;
        nextAllowed[key] = now + teleportCooldown;

        Transform spawnPoint = targetRoom.GetEntryPoint(targetEntryPointName);
        if (spawnPoint == null)
        {
            Debug.LogWarning("Spawn point '" + targetEntryPointName + "' not found in room '" + targetRoom.roomId + "'");
            return;
        }

        var rb = other.attachedRigidbody;
        if (rb != null)
            rb.position = spawnPoint.position;
        else
            other.transform.position = spawnPoint.position;

        RoomManager.Instance.EnterRoom(targetRoom);
    }
}
