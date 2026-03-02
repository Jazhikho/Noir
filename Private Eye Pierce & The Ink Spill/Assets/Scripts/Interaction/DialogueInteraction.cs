using UnityEngine;
using UnityEngine.Events;

public class DialogueInteraction : MonoBehaviour
{
    [Header("Dialogue")]
    public DialogueRunner dialogueRunner;
    public DialogueAsset dialogueAsset;

    [Header("Conditional Dialogue")]
    public EventFlag conditionalFlag;
    public DialogueAsset conditionalDialogueAsset;

    // KEVIN EDIT - one-shot gating: if talkOnceFlag is active, interaction is skipped entirely
    [Header("One-Shot")]
    [Tooltip("If set and active, this interaction does nothing (used for talk-once NPCs like Carmen).")]
    public EventFlag talkOnceFlag;
    [Tooltip("If set, this flag is activated when the dialogue finishes (pairs with talkOnceFlag for one-shot behavior).")]
    public EventFlag flagToSetOnEnd;

    [Header("Events")]
    public UnityEvent OnDialogueStarted;
    public UnityEvent OnDialogueEnded;

    private bool isDialogueActive = false;
    private DialogueUI dialogueUI;

    void Start()
    {
        if (dialogueRunner == null)
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();

        if (dialogueRunner != null)
            dialogueUI = dialogueRunner.GetComponent<DialogueUI>();

        if (dialogueUI != null)
            dialogueUI.OnDialogueFinished += HandleDialogueFinished;
    }

    void OnDestroy()
    {
        if (dialogueUI != null)
            dialogueUI.OnDialogueFinished -= HandleDialogueFinished;
    }

    public void StartDialogue()
    {
        // KEVIN EDIT - block if talk-once flag is already set (e.g., Carmen already talked)
        if (talkOnceFlag != null && talkOnceFlag.isActive)
        {
            Interactable.EndCurrentInteraction();
            return;
        }

        if (dialogueRunner == null)
        {
            Debug.LogWarning("DialogueInteraction: No DialogueRunner assigned.");
            Interactable.EndCurrentInteraction();
            return;
        }

        DialogueAsset assetToPlay = dialogueAsset;

        if (conditionalFlag != null && conditionalFlag.isActive && conditionalDialogueAsset != null)
            assetToPlay = conditionalDialogueAsset;

        if (assetToPlay == null)
        {
            Debug.LogWarning("DialogueInteraction: No DialogueAsset to play.");
            Interactable.EndCurrentInteraction();
            return;
        }

        isDialogueActive = true;
        OnDialogueStarted?.Invoke();

        dialogueRunner.StartDialogue(assetToPlay);
    }

    void HandleDialogueFinished()
    {
        if (!isDialogueActive) return;

        isDialogueActive = false;

        // KEVIN EDIT - set the end flag so this interaction is gated on next click
        if (flagToSetOnEnd != null && !flagToSetOnEnd.isActive)
            flagToSetOnEnd.SetActive(true);

        OnDialogueEnded?.Invoke();

        Interactable.EndCurrentInteraction();
    }

    public bool IsDialogueActive() => isDialogueActive;
}