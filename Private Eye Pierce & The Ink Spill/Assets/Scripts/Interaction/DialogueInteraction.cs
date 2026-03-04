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

        // KEVIN EDIT - subscription verification to confirm dialogue event wiring per-NPC
        if (dialogueUI != null)
        {
            dialogueUI.OnDialogueFinished += HandleDialogueFinished;
            Debug.Log($"[DialogueInteraction] {gameObject.name}: subscribed to OnDialogueFinished (dialogueUI on '{dialogueUI.gameObject.name}')");
        }
        else
            Debug.LogWarning($"[DialogueInteraction] {gameObject.name}: dialogueUI is NULL — subscription FAILED");
    }

    void OnDestroy()
    {
        if (dialogueUI != null)
            dialogueUI.OnDialogueFinished -= HandleDialogueFinished;
    }

    public void StartDialogue()
    {
        // KEVIN EDIT - entry point trace to verify interaction-to-dialogue pipeline
        Debug.Log($"[DialogueInteraction] {gameObject.name}: StartDialogue called");

        // KEVIN EDIT - block if talk-once flag is already set (e.g., Carmen already talked)
        if (talkOnceFlag != null && talkOnceFlag.isActive)
        {
            Debug.Log($"[DialogueInteraction] {gameObject.name}: talkOnceFlag active — skipping dialogue");
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
        // KEVIN EDIT - trace dialogue asset selection for QA playthrough validation
        Debug.Log($"[DialogueInteraction] {gameObject.name}: isDialogueActive = true, starting dialogue '{assetToPlay.dialogueName}'");
        OnDialogueStarted?.Invoke();

        dialogueRunner.StartDialogue(assetToPlay);
    }

    void HandleDialogueFinished()
    {
        // KEVIN EDIT - completion trace to confirm movement re-enable fires correctly
        Debug.Log($"[DialogueInteraction] {gameObject.name}: HandleDialogueFinished called, isDialogueActive={isDialogueActive}");

        if (!isDialogueActive)
        {
            Debug.Log($"[DialogueInteraction] {gameObject.name}: SKIPPED — isDialogueActive was false");
            return;
        }

        isDialogueActive = false;

        // KEVIN EDIT - set the end flag so this interaction is gated on next click
        if (flagToSetOnEnd != null && !flagToSetOnEnd.isActive)
            flagToSetOnEnd.SetActive(true);

        OnDialogueEnded?.Invoke();

        Debug.Log($"[DialogueInteraction] {gameObject.name}: calling EndCurrentInteraction now");
        Interactable.EndCurrentInteraction();
    }

    public bool IsDialogueActive() => isDialogueActive;
}