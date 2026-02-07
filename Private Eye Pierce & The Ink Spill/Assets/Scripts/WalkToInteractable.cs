using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Interactable object that makes the player walk to a specific position before triggering interaction.
/// Implements IInteractable to work with ClickController2D.
/// </summary>
public class WalkToInteractable : MonoBehaviour, IInteractable
{
    [Header("Interaction Position")]
    // The specific point where the player should stand to interact. If null, uses defaultOffset.
    [SerializeField] private Transform interactionPoint;
    
    // Distance from this object where the player should stand if no interaction point is set.
    [SerializeField] private float defaultOffset = 1f;

    [Header("Events")]
    // Event triggered when the player reaches the interaction point and clicks.
    public UnityEvent OnInteract;

    private PlayerMover2D playerMover;
    private bool isWaitingForPlayer = false;
    private float targetInteractionX;

    /// <summary>
    /// Caches the PlayerMover2D reference on start.
    /// </summary>
    private void Start()
    {
        playerMover = FindFirstObjectByType<PlayerMover2D>();
        
        if (playerMover == null)
        {
            Debug.LogError($"WalkToInteractable on {gameObject.name}: No PlayerMover2D found in scene. This component requires a PlayerMover2D to function.", this);
        }
    }

    /// <summary>
    /// Called when the player clicks this interactable. Calculates interaction position and moves player there.
    /// </summary>
    public void OnClick()
    {
        if (playerMover == null)
        {
            Debug.LogError($"WalkToInteractable on {gameObject.name}: PlayerMover2D is null. Cannot move player.", this);
            return;
        }

        targetInteractionX = GetInteractionX();
        playerMover.SetTargetX(targetInteractionX);
        isWaitingForPlayer = true;
    }

    /// <summary>
    /// Called when the mouse hovers over or leaves this interactable.
    /// </summary>
    /// <param name="isHovering">True when mouse enters, false when it leaves.</param>
    public void OnHover(bool isHovering)
    {
        // Optional: Add visual feedback like sprite tint, outline, etc.
    }

    /// <summary>
    /// Checks each frame if the player has arrived at the interaction point.
    /// </summary>
    private void Update()
    {
        if (!isWaitingForPlayer) return;
        if (playerMover == null) return;

        // Check if player has reached the interaction point
        float distanceToTarget = Mathf.Abs(playerMover.transform.position.x - targetInteractionX);
        
        if (distanceToTarget <= playerMover.StopDistance)
        {
            isWaitingForPlayer = false;
            OnPlayerArrived();
        }
    }

    /// <summary>
    /// Returns the X position where the player should stand to interact with this object.
    /// Uses interactionPoint if set, otherwise calculates based on player's current position and defaultOffset.
    /// </summary>
    /// <returns>The X coordinate where the player should move to.</returns>
    private float GetInteractionX()
    {
        if (interactionPoint != null)
        {
            return interactionPoint.position.x;
        }

        if (playerMover != null)
        {
            float playerX = playerMover.transform.position.x;
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

    /// <summary>
    /// Called when the player arrives at the interaction point. Triggers the OnInteract event.
    /// </summary>
    private void OnPlayerArrived()
    {
        OnInteract.Invoke();
    }

    /// <summary>
    /// Draws the interaction point in the Scene view for debugging.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (interactionPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(interactionPoint.position, 0.3f);
        }
        else
        {
            // Show both possible interaction positions
            Gizmos.color = Color.yellow;
            Vector3 leftPos = transform.position + Vector3.left * defaultOffset;
            Vector3 rightPos = transform.position + Vector3.right * defaultOffset;
            Gizmos.DrawWireSphere(leftPos, 0.2f);
            Gizmos.DrawWireSphere(rightPos, 0.2f);
        }
    }
}
