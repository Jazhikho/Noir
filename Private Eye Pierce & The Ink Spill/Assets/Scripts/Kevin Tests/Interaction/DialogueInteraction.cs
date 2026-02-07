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
        OnDialogueEnded?.Invoke();

        Interactable.EndCurrentInteraction();
    }

    public bool IsDialogueActive() => isDialogueActive;
}
