using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages a simple tutorial puzzle where the player must find a bat to break a locked door.
/// Demonstrates basic puzzle state management and UnityEvent integration.
/// </summary>
public class TutorialPuzzle : MonoBehaviour
{
    [Header("Object References")]
    // The baseball bat GameObject that the player can pick up.
    [SerializeField] private GameObject bat;
    
    // The door GameObject that requires the bat to open.
    [SerializeField] private GameObject door;

    [Header("Door Sprite")]
    // Sprite to display when the door window is broken.
    [SerializeField] private Sprite brokenDoorSprite;

    [Header("Bat Events")]
    // Event triggered when the player picks up the bat.
    public UnityEvent OnBatPickedUp;

    [Header("Door Events")]
    // Event triggered when the player first tries the locked door.
    public UnityEvent OnDoorLocked;
    
    // Event triggered when the player tries the door without the bat.
    public UnityEvent OnNeedBat;
    
    // Event triggered when the player successfully opens the door with the bat.
    public UnityEvent OnDoorOpened;

    private bool hasBat = false;
    private bool doorTriedOnce = false;
    private bool doorOpened = false;

    /// <summary>
    /// Validates that required references are assigned in the Inspector.
    /// </summary>
    private void Start()
    {
        if (bat == null)
        {
            Debug.LogError($"TutorialPuzzle on {gameObject.name}: Bat GameObject is not assigned.", this);
        }
        
        if (door == null)
        {
            Debug.LogError($"TutorialPuzzle on {gameObject.name}: Door GameObject is not assigned.", this);
        }
    }

    /// <summary>
    /// Called when the player interacts with the bat. Picks up the bat and hides it from the scene.
    /// This should be connected to a WalkToInteractable's OnInteract event.
    /// </summary>
    public void InteractWithBat()
    {
        if (hasBat)
        {
            Debug.Log("PUZZLE: Pierce already has the baseball bat.");
            return;
        }

        hasBat = true;

        if (bat != null)
        {
            SpriteRenderer spriteRenderer = bat.GetComponent<SpriteRenderer>();
            
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }

            Collider2D collider = bat.GetComponent<Collider2D>();
            
            if (collider != null)
            {
                collider.enabled = false;
            }
        }

        Debug.Log("PUZZLE: Pierce picks up the baseball bat.");
        OnBatPickedUp.Invoke();
    }

    /// <summary>
    /// Called when the player interacts with the door. Handles puzzle state progression.
    /// This should be connected to a WalkToInteractable's OnInteract event.
    /// </summary>
    public void InteractWithDoor()
    {
        if (doorOpened)
        {
            Debug.Log("PUZZLE: The door is already open.");
            return;
        }

        if (!doorTriedOnce)
        {
            doorTriedOnce = true;
            Debug.Log("PUZZLE: The door is locked. Pierce can't leave.");
            OnDoorLocked.Invoke();
            return;
        }

        if (!hasBat)
        {
            Debug.Log("PUZZLE: Pierce needs something to break the window.");
            OnNeedBat.Invoke();
            return;
        }

        OpenDoor();
    }

    /// <summary>
    /// Opens the door by changing its sprite and marking the puzzle as complete.
    /// </summary>
    private void OpenDoor()
    {
        doorOpened = true;

        if (door != null && brokenDoorSprite != null)
        {
            SpriteRenderer spriteRenderer = door.GetComponent<SpriteRenderer>();
            
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = brokenDoorSprite;
            }
            else
            {
                Debug.LogWarning($"TutorialPuzzle on {gameObject.name}: Door GameObject has no SpriteRenderer component.", this);
            }
        }

        Debug.Log("PUZZLE: Pierce smashes the window with the bat and unlocks the door.");
        OnDoorOpened.Invoke();
    }

    /// <summary>
    /// Public method to check if the player has the bat. Useful for external systems.
    /// </summary>
    /// <returns>True if the player has picked up the bat.</returns>
    public bool HasBat()
    {
        return hasBat;
    }

    /// <summary>
    /// Public method to check if the door is open. Useful for external systems.
    /// </summary>
    /// <returns>True if the door has been opened.</returns>
    public bool IsDoorOpen()
    {
        return doorOpened;
    }

    /// <summary>
    /// Resets the puzzle to its initial state. Useful for testing or replay functionality.
    /// </summary>
    public void ResetPuzzle()
    {
        hasBat = false;
        doorTriedOnce = false;
        doorOpened = false;

        if (bat != null)
        {
            SpriteRenderer spriteRenderer = bat.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }

            Collider2D collider = bat.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = true;
            }
        }

        Debug.Log("PUZZLE: Tutorial puzzle has been reset.");
    }
}
