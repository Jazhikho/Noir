using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton that manages room transitions: which room is active, spawn positions, and door-based transitions.
/// Assign rooms in the Inspector; use DoorInteractable to trigger transitions when the player reaches a door.
/// </summary>
public class RoomManager : MonoBehaviour
{
    /// <summary>Singleton instance. Set in Awake.</summary>
    public static RoomManager Instance { get; private set; }

    public Transform player;
    public Transform mainCamera;
    public ClickController2D clickController;
    [Tooltip("Used for click-to-move and door transitions. If unset, PointClickController on player is used when present.")]
    public PlayerMover2D playerMover;
    [Tooltip("Pierce controller. If unset and player is set, found via GetComponent on player at runtime. Used for door transition target when playerMover is null.")]
    public PointClickController pierceController;

    [Tooltip("Room ID to enter when the game starts. Pierce spawns at this room's Spawn_Left.")]
    public string initialRoomId = "Pierce";
    [Tooltip("Distance (world units) from door X at which the room transition triggers.")]
    public float doorReachDistance = 0.5f;
    [Tooltip("When spawning at a door, move the player this many units inward so they are not standing on the door. Tune in Inspector.")]
    public float spawnInwardOffset = 0.8f;
    [Tooltip("Optional. When set, room change plays a page-turn animation (direction from spawn: Right = right-to-left, Left = left-to-right).")]
    public PageTurnTransition pageTurnTransition;

    public List<RoomDefinition> rooms = new();

    private Dictionary<string, RoomDefinition> _map;
    private RoomDefinition _current;
    private AdventureCameraController _cameraController;
    private string _pendingRoomId;
    private string _pendingSpawnKey;
    private float _pendingDoorWorldX;
    private string _pendingLeavingRoomId;
    private bool _pendingDoorWasOnLeft;

    /// <summary>
    /// Enforces singleton and builds the room ID map. Deactivates all room roots until EnterRoom is called.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _map = new Dictionary<string, RoomDefinition>();
        foreach (RoomDefinition r in rooms)
        {
            _map[r.roomId] = r;
            if (r.roomRoot != null) r.roomRoot.SetActive(false);
        }
    }

    /// <summary>
    /// Enters the initial room using initialRoomId and the default "Left" spawn.
    /// </summary>
    private void Start()
    {
        EnterRoom(initialRoomId, "Left", null);
    }

    /// <summary>
    /// Resolves pierceController reference: assigned field, then player (including inactive children), then any in scene.
    /// </summary>
    private void ResolvePierceController()
    {
        if (pierceController != null) return;
        if (player != null)
            pierceController = player.GetComponentInChildren<PointClickController>(true);
        if (pierceController == null)
            pierceController = FindFirstObjectByType<PointClickController>(FindObjectsInactive.Include);
    }

    /// <summary>
    /// When a room transition is pending, checks if the player has reached the door; if so, runs the transition (with optional page turn).
    /// </summary>
    private void Update()
    {
        if (string.IsNullOrEmpty(_pendingRoomId) || player == null) return;

        float dist = Mathf.Abs(player.position.x - _pendingDoorWorldX);
        if (dist <= doorReachDistance)
        {
            string roomId = _pendingRoomId;
            string spawnKey = _pendingSpawnKey;
            string leavingRoomId = _pendingLeavingRoomId;
            bool doorWasOnLeft = _pendingDoorWasOnLeft;
            _pendingRoomId = null;
            _pendingSpawnKey = null;
            _pendingLeavingRoomId = null;
            bool rightToLeft = (spawnKey == "Right");
            if (pageTurnTransition != null)
                pageTurnTransition.PlayTransition(rightToLeft, () => EnterRoom(roomId, spawnKey, leavingRoomId, doorWasOnLeft));
            else
                EnterRoom(roomId, spawnKey, leavingRoomId, doorWasOnLeft);
        }
    }

    /// <summary>
    /// Queues a room transition when the player reaches the door. Sets the player movement target to the door position.
    /// </summary>
    /// <param name="roomId">Room ID to transition to (must match a RoomDefinition.roomId in rooms).</param>
    /// <param name="spawnKey">Spawn point in the target room: "Left" or "Right".</param>
    /// <param name="doorTransform">Transform of the door the player is moving to; used for reach distance and side.</param>
    public void PrepareTransitionToDoor(string roomId, string spawnKey, Transform doorTransform)
    {
        if (doorTransform == null)
        {
            Debug.LogError($"[RoomManager] PrepareTransitionToDoor failed: doorTransform is null for roomId '{roomId}'. Assign the door's transform.", this);
            return;
        }
        ResolvePierceController();
        if (playerMover == null && pierceController == null)
        {
            Debug.LogError("[RoomManager] PrepareTransitionToDoor failed: assign Player Mover (PlayerMover2D) or add PointClickController to the Player GameObject and assign Player in RoomManager.", this);
            return;
        }
        _pendingRoomId = roomId;
        _pendingSpawnKey = spawnKey;
        _pendingDoorWorldX = doorTransform.position.x;
        if (_current != null)
            _pendingLeavingRoomId = _current.roomId;
        else
            _pendingLeavingRoomId = null;
        float roomCenterX = GetRoomCenterX(_current);
        _pendingDoorWasOnLeft = _current != null && doorTransform.position.x < roomCenterX;
        if (playerMover != null)
            playerMover.SetTargetX(_pendingDoorWorldX);
        else if (pierceController != null)
            pierceController.SetTargetX(_pendingDoorWorldX);
    }

    /// <summary>
    /// Enters a room: deactivates current room, activates the new one, teleports player to spawn, updates camera and click walk bounds.
    /// On game start use spawnKey only; when coming from a door, leavingRoomId and doorWasOnLeft determine spawn (e.g. left door exit → spawn right).
    /// </summary>
    /// <param name="roomId">Room ID to enter (must exist in rooms).</param>
    /// <param name="spawnKey">"Left" or "Right" spawn in the target room.</param>
    /// <param name="leavingRoomId">When transitioning via door, the room we are leaving; null on initial load.</param>
    /// <param name="doorWasOnLeft">When true, player exited via a door on the left side of the previous room (affects which spawn is used).</param>
    public void EnterRoom(string roomId, string spawnKey, string leavingRoomId = null, bool doorWasOnLeft = false)
    {
        if (!_map.TryGetValue(roomId, out RoomDefinition next))
        {
            Debug.LogError($"[RoomManager] EnterRoom failed: No room defined with roomId '{roomId}'. Check RoomManager.rooms and RoomDefinition.roomId.", this);
            return;
        }

        ApplyRoomSwitch(next);

        Vector3 spawnPosition;
        if (leavingRoomId == null)
        {
            Transform spawn = _current.GetSpawn(spawnKey);
            spawnPosition = SpawnPositionFrom(spawn);
        }
        else
        {
            Transform doorInNewRoom = GetDoorInRoom(_current, leavingRoomId);
            if (doorInNewRoom != null)
            {
                spawnPosition = SpawnPositionFrom(doorInNewRoom);
                float inward = (doorWasOnLeft ? -1f : 1f) * spawnInwardOffset;
                spawnPosition.x += inward;
            }
            else if (roomId == "Room1")
            {
                Transform spawn = _current.GetSpawn("Right");
                spawnPosition = SpawnPositionFrom(spawn);
            }
            else
            {
                string key = doorWasOnLeft ? "Right" : "Left";
                Transform spawn = _current.GetSpawn(key);
                spawnPosition = SpawnPositionFrom(spawn);
            }
        }

        if (player != null)
            player.position = new Vector3(spawnPosition.x, player.position.y, player.position.z);

        ResolvePierceController();
        if (leavingRoomId != null && pierceController != null)
            pierceController.SetFaceLeft(doorWasOnLeft);
        if (playerMover != null)
            playerMover.SetTargetX(player.position.x);
        else if (pierceController != null)
            pierceController.SetTargetX(player.position.x);

        ApplyCameraAndClickBounds();
    }

    /// <summary>
    /// Enters a room by room definition only: switches active room and updates camera/click bounds. Does not move the player.
    /// Used by LockedDoor after it teleports the player.
    /// </summary>
    /// <param name="newRoom">Room to enter (must not be null).</param>
    public void EnterRoom(RoomDefinition newRoom)
    {
        if (newRoom == null)
        {
            Debug.LogError("[RoomManager] EnterRoom(RoomDefinition) failed: newRoom is null.", this);
            return;
        }
        ApplyRoomSwitch(newRoom);
        ApplyCameraAndClickBounds();
    }

    /// <summary>
    /// Deactivates current room root/background, sets _current to next, activates next room root/background.
    /// </summary>
    private void ApplyRoomSwitch(RoomDefinition next)
    {
        if (_current != null)
        {
            if (_current.roomRoot != null)
                _current.roomRoot.SetActive(false);
            if (_current.background != null)
                _current.background.SetActive(false);
        }

        _current = next;

        if (_current != null)
        {
            if (_current.roomRoot != null)
                _current.roomRoot.SetActive(true);
            if (_current.background != null)
                _current.background.SetActive(true);
        }
    }

    /// <summary>
    /// Updates camera bounds (or anchor/angle) and clickController walk bounds for _current room.
    /// </summary>
    private void ApplyCameraAndClickBounds()
    {
        if (mainCamera != null)
        {
            if (_cameraController == null)
                _cameraController = mainCamera.GetComponent<AdventureCameraController>();

            BoxCollider2D bounds = _current != null ? _current.GetRoomBounds() : null;
            if (_cameraController != null && bounds != null)
            {
                _cameraController.SetRoomBounds(bounds);
            }
            else if (_current != null && _current.cameraAnchor != null)
            {
                mainCamera.position = new Vector3(_current.cameraAnchor.position.x, _current.cameraAnchor.position.y, mainCamera.position.z);
                mainCamera.eulerAngles = new Vector3(0f, 0f, _current.cameraAngleDegrees);
            }
        }

        if (clickController != null && _current != null && _current.floorCollider != null)
            clickController.walkBounds = _current.floorCollider;
    }

    /// <summary>
    /// Computes world spawn position from a spawn transform, preserving player Y and Z when available.
    /// </summary>
    /// <param name="t">Spawn transform; if null, returns player position or zero.</param>
    /// <returns>World position for the player spawn.</returns>
    private Vector3 SpawnPositionFrom(Transform t)
    {
        if (t == null)
        {
            Debug.LogWarning("[RoomManager] SpawnPositionFrom called with null transform; using player position or zero.", this);
            if (player != null) return player.position;
            return Vector3.zero;
        }
        float y;
        if (player != null)
            y = player.position.y;
        else
            y = t.position.y;
        float z;
        if (player != null)
            z = player.position.z;
        else
            z = 0f;
        return new Vector3(t.position.x, y, z);
    }

    /// <summary>
    /// Returns the approximate center X of a room (from floor collider or camera anchor).
    /// </summary>
    /// <param name="room">Room definition; may be null.</param>
    /// <returns>Center X or 0 if room is null or has no floor/anchor.</returns>
    private float GetRoomCenterX(RoomDefinition room)
    {
        if (room == null) return 0f;
        if (room.floorCollider != null) return room.floorCollider.bounds.center.x;
        if (room.cameraAnchor != null) return room.cameraAnchor.position.x;
        return 0f;
    }

    /// <summary>
    /// Finds the first door in the room that targets the given room ID.
    /// </summary>
    /// <param name="room">Room to search (roomRoot and children).</param>
    /// <param name="targetRoomId">DoorInteractable.targetRoomId to match.</param>
    /// <returns>The door's transform, or null if not found.</returns>
    private static Transform GetDoorInRoom(RoomDefinition room, string targetRoomId)
    {
        if (room?.roomRoot == null) return null;
        foreach (DoorInteractable door in room.roomRoot.GetComponentsInChildren<DoorInteractable>(true))
        {
            if (door.targetRoomId == targetRoomId) return door.transform;
        }
        return null;
    }
}
