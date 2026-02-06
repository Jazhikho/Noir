using UnityEngine;

public class RoomDefinition : MonoBehaviour
{
    public string roomId;
    public Transform spawnLeft;
    public Transform spawnRight;
    public Transform cameraAnchor;
    public GameObject roomRoot;
    /// <summary>Collider (e.g. Floor's BoxCollider2D) that defines the X clamp range for walking in this room.</summary>
    public Collider2D floorCollider;
    [Tooltip("Camera Z rotation in degrees (0, 90, 180, 270) for rooms that change angle.")]
    [Range(0f, 360f)]
    public float cameraAngleDegrees;

    public Transform GetSpawn(string spawnKey)
    {
        if (spawnKey == "Right") return spawnRight;
        return spawnLeft;
    }
}
