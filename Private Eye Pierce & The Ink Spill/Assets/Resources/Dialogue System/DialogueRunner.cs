using UnityEngine;

[RequireComponent(typeof(DialogueUI))]
public class DialogueRunner : MonoBehaviour
{
    public DialogueAsset dialogueAsset;
    DialogueUI ui;

    void Awake()
    {
        ui = GetComponent<DialogueUI>();
    }

    public void StartDialogue(DialogueAsset asset = null)
    {
        if (asset != null) dialogueAsset = asset;
        if (dialogueAsset == null)
        {
            Debug.LogWarning("DialogueRunner: no DialogueAsset assigned.");
            return;
        }
        ui.StartDialogue(dialogueAsset);
    }
}
