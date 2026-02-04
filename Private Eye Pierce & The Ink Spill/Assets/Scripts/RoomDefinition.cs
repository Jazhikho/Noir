using UnityEngine;

/// <summary>
/// Defines one room: ID, spawn points, camera anchor, root GameObject, floor collider for walk bounds, and optional camera angle.
/// </summary>
public class RoomDefinition : MonoBehaviour
{
    /// <summary>Unique ID for this room. Must match references in DoorInteractable and RoomManager.initialRoomId.</summary>
    public string roomId;

    /// <summary>Transform where the player spawns when entering from the left (e.g. left door exit).</summary>
    public Transform spawnLeft;

    /// <summary>Transform where the player spawns when entering from the right (e.g. right door exit).</summary>
    public Transform spawnRight;

    /// <summary>Optional. Camera position and rotation are set to this when entering the room.</summary>
    public Transform cameraAnchor;

    /// <summary>Root GameObject for this room; activated/deactivated by RoomManager when entering or leaving.</summary>
    public GameObject roomRoot;

    /// <summary>Collider (e.g. Floor's BoxCollider2D) that defines the X clamp range for walking in this room.</summary>
    public Collider2D floorCollider;

    /// <summary>Camera Z rotation in degrees (0, 90, 180, 270) for rooms that change angle. Applied when entering this room.</summary>
    [Tooltip("Camera Z rotation in degrees (0, 90, 180, 270) for rooms that change angle.")]
    [Range(0f, 360f)]
    public float cameraAngleDegrees;

    /// <summary>
    /// Returns the spawn transform for the given key.
    /// </summary>
    /// <param name="spawnKey">"Right" for spawnRight, anything else for spawnLeft.</param>
    /// <returns>The spawn transform, or null if not assigned.</returns>
    public Transform GetSpawn(string spawnKey)
    {
        if (spawnKey == "Right") return spawnRight;
        return spawnLeft;
    }
}
