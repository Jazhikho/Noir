using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Interaction Position")]
    public Transform interactionPoint;
    public float defaultOffset = 1f;

    [Header("Events")]
    public UnityEvent OnInteract;

    private PointClickController playerController;

    void Start()
    {
        playerController = FindFirstObjectByType<PointClickController>();
    }

    // Returns the X position where the player should stand to interact
    public float GetInteractionX()
    {
        if (interactionPoint != null)
        {
            return interactionPoint.position.x;
        }

        if (playerController != null)
        {
            float playerX = playerController.transform.position.x;
            float objectX = transform.position.x;

            if (playerX < objectX)
            {
                return objectX - defaultOffset;
            }
            else
            {
                return objectX + defaultOffset;
            }
        }

        return transform.position.x - defaultOffset;
    }

    // Called by PointClickController when the player arrives
    public void OnPlayerArrived()
    {
        OnInteract.Invoke();
    }

    // Call this when the interaction is finished to re-enable movement
    public void EndInteraction()
    {
        if (playerController != null)
        {
            playerController.EnableMovement();
        }
    }

    // Static helper to end interaction from anywhere
    public static void EndCurrentInteraction()
    {
        PointClickController player = FindFirstObjectByType<PointClickController>();
        if (player != null)
        {
            player.EnableMovement();
        }
    }
}
