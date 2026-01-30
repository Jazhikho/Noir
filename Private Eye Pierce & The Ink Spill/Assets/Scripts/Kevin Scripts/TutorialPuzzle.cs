using UnityEngine;
using UnityEngine.Events;

// THE BELOW IS CURRENTLY IN DEBUG MODE

public class TutorialPuzzle : MonoBehaviour
{
    [Header("Object References")]
    public GameObject bat;
    public GameObject door;

    [Header("Door Sprite")]
    public Sprite brokenDoorSprite;

    [Header("Testing")]
    public bool autoReenableMovement = true;

    [Header("Bat Events")]
    public UnityEvent OnBatPickedUp;

    [Header("Door Events")]
    public UnityEvent OnDoorLocked;
    public UnityEvent OnNeedBat;
    public UnityEvent OnDoorOpened;

    private bool hasBat = false;
    private bool doorTriedOnce = false;
    private bool doorOpened = false;

    // Called by the bat's Interactable OnInteract event
    public void InteractWithBat()
    {
        if (hasBat) return;

        hasBat = true;

        SpriteRenderer sr = bat.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = false;
        }

        Collider2D col = bat.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        Debug.Log("PUZZLE: Pierce picks up the baseball bat.");
        OnBatPickedUp.Invoke();

        if (autoReenableMovement)
        {
            Interactable.EndCurrentInteraction();
        }
    }

    // Called by the door's Interactable OnInteract event
    public void InteractWithDoor()
    {
        if (doorOpened)
        {
            Debug.Log("PUZZLE: The door is already open.");
            if (autoReenableMovement) Interactable.EndCurrentInteraction();
            return;
        }

        if (!doorTriedOnce)
        {
            doorTriedOnce = true;
            Debug.Log("PUZZLE: The door is locked. Pierce can't leave.");
            OnDoorLocked.Invoke();
            if (autoReenableMovement) Interactable.EndCurrentInteraction();
            return;
        }

        if (!hasBat)
        {
            Debug.Log("PUZZLE: Pierce needs something to break the window.");
            OnNeedBat.Invoke();
            if (autoReenableMovement) Interactable.EndCurrentInteraction();
            return;
        }

        doorOpened = true;

        if (brokenDoorSprite != null)
        {
            SpriteRenderer sr = door.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = brokenDoorSprite;
            }
        }

        Debug.Log("PUZZLE: Pierce smashes the window with the bat and unlocks the door.");
        OnDoorOpened.Invoke();
        if (autoReenableMovement) Interactable.EndCurrentInteraction();
    }
}