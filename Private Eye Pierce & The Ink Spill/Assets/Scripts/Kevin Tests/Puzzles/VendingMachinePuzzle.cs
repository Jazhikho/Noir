using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class VendingMachinePuzzle : MonoBehaviour
{
    [Header("Progress Settings")]
    public float targetProgress = 100f;
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
    public float maxShakeIntensity = 20f;
    public float clickWindowDuration = 1f;

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

    [Header("Events")]
    public UnityEvent OnPuzzleStarted;
    public UnityEvent OnPuzzleCompleted;
    public UnityEvent OnPuzzleEnded;

    private float currentProgress = 0f;
    private bool puzzleActive = false;
    private float resistanceTimer = 0f;
    private List<float> clickTimestamps = new List<float>();
    private RectTransform armRectTransform;
    private Vector2 armOriginalPosition;

    void Awake()
    {
        if (fullscreenArmImage != null)
        {
            armRectTransform = fullscreenArmImage.GetComponent<RectTransform>();
            armOriginalPosition = armRectTransform.anchoredPosition;
            fullscreenArmImage.gameObject.SetActive(false);
        }

        if (handRevealImage != null)
        {
            handRevealImage.gameObject.SetActive(false);
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

        puzzleActive = true;
        currentProgress = 0f;
        resistanceTimer = 0f;
        clickTimestamps.Clear();

        if (fullscreenArmImage != null)
        {
            fullscreenArmImage.gameObject.SetActive(true);
            SetImageAlpha(fullscreenArmImage, 1f);
        }

        if (promptUI != null)
        {
            promptUI.SetVisible(true);
            promptUI.SetProgress01(0f);
        }

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

    public float GetProgressNormalized() => currentProgress / targetProgress;
    public bool IsPuzzleActive() => puzzleActive;
}