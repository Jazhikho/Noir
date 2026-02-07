using UnityEngine;

/// <summary>
/// Defines one room: ID, spawn/entry points, camera anchor, root GameObject, floor collider for walk bounds,
/// optional camera bounds, and optional camera angle. Used by both root RoomManager and Kevin's RoomManager/LockedDoor/InteractDoor.
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

    /// <summary>Optional. Background graphic for this room; shown when entering, hidden when leaving. Can be a child of roomRoot or a separate GameObject.</summary>
    public GameObject background;

    /// <summary>Collider (e.g. Floor's BoxCollider2D) that defines the X clamp range for walking in this room.</summary>
    public Collider2D floorCollider;

    /// <summary>Optional. BoxCollider2D used for camera bounds (e.g. AdventureCameraController). If null, floorCollider is used when it is a BoxCollider2D.</summary>
    public BoxCollider2D roomBounds;

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

    /// <summary>
    /// Returns the entry point transform for the given entrance name. Used by LockedDoor, InteractDoor, and Door. Only Left and Right are used for this 2D side-scroller.
    /// </summary>
    /// <param name="entranceName">"Right" for spawnRight, anything else returns spawnLeft.</param>
    /// <returns>The entry point transform, or null if not assigned.</returns>
    public Transform GetEntryPoint(string entranceName)
    {
        if (entranceName == "Right") return spawnRight;
        return spawnLeft;
    }

    /// <summary>
    /// Returns the BoxCollider2D to use for camera bounds. Prefers roomBounds; falls back to floorCollider if it is a BoxCollider2D.
    /// </summary>
    public BoxCollider2D GetRoomBounds()
    {
        if (roomBounds != null) return roomBounds;
        return floorCollider as BoxCollider2D;
    }
}
