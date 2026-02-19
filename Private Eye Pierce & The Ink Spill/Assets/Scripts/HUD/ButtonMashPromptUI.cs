using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    [Header("UI References (Use TMP or Legacy Text)")]
    public TMP_Text tmpText;
    public Text legacyText;
    public GameObject panelRoot;
    /// <summary>Optional. Assign to have only this (e.g. text) drawn in front of overlays; panel stays behind. If unset, the TMP/legacy text transform is used.</summary>
    public Transform textFrontRoot;

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
    private Transform _textOriginalParent;
    private int _textOriginalSiblingIndex;

    void Awake()
    {
        if (tmpText != null)
        {
            rectTransform = tmpText.GetComponent<RectTransform>();
        }
        else if (legacyText != null)
        {
            rectTransform = legacyText.GetComponent<RectTransform>();
        }

        if (rectTransform != null)
        {
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
        if (visible)
            gameObject.SetActive(true);

        if (panelRoot != null)
            panelRoot.SetActive(visible);

        if (!visible)
            SetTextBackToPanel();

        if (visible)
        {
            currentProgress = 0f;
            UpdateTextFromProgress();
        }
    }

    /// <summary>
    /// Reparents the text (or textFrontRoot) to the canvas as last sibling so it draws in front. Panel stays in place (behind).
    /// </summary>
    public void SetTextToFront(Canvas canvas)
    {
        if (canvas == null) return;
        Transform textRoot = textFrontRoot != null ? textFrontRoot : rectTransform != null ? rectTransform : null;
        if (textRoot == null) return;
        if (textRoot.parent == canvas.transform) return;

        _textOriginalParent = textRoot.parent;
        _textOriginalSiblingIndex = textRoot.GetSiblingIndex();
        textRoot.SetParent(canvas.transform, true);
        textRoot.SetAsLastSibling();
    }

    /// <summary>
    /// Puts the text back under the panel. Call when hiding the prompt.
    /// </summary>
    public void SetTextBackToPanel()
    {
        if (_textOriginalParent == null) return;
        Transform textRoot = textFrontRoot != null ? textFrontRoot : rectTransform != null ? rectTransform : null;
        if (textRoot == null) return;

        textRoot.SetParent(_textOriginalParent, true);
        textRoot.SetSiblingIndex(_textOriginalSiblingIndex);
        _textOriginalParent = null;
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
        if (tmpText != null)
            tmpText.text = text;
        else if (legacyText != null)
            legacyText.text = text;
    }

    void UpdateTextFromProgress()
    {
        if (progressTexts.Count == 0) return;
        if (tmpText == null && legacyText == null) return;

        string textToUse = progressTexts[0].text;
        foreach (var pt in progressTexts)
        {
            if (currentProgress >= pt.threshold)
                textToUse = pt.text;
            else
                break;
        }
        SetText(textToUse);
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