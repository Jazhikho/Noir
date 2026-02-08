using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public class DialogueUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel; // Assign the panel GameObject in Inspector
    public Image playerImage;
    public Image npcImage;
    public TMP_Text dialogueText;
    public TMP_Text speakerNameText;

    private DialogueAsset currentDialogue;
    private int currentLineIndex;
    private bool dialogueActive;

    private PlayerInput playerInput;
    private InputAction advanceAction;

    private Coroutine typingCoroutine;

    private void Awake()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        else
            Debug.LogWarning("DialogueUI: Dialogue Panel not assigned!");

        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found on DialogueUI GameObject.");
            return;
        }

        advanceAction = playerInput.actions["AdvanceDialogue"];
    }

    private void OnEnable()
    {
        if (advanceAction != null)
            advanceAction.performed += OnAdvanceDialogue;
    }

    private void OnDisable()
    {
        if (advanceAction != null)
            advanceAction.performed -= OnAdvanceDialogue;
    }

    public void StartDialogue(DialogueAsset dialogue)
    {
        if (dialogue == null)
        {
            Debug.LogWarning("DialogueUI: StartDialogue called with null dialogue.");
            return;
        }

        currentDialogue = dialogue;
        currentLineIndex = 0;
        dialogueActive = true;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        ShowLine();
    }

    private void ShowLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (currentDialogue == null || currentLineIndex >= currentDialogue.lines.Count)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentDialogue.lines[currentLineIndex];

        // Set speaker name
        speakerNameText.text = (line.speaker == Speaker.Player) ? currentDialogue.playerName : currentDialogue.otherName;

        // Set portraits with overrides or defaults
        playerImage.sprite = (line.speaker == Speaker.Player && line.portrait != null)
            ? line.portrait
            : currentDialogue.defaultPlayerPortrait;

        npcImage.sprite = (line.speaker == Speaker.Other && line.portrait != null)
            ? line.portrait
            : currentDialogue.defaultOtherPortrait;

        Color brightColor = Color.white;
        Color dimColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        // Brighten speaking portrait, dim the other
        if (line.speaker == Speaker.Player)
        {
            playerImage.color = brightColor;
            npcImage.color = dimColor;
        }
        else
        {
            playerImage.color = dimColor;
            npcImage.color = brightColor;
        }

        // Start typing text coroutine
        typingCoroutine = StartCoroutine(TypeText(line.text, line.typingSpeed, currentLineIndex));
    }

    private IEnumerator TypeText(string fullText, float typingSpeed, int currentlineIndex)
    {
        dialogueText.text = "";
        if (typingSpeed <= 0f)
        {
            dialogueText.text = fullText; // show instantly
            yield break;
        }

        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        Debug.Log("OnLineEnd");

        currentDialogue.lines[currentLineIndex].onLineEnd.Invoke();
    }

    public void OnAdvanceDialogue(InputAction.CallbackContext context)
    {
        if (!dialogueActive)
            return;

        // If currently typing, finish immediately
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;

            // Show full text instantly
            if (currentDialogue != null && currentLineIndex < currentDialogue.lines.Count)
                dialogueText.text = currentDialogue.lines[currentLineIndex].text;
            return;
        }
        Debug.Log("OnLineEnd");

        currentDialogue.lines[currentLineIndex].onLineEnd.Invoke();

        currentLineIndex++;
        ShowLine();
    }

    public event Action OnDialogueFinished;
    private void EndDialogue()
    {
        dialogueActive = false;
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        OnDialogueFinished?.Invoke();
    }
}
