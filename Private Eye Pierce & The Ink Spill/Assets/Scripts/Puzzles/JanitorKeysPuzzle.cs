using UnityEngine;
using UnityEngine.Events;

namespace KevinTests.Puzzles
{
    /// <summary>
    /// Controls the janitor keys quest: Pierce talks to the janitor at the elevator (quest start),
    /// searches garbage cans and office props across rooms to find the keys, then returns to the elevator to complete.
    /// Search locations: Pierce's office, office back, break room (e.g. fridge). Use KeyHuntManager + SearchableProp for searchables.
    /// </summary>
    public class JanitorKeysPuzzle : MonoBehaviour
    {
        [Header("Dialogue")]
        [Tooltip("Played when Pierce first interacts with the elevator; janitor gives the quest.")]
        public DialogueAsset janitorQuestStartDialogue;
        [Tooltip("Played when Pierce returns to the elevator without the keys yet.")]
        public DialogueAsset janitorReminderDialogue;
        [Tooltip("Played when Pierce returns with the keys; quest complete.")]
        public DialogueAsset janitorThanksDialogue;

        [Header("References")]
        public DialogueRunner dialogueRunner;
        [Tooltip("Optional. If set, keys hunt is driven by this manager; assign same Has Keys Flag here and on KeyHuntManager.")]
        public KeyHuntManager keyHuntManager;

        [Header("Flags")]
        [Tooltip("Set when the janitor has given the quest (first elevator interaction).")]
        public EventFlag janitorQuestStartedFlag;
        [Tooltip("Set when Pierce finds the keys (by SearchableProp/KeyHuntManager).")]
        public EventFlag hasKeysFlag;
        [Tooltip("Set when Pierce returns to the elevator with keys and completes the quest.")]
        public EventFlag janitorQuestCompleteFlag;

        [Header("Events")]
        public UnityEvent OnQuestStarted;
        public UnityEvent OnQuestCompleted;

        // KEVIN EDIT - fires AFTER the thanks dialogue finishes; wire DemoEndSequence.PlayEndSequence here
        [Tooltip("Fires after the quest-complete dialogue finishes. Wire DemoEndSequence.PlayEndSequence here for the end-of-demo transition.")]
        public UnityEvent OnQuestDialogueFinished;

        private DialogueUI _dialogueUI;
        private bool _waitingForDialogue;

        // KEVIN EDIT - tracks whether the dialogue we're waiting on is the quest-complete thanks dialogue
        private bool _questJustCompleted;

        private void Start()
        {
            if (dialogueRunner == null)
                dialogueRunner = FindFirstObjectByType<DialogueRunner>();

            if (dialogueRunner != null)
            {
                _dialogueUI = dialogueRunner.GetComponent<DialogueUI>();
                if (_dialogueUI != null)
                    _dialogueUI.OnDialogueFinished += OnDialogueFinished;
            }
        }

        private void OnDestroy()
        {
            if (_dialogueUI != null)
                _dialogueUI.OnDialogueFinished -= OnDialogueFinished;
        }

        private void OnDialogueFinished()
        {
            if (_waitingForDialogue)
            {
                _waitingForDialogue = false;

                // KEVIN EDIT - if this was the thanks dialogue, fire the post-dialogue event (DemoEndSequence hook)
                if (_questJustCompleted)
                {
                    _questJustCompleted = false;
                    OnQuestDialogueFinished?.Invoke();
                }

                Interactable.EndCurrentInteraction();
            }
        }

        /// <summary>
        /// Call from the elevator's Interactable On Interact. Handles quest start, reminder, or completion based on state.
        /// </summary>
        public void OnElevatorInteract()
        {
            bool questStarted = janitorQuestStartedFlag != null && janitorQuestStartedFlag.isActive;
            bool hasKeys = hasKeysFlag != null && hasKeysFlag.isActive;
            bool questComplete = janitorQuestCompleteFlag != null && janitorQuestCompleteFlag.isActive;

            if (questComplete)
            {
                Interactable.EndCurrentInteraction();
                return;
            }

            if (hasKeys)
            {
                if (janitorQuestCompleteFlag != null)
                    janitorQuestCompleteFlag.Toggle();

                Debug.Log("PUZZLE: Janitor keys quest completed.");
                OnQuestCompleted?.Invoke();

                // KEVIN EDIT - mark that we're waiting for the thanks dialogue specifically.
                // The existing OnQuestCompleted fired before the thanks dialogue started, which would have kicked off the demo end sequence mid-conversation. Oops
                _questJustCompleted = true;

                if (dialogueRunner != null && janitorThanksDialogue != null)
                {
                    _waitingForDialogue = true;
                    dialogueRunner.StartDialogue(janitorThanksDialogue);
                }
                else
                {
                    // KEVIN EDIT - no dialogue to play, fire the post-dialogue event immediately
                    _questJustCompleted = false;
                    OnQuestDialogueFinished?.Invoke();
                    Interactable.EndCurrentInteraction();
                }
                return;
            }

            if (!questStarted)
            {
                if (janitorQuestStartedFlag != null)
                    janitorQuestStartedFlag.Toggle();

                Debug.Log("PUZZLE: Janitor keys quest started.");
                OnQuestStarted?.Invoke();

                if (dialogueRunner != null && janitorQuestStartDialogue != null)
                {
                    _waitingForDialogue = true;
                    dialogueRunner.StartDialogue(janitorQuestStartDialogue);
                }
                else
                {
                    Interactable.EndCurrentInteraction();
                }
                return;
            }

            if (dialogueRunner != null && janitorReminderDialogue != null)
            {
                _waitingForDialogue = true;
                dialogueRunner.StartDialogue(janitorReminderDialogue);
            }
            else
            {
                Interactable.EndCurrentInteraction();
            }
        }

        /// <summary>True if the janitor has already given the quest.</summary>
        public bool IsQuestStarted()
        {
            return janitorQuestStartedFlag != null && janitorQuestStartedFlag.isActive;
        }

        /// <summary>True if Pierce has found the keys (HasKeys flag).</summary>
        public bool HasKeys()
        {
            return hasKeysFlag != null && hasKeysFlag.isActive;
        }

        /// <summary>True if the quest has been completed (keys returned to janitor).</summary>
        public bool IsQuestComplete()
        {
            return janitorQuestCompleteFlag != null && janitorQuestCompleteFlag.isActive;
        }
    }
}