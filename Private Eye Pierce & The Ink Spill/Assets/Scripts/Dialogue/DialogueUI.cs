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

    private Coroutine typingCoroutine;

    private void Awake()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        else
            Debug.LogWarning("DialogueUI: Dialogue Panel not assigned!");
    }

    private void Update()
    {
        if (!dialogueActive)
            return;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            AdvanceDialogue();
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
        if (line.speaker == Speaker.Player)
            speakerNameText.text = currentDialogue.playerName;
        else
            speakerNameText.text = currentDialogue.otherName;

        // Set portraits with overrides or defaults
        if (line.speaker == Speaker.Player && line.portrait != null)
            playerImage.sprite = line.portrait;
        else
            playerImage.sprite = currentDialogue.defaultPlayerPortrait;

        if (line.speaker == Speaker.Other && line.portrait != null)
            npcImage.sprite = line.portrait;
        else
            npcImage.sprite = currentDialogue.defaultOtherPortrait;

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
        typingCoroutine = StartCoroutine(TypeText(line.text, line.typingSpeed));
    }

    /// <summary>
    /// Types out the line character by character. Uses currentLineIndex field when invoking onLineEnd.
    /// </summary>
    private IEnumerator TypeText(string fullText, float typingSpeed)
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

        if (currentDialogue != null && currentLineIndex < currentDialogue.lines.Count)
            currentDialogue.lines[currentLineIndex].onLineEnd.Invoke();
    }

    /// <summary>
    /// Advances dialogue on click: either completes the current line typewriter or moves to the next line.
    /// </summary>
    private void AdvanceDialogue()
    {
        if (!dialogueActive)
            return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            if (currentDialogue != null && currentLineIndex < currentDialogue.lines.Count)
                dialogueText.text = currentDialogue.lines[currentLineIndex].text;
            currentDialogue.lines[currentLineIndex].onLineEnd.Invoke();
            return;
        }

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
