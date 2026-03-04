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
    public Image dialogueBubble;
    public Sprite bubbleRight;
    public Sprite bubbleLeft;
    public TMP_Text dialogueText;
    public TMP_Text speakerNameText;

    public static bool IsDialogueActive { get; private set; }

    private DialogueAsset currentDialogue;
    private int currentLineIndex;
    private bool dialogueActive;

    private Coroutine typingCoroutine;
    private int dialogueStartFrame;

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
        if (Time.frameCount == dialogueStartFrame)
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
        IsDialogueActive = true;
        dialogueStartFrame = Time.frameCount;

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
        { 
            speakerNameText.text = currentDialogue.playerName;
            dialogueBubble.sprite = bubbleRight;
        }
        else
        { 
            speakerNameText.text = currentDialogue.otherName;
            dialogueBubble.sprite = bubbleLeft;
        }

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

        if (line.typingSpeed <= 0f)
        {
            dialogueText.text = line.text;
        }
        else
        {
            typingCoroutine = StartCoroutine(TypeText(line.text, line.typingSpeed));
        }
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
            typingCoroutine = null;
            yield break;
        }

        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        typingCoroutine = null;

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

        // KEVIN EDIT - trace logging to verify dialogue line advancement during QA
        Debug.Log($"[DialogueUI] AdvanceDialogue: lineIndex={currentLineIndex}, typingCoroutine={(typingCoroutine != null ? "active" : "null")}");

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
        // KEVIN EDIT - granular handler tracing to catch missing subscriber wiring during QA
        Debug.Log($"[DialogueUI] EndDialogue called. Subscriber count: {OnDialogueFinished?.GetInvocationList()?.Length ?? 0}");
        dialogueActive = false;
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        Debug.Log("[DialogueUI] About to invoke OnDialogueFinished handlers...");
        if (OnDialogueFinished != null)
        {
            var handlers = OnDialogueFinished.GetInvocationList();
            for (int i = 0; i < handlers.Length; i++)
            {
                var target = handlers[i].Target as MonoBehaviour;
                string targetName = target != null ? target.gameObject.name : "null";
                Debug.Log($"[DialogueUI] Invoking handler {i}: {handlers[i].Method.Name} on '{targetName}'");
                try
                {
                    ((Action)handlers[i]).Invoke();
                    Debug.Log($"[DialogueUI] Handler {i} completed OK");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[DialogueUI] Handler {i} threw: {e}");
                }
            }
        }
        Debug.Log("[DialogueUI] All handlers done. Dialogue fully ended.");
        StartCoroutine(ClearDialogueActiveNextFrame());
    }

    private IEnumerator ClearDialogueActiveNextFrame()
    {
        yield return null;
        IsDialogueActive = false;
    }
}
