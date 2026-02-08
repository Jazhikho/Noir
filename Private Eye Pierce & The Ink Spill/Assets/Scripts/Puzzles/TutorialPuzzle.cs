using UnityEngine;
using UnityEngine.Events;

namespace KevinTests.Puzzles
{
    /// <summary>
    /// Kevin Tests: tutorial puzzle with dialogue integration. Main game uses Scripts/TutorialPuzzle.
    /// </summary>
    public class TutorialPuzzle : MonoBehaviour
    {
        [Header("Object References")]
        public GameObject bat;
        public GameObject door;

        [Header("Door Sprite")]
        public Sprite brokenDoorSprite;

        [Header("Dialogue")]
        public DialogueRunner dialogueRunner;
        public DialogueAsset doorLockedDialogue;
        public DialogueAsset needBatDialogue;
        public DialogueAsset batPickedUpDialogue;
        public DialogueAsset doorOpenedDialogue;

        [Header("Flags")]
        public EventFlag tutorialCompleteFlag;

        [Header("Testing")]
        public bool autoReenableMovement = false;

        [Header("Events")]
        public UnityEvent OnBatPickedUp;
        public UnityEvent OnDoorLocked;
        public UnityEvent OnNeedBat;
        public UnityEvent OnDoorOpened;

        private bool hasBat = false;
        private bool doorTriedOnce = false;
        private bool doorOpened = false;
        private DialogueUI dialogueUI;
        private bool waitingForDialogue = false;

        void Start()
        {
            if (dialogueRunner == null)
                dialogueRunner = FindFirstObjectByType<DialogueRunner>();

            if (dialogueRunner != null)
                dialogueUI = dialogueRunner.GetComponent<DialogueUI>();

            if (dialogueUI != null)
                dialogueUI.OnDialogueFinished += OnDialogueComplete;
        }

        void OnDestroy()
        {
            if (dialogueUI != null)
                dialogueUI.OnDialogueFinished -= OnDialogueComplete;
        }

        void OnDialogueComplete()
        {
            if (waitingForDialogue)
            {
                waitingForDialogue = false;
                Interactable.EndCurrentInteraction();
            }
        }

        public void InteractWithBat()
        {
            if (hasBat)
            {
                if (autoReenableMovement) Interactable.EndCurrentInteraction();
                return;
            }

            hasBat = true;

            SpriteRenderer sr = bat.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;

            Collider2D col = bat.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            Debug.Log("PUZZLE: Pierce picks up the baseball bat.");
            OnBatPickedUp?.Invoke();

            if (dialogueRunner != null && batPickedUpDialogue != null)
            {
                waitingForDialogue = true;
                dialogueRunner.StartDialogue(batPickedUpDialogue);
            }
            else if (autoReenableMovement)
            {
                Interactable.EndCurrentInteraction();
            }
        }

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
                Debug.Log("PUZZLE: The door is locked. Pierce cannot leave.");
                OnDoorLocked?.Invoke();

                if (dialogueRunner != null && doorLockedDialogue != null)
                {
                    waitingForDialogue = true;
                    dialogueRunner.StartDialogue(doorLockedDialogue);
                }
                else if (autoReenableMovement)
                {
                    Interactable.EndCurrentInteraction();
                }
                return;
            }

            if (!hasBat)
            {
                Debug.Log("PUZZLE: Pierce needs something to break the window.");
                OnNeedBat?.Invoke();

                if (dialogueRunner != null && needBatDialogue != null)
                {
                    waitingForDialogue = true;
                    dialogueRunner.StartDialogue(needBatDialogue);
                }
                else if (autoReenableMovement)
                {
                    Interactable.EndCurrentInteraction();
                }
                return;
            }

            doorOpened = true;

            if (brokenDoorSprite != null)
            {
                SpriteRenderer doorSR = door.GetComponent<SpriteRenderer>();
                if (doorSR != null) doorSR.sprite = brokenDoorSprite;
            }

            if (tutorialCompleteFlag != null)
                tutorialCompleteFlag.Toggle();

            Debug.Log("PUZZLE: Pierce smashes the window with the bat and unlocks the door.");
            OnDoorOpened?.Invoke();

            if (dialogueRunner != null && doorOpenedDialogue != null)
            {
                waitingForDialogue = true;
                dialogueRunner.StartDialogue(doorOpenedDialogue);
            }
            else if (autoReenableMovement)
            {
                Interactable.EndCurrentInteraction();
            }
        }

        public bool HasBat() => hasBat;
        public bool IsDoorOpened() => doorOpened;
    }
}
