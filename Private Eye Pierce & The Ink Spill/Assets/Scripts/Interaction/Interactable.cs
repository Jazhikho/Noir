using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Walk-to interactable used by PointClickController and ClickController2D. Implements IInteractable so ClickController2D can detect and trigger it (e.g. bat in office).
/// </summary>
public class Interactable : MonoBehaviour, IInteractable
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

    /// <summary>
    /// Called by ClickController2D when the player clicks this object. Starts walk to interaction point; OnPlayerArrived is called when the player arrives.
    /// </summary>
    public void OnClick()
    {
        if (playerController == null) return;
        float x = GetInteractionX();
        playerController.SetTargetXForInteractable(x, this);
    }

    /// <summary>
    /// Called when the cursor enters or leaves this interactable. Optional visual feedback can be added.
    /// </summary>
    public void OnHover(bool isHovering)
    {
    }

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

    public void OnPlayerArrived()
    {
        OnInteract.Invoke();
    }

    public void EndInteraction()
    {
        if (playerController != null)
        {
            playerController.EnableMovement();
        }
    }

    public static void EndCurrentInteraction()
    {
        PointClickController player = FindFirstObjectByType<PointClickController>();
        // KEVIN EDIT - fallback for when player GO is inactive (e.g., KeysFoundRevealUI hides Pierce)
        if (player == null)
        {
            foreach (var p in Resources.FindObjectsOfTypeAll<PointClickController>())
            {
                if (p.gameObject.scene.IsValid()) { player = p; break; }
            }
        }
        if (player != null)
        {
            player.EnableMovement();
        }
    }
}