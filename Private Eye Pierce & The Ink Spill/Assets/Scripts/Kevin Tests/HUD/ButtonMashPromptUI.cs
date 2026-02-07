using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ButtonMashPromptUI : MonoBehaviour
{
    [System.Serializable]
    public class ProgressText
    {
        [Range(0f, 1f)]
        public float threshold = 0f;
        public string text = "Click to escape!";
    }

    [Header("UI References")]
    public Text promptText;
    public GameObject panelRoot;

    [Header("Bob Animation")]
    public float bobSpeed = 2f;
    public float bobAmount = 10f;

    [Header("Punch Animation")]
    public float punchScale = 1.2f;
    public float punchDuration = 0.1f;

    [Header("Progress Texts")]
    public List<ProgressText> progressTexts = new List<ProgressText>();

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector3 originalScale;
    private Coroutine punchCoroutine;
    private float currentProgress = 0f;

    void Awake()
    {
        if (promptText != null)
        {
            rectTransform = promptText.GetComponent<RectTransform>();
            originalPosition = rectTransform.anchoredPosition;
            originalScale = rectTransform.localScale;
        }

        progressTexts.Sort((a, b) => a.threshold.CompareTo(b.threshold));

        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    void Update()
    {
        if (panelRoot != null && !panelRoot.activeSelf) return;

        if (rectTransform != null && punchCoroutine == null)
        {
            float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
            rectTransform.anchoredPosition = originalPosition + new Vector2(0f, bob);
        }
    }

    public void SetVisible(bool visible)
    {
        if (panelRoot != null)
            panelRoot.SetActive(visible);

        if (visible)
        {
            currentProgress = 0f;
            UpdateTextFromProgress();
        }
    }

    public void NotifyMash()
    {
        if (punchCoroutine != null)
            StopCoroutine(punchCoroutine);
        punchCoroutine = StartCoroutine(PunchCoroutine());
    }

    public void SetProgress01(float progress)
    {
        currentProgress = Mathf.Clamp01(progress);
        UpdateTextFromProgress();
    }

    public void SetText(string text)
    {
        if (promptText != null)
            promptText.text = text;
    }

    void UpdateTextFromProgress()
    {
        if (progressTexts.Count == 0 || promptText == null) return;

        string textToUse = progressTexts[0].text;
        foreach (var pt in progressTexts)
        {
            if (currentProgress >= pt.threshold)
                textToUse = pt.text;
            else
                break;
        }
        promptText.text = textToUse;
    }

    IEnumerator PunchCoroutine()
    {
        if (rectTransform == null) yield break;

        float elapsed = 0f;
        float halfDuration = punchDuration * 0.5f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            float scale = Mathf.Lerp(1f, punchScale, t);
            rectTransform.localScale = originalScale * scale;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            float scale = Mathf.Lerp(punchScale, 1f, t);
            rectTransform.localScale = originalScale * scale;
            yield return null;
        }

        rectTransform.localScale = originalScale;
        punchCoroutine = null;
    }
}
