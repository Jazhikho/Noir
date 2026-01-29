using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    public Transform player;
    public Transform mainCamera;
    public ClickController2D clickController;
    public PlayerMover2D playerMover;

    [Tooltip("Room ID to enter when the game starts. Pierce spawns at this room's Spawn_Left.")]
    public string initialRoomId = "Pierce";
    [Tooltip("Distance (world units) from door X at which the room transition triggers.")]
    public float doorReachDistance = 0.5f;
    [Tooltip("Optional. When set, room change plays a page-turn animation (direction from spawn: Right = right-to-left, Left = left-to-right).")]
    public PageTurnTransition pageTurnTransition;

    public List<RoomDefinition> rooms = new();

    private Dictionary<string, RoomDefinition> _map;
    private RoomDefinition _current;
    private string _pendingRoomId;
    private string _pendingSpawnKey;
    private float _pendingDoorWorldX;
    private string _pendingLeavingRoomId;
    private bool _pendingDoorWasOnLeft;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _map = new Dictionary<string, RoomDefinition>();
        foreach (var r in rooms)
        {
            _map[r.roomId] = r;
            if (r.roomRoot != null) r.roomRoot.SetActive(false);
        }
    }

    private void Start()
    {
        EnterRoom(initialRoomId, "Left", null);
    }

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

    /// <summary>Queue a room transition when the player reaches the door. Sets movement target to the door.</summary>
    public void PrepareTransitionToDoor(string roomId, string spawnKey, Transform doorTransform)
    {
        if (doorTransform == null || playerMover == null) return;
        _pendingRoomId = roomId;
        _pendingSpawnKey = spawnKey;
        _pendingDoorWorldX = doorTransform.position.x;
        _pendingLeavingRoomId = _current != null ? _current.roomId : null;
        float roomCenterX = GetRoomCenterX(_current);
        _pendingDoorWasOnLeft = _current != null && doorTransform.position.x < roomCenterX;
        playerMover.SetTargetX(_pendingDoorWorldX);
    }

    /// <summary>Enter a room and teleport the player. Game start: spawnKey only (use Spawn_Left). Via door: leavingRoomId and doorWasOnLeft drive spawn (left door exit → spawn right; exceptions for Room1 and Hallway-from-Room1).</summary>
    public void EnterRoom(string roomId, string spawnKey, string leavingRoomId = null, bool doorWasOnLeft = false)
    {
        if (!_map.TryGetValue(roomId, out var next)) return;

        if (_current != null && _current.roomRoot != null)
            _current.roomRoot.SetActive(false);

        _current = next;

        if (_current.roomRoot != null)
            _current.roomRoot.SetActive(true);

        Vector3 spawnPosition;
        if (leavingRoomId == null)
        {
            Transform spawn = _current.GetSpawn(spawnKey);
            spawnPosition = SpawnPositionFrom(spawn);
        }
        else if (roomId == "Room1")
        {
            Transform spawn = _current.GetSpawn("Right");
            spawnPosition = SpawnPositionFrom(spawn);
        }
        else if (roomId == "Hallway" && leavingRoomId == "Room1")
        {
            Transform doorInNewRoom = GetDoorInRoom(_current, "Room1");
            spawnPosition = doorInNewRoom != null ? SpawnPositionFrom(doorInNewRoom) : SpawnPositionFrom(_current.GetSpawn("Left"));
        }
        else
        {
            string key = doorWasOnLeft ? "Right" : "Left";
            Transform spawn = _current.GetSpawn(key);
            spawnPosition = SpawnPositionFrom(spawn);
        }

        if (player != null)
            player.position = new Vector3(spawnPosition.x, player.position.y, player.position.z);

        if (playerMover != null)
            playerMover.SetTargetX(player.position.x);

        if (_current.cameraAnchor != null && mainCamera != null)
        {
            mainCamera.position = new Vector3(_current.cameraAnchor.position.x, _current.cameraAnchor.position.y, mainCamera.position.z);
            mainCamera.eulerAngles = new Vector3(0f, 0f, _current.cameraAngleDegrees);
        }

        if (clickController != null && _current.floorCollider != null)
            clickController.walkBounds = _current.floorCollider;
    }

    private Vector3 SpawnPositionFrom(Transform t)
    {
        if (t == null) return player != null ? player.position : Vector3.zero;
        float y = player != null ? player.position.y : t.position.y;
        float z = player != null ? player.position.z : 0f;
        return new Vector3(t.position.x, y, z);
    }

    private float GetRoomCenterX(RoomDefinition room)
    {
        if (room == null) return 0f;
        if (room.floorCollider != null) return room.floorCollider.bounds.center.x;
        if (room.cameraAnchor != null) return room.cameraAnchor.position.x;
        return 0f;
    }

    private static Transform GetDoorInRoom(RoomDefinition room, string targetRoomId)
    {
        if (room?.roomRoot == null) return null;
        foreach (var door in room.roomRoot.GetComponentsInChildren<DoorInteractable>(true))
        {
            if (door.targetRoomId == targetRoomId) return door.transform;
        }
        return null;
    }
}
