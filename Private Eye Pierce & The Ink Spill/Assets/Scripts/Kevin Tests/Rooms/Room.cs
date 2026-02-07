using UnityEngine;

public class Room : MonoBehaviour
{
    public string roomID;

    public BoxCollider2D roomBounds;  // assign in inspector, covers whole room

    public Transform entryPointLeft;
    public Transform entryPointRight;
    public Transform entryPointTop;
    public Transform entryPointBottom;
    public Transform outsideInteractDoor;

    // Return the correct entry point by name
    public Transform GetEntryPoint(string entranceName)
    {
        switch (entranceName)
        {
            case "Left": return entryPointLeft;
            case "Right": return entryPointRight;
            case "Top": return entryPointTop;
            case "Bottom": return entryPointBottom;
            case "Exit": return outsideInteractDoor;
            default: return entryPointLeft; // fallback
        }
    }

    // Forward SetActive to the GameObject
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}