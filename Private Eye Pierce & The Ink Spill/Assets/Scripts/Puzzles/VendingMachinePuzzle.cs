using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Pierce's hand gets stuck; fullscreen arm image shakes. Click fast to escape.
/// Setup: Same GameObject (or sibling) needs Interactable with On Interact -> VendingMachinePuzzle.StartPuzzle.
/// Player must be in the same room as the vending machine (e.g. enter the Hallway first); otherwise the collider is inactive.
/// </summary>
public class VendingMachinePuzzle : MonoBehaviour
{
    [Header("Progress Settings")]
    public float targetProgress = 20f;
    public float clickPower = 1f;

    [Header("Early Stage Resistance")]
    public float resistanceInterval = 2f;
    public float resistanceStep = 1f;

    [Header("Late Stage Resistance")]
    public bool lateStageEnabled = true;
    [Range(0f, 1f)]
    public float lateStageStartNormalized = 0.7f;
    public float lateStageMaxDecayPerSecond = 5f;
    public bool lateStageOverridesDiscrete = true;

    [Header("Fullscreen Arm (Shakes During Puzzle)")]
    public Image fullscreenArmImage;
    public Image blackBackgroundImage;
    public float maxShakeIntensity = 20f;
    public float clickWindowDuration = 1f;

    [Header("Player (Hidden During Puzzle)")]
    public GameObject playerRoot;

    [Header("Hand Reveal (Shows After Escape)")]
    public Image handRevealImage;
    public float handDisplayDuration = 2f;
    public float handFadeDuration = 0.5f;

    [Header("Fade Settings")]
    public float armFadeDuration = 0.5f;

    [Header("UI")]
    public ButtonMashPromptUI promptUI;

    [Header("Completion")]
    public EventFlag breakRoomUnlockedFlag;

    // KEVIN EDIT - gate: puzzle won't start unless this flag is active (e.g. TutorialComplete)
    [Header("Gate")]
    [Tooltip("If set, puzzle only starts when this flag is active. Use TutorialComplete so vending machine is blocked until window is broken.")]
    public EventFlag requiredFlag;
    [Tooltip("Optional. Dialogue runner for showing the blocked message.")]
    public DialogueRunner dialogueRunner;
    [Tooltip("Optional. Played when player tries the vending machine before the required flag is set.")]
    public DialogueAsset blockedDialogue;

    [Header("Events")]
    public UnityEvent OnPuzzleStarted;
    public UnityEvent OnPuzzleCompleted;
    public UnityEvent OnPuzzleEnded;

    private float currentProgress = 0f;
    private bool puzzleActive = false;
    private bool puzzleCompleted = false;
    private float resistanceTimer = 0f;
    private List<float> clickTimestamps = new List<float>();
    private RectTransform armRectTransform;
    private Vector2 armOriginalPosition;

    // KEVIN EDIT - for waiting on blocked dialogue before ending interaction
    private DialogueUI _dialogueUI;
    private bool _waitingForBlockedDialogue;

    void Awake()
    {
        if (fullscreenArmImage != null)
        {
            armRectTransform = fullscreenArmImage.GetComponent<RectTransform>();
            armOriginalPosition = armRectTransform.anchoredPosition;
            fullscreenArmImage.preserveAspect = true;
            fullscreenArmImage.gameObject.SetActive(false);
        }

        if (blackBackgroundImage != null)
            blackBackgroundImage.gameObject.SetActive(false);

        if (handRevealImage != null)
            handRevealImage.gameObject.SetActive(false);

        if (playerRoot == null)
        {
            PointClickController p = FindFirstObjectByType<PointClickController>();
            if (p != null)
                playerRoot = p.gameObject;
        }
    }

    // KEVIN EDIT - cache DialogueUI for blocked dialogue callback
    void Start()
    {
        if (dialogueRunner == null)
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();
        if (dialogueRunner != null)
            _dialogueUI = dialogueRunner.GetComponent<DialogueUI>();
        if (_dialogueUI != null)
            _dialogueUI.OnDialogueFinished += OnBlockedDialogueFinished;
    }

    void OnDestroy()
    {
        if (_dialogueUI != null)
            _dialogueUI.OnDialogueFinished -= OnBlockedDialogueFinished;
    }

    private void OnBlockedDialogueFinished()
    {
        if (_waitingForBlockedDialogue)
        {
            _waitingForBlockedDialogue = false;
            Interactable.EndCurrentInteraction();
        }
    }

    void Update()
    {
        if (!puzzleActive) return;

        HandleInput();
        HandleResistance();
        HandleShake();
        UpdateUI();
    }

    public void StartPuzzle()
    {
        if (puzzleActive) return;
        if (puzzleCompleted)
        {
            Interactable.EndCurrentInteraction();
            return;
        }

        // KEVIN EDIT - block puzzle if required flag is not yet active (dunno how players would get to this before the tutorial ends, but just to be safe...)
        if (requiredFlag != null && !requiredFlag.isActive)
        {
            Debug.Log("PUZZLE: Vending machine blocked — required flag not active.");
            if (dialogueRunner != null && blockedDialogue != null)
            {
                _waitingForBlockedDialogue = true;
                dialogueRunner.StartDialogue(blockedDialogue);
            }
            else
            {
                Interactable.EndCurrentInteraction();
            }
            return;
        }

        if (fullscreenArmImage == null)
            Debug.LogWarning("VendingMachinePuzzle: Fullscreen Arm Image is not assigned. Assign it in the Inspector so the arm is visible during the puzzle.", this);

        puzzleActive = true;
        currentProgress = 0f;
        resistanceTimer = 0f;
        clickTimestamps.Clear();

        if (playerRoot != null)
            playerRoot.SetActive(false);

        if (blackBackgroundImage != null)
        {
            blackBackgroundImage.gameObject.SetActive(true);
            SetImageAlpha(blackBackgroundImage, 1f);
        }

        if (fullscreenArmImage != null)
        {
            fullscreenArmImage.preserveAspect = true;
            fullscreenArmImage.gameObject.SetActive(true);
            SetImageAlpha(fullscreenArmImage, 1f);
        }

        if (promptUI != null)
        {
            promptUI.SetVisible(true);
            promptUI.SetProgress01(0f);
            Canvas canvas = GetPuzzleCanvas();
            if (canvas != null)
                promptUI.SetTextToFront(canvas);
        }
        else
            Debug.LogWarning("VendingMachinePuzzle: ButtonMashPromptUI (Prompt UI) is not assigned. Assign it for click-to-escape feedback.", this);

        OnPuzzleStarted?.Invoke();
        Debug.Log("PUZZLE: Vending machine puzzle started. Click to escape!");
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
            RegisterClick();
    }

    void RegisterClick()
    {
        currentProgress += clickPower;
        clickTimestamps.Add(Time.time);

        if (promptUI != null)
            promptUI.NotifyMash();

        if (currentProgress >= targetProgress)
            CompletePuzzle();
    }

    void HandleResistance()
    {
        float normalized = currentProgress / targetProgress;
        bool inLateStage = lateStageEnabled && normalized >= lateStageStartNormalized;

        if (inLateStage)
        {
            float lateProgress = (normalized - lateStageStartNormalized) / (1f - lateStageStartNormalized);
            float decayRate = Mathf.Lerp(0f, lateStageMaxDecayPerSecond, lateProgress);
            currentProgress -= decayRate * Time.deltaTime;

            if (lateStageOverridesDiscrete)
            {
                resistanceTimer = 0f;
                currentProgress = Mathf.Max(0f, currentProgress);
                return;
            }
        }

        resistanceTimer += Time.deltaTime;
        if (resistanceTimer >= resistanceInterval)
        {
            resistanceTimer = 0f;
            currentProgress -= resistanceStep;
        }

        currentProgress = Mathf.Max(0f, currentProgress);
    }

    void HandleShake()
    {
        if (armRectTransform == null) return;

        float cutoff = Time.time - clickWindowDuration;
        clickTimestamps.RemoveAll(t => t < cutoff);

        float clicksPerSecond = clickTimestamps.Count / clickWindowDuration;
        float intensity = Mathf.Clamp01(clicksPerSecond / 10f) * maxShakeIntensity;

        Vector2 shake = new Vector2(
            Random.Range(-intensity, intensity),
            Random.Range(-intensity, intensity)
        );
        armRectTransform.anchoredPosition = armOriginalPosition + shake;
    }

    void UpdateUI()
    {
        if (promptUI != null)
            promptUI.SetProgress01(currentProgress / targetProgress);
    }

    void CompletePuzzle()
    {
        puzzleActive = false;
        puzzleCompleted = true;

        Debug.Log("PUZZLE: Vending machine puzzle completed!");
        OnPuzzleCompleted?.Invoke();

        if (armRectTransform != null)
            armRectTransform.anchoredPosition = armOriginalPosition;

        if (promptUI != null)
            promptUI.SetVisible(false);

        StartCoroutine(CompletionSequence());
    }

    IEnumerator CompletionSequence()
    {
        yield return StartCoroutine(FadeOutArm());

        yield return StartCoroutine(ShowHandReveal());

        if (playerRoot != null)
            playerRoot.SetActive(true);

        if (breakRoomUnlockedFlag != null)
            breakRoomUnlockedFlag.Toggle();

        Interactable.EndCurrentInteraction();

        OnPuzzleEnded?.Invoke();
    }

    IEnumerator FadeOutArm()
    {
        if (fullscreenArmImage == null) yield break;

        float elapsed = 0f;
        while (elapsed < armFadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / armFadeDuration);
            SetImageAlpha(fullscreenArmImage, alpha);
            yield return null;
        }

        fullscreenArmImage.gameObject.SetActive(false);

        if (blackBackgroundImage != null)
            blackBackgroundImage.gameObject.SetActive(false);
    }

    IEnumerator ShowHandReveal()
    {
        if (handRevealImage == null) yield break;

        handRevealImage.gameObject.SetActive(true);
        SetImageAlpha(handRevealImage, 1f);

        yield return new WaitForSeconds(handDisplayDuration);

        float elapsed = 0f;
        while (elapsed < handFadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / handFadeDuration);
            SetImageAlpha(handRevealImage, alpha);
            yield return null;
        }

        handRevealImage.gameObject.SetActive(false);
    }

    void SetImageAlpha(Image img, float alpha)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    Canvas GetPuzzleCanvas()
    {
        if (fullscreenArmImage != null)
            return fullscreenArmImage.GetComponentInParent<Canvas>();
        if (blackBackgroundImage != null)
            return blackBackgroundImage.GetComponentInParent<Canvas>();
        return null;
    }

    public float GetProgressNormalized() => currentProgress / targetProgress;
    public bool IsPuzzleActive() => puzzleActive;
    public bool IsPuzzleCompleted() => puzzleCompleted;
}