using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Plays a fullscreen sliding "page turn" transition. Direction: right-to-left (default) or left-to-right.
/// Assign a fullscreen RectTransform panel (e.g. under a Screen Space - Overlay Canvas); it will be animated off-screen.
/// </summary>
public class PageTurnTransition : MonoBehaviour
{
    [Tooltip("Fullscreen panel to slide (e.g. Image with paper colour). Must be under a Canvas.")]
    public RectTransform pagePanel;
    [Tooltip("Duration of the slide-in and slide-out in seconds.")]
    public float duration = 0.4f;

    private bool _isPlaying;

    /// <summary>
    /// Plays the page turn, calls onMidTransition when the screen is covered (switch room here), then slides out.
    /// </summary>
    /// <param name="rightToLeft">True = page slides in from right (default book turn). False = from left.</param>
    /// <param name="onMidTransition">Called when the screen is fully covered; perform the room switch here.</param>
    public void PlayTransition(bool rightToLeft, Action onMidTransition)
    {
        if (pagePanel == null)
        {
            onMidTransition?.Invoke();
            return;
        }
        if (_isPlaying)
            return;
        StartCoroutine(PlayTransitionCoroutine(rightToLeft, onMidTransition));
    }

    private IEnumerator PlayTransitionCoroutine(bool rightToLeft, Action onMidTransition)
    {
        _isPlaying = true;
        pagePanel.gameObject.SetActive(true);

        float halfDuration = duration * 0.5f;

        if (rightToLeft)
        {
            SetAnchorX(pagePanel, 1f, 2f);
            yield return LerpAnchorX(pagePanel, 1f, 2f, 0f, 1f, halfDuration);
        }
        else
        {
            SetAnchorX(pagePanel, -1f, 0f);
            yield return LerpAnchorX(pagePanel, -1f, 0f, 0f, 1f, halfDuration);
        }

        onMidTransition?.Invoke();

        if (rightToLeft)
            yield return LerpAnchorX(pagePanel, 0f, 1f, -1f, 0f, halfDuration);
        else
            yield return LerpAnchorX(pagePanel, 0f, 1f, 1f, 2f, halfDuration);

        pagePanel.gameObject.SetActive(false);
        _isPlaying = false;
    }

    private static void SetAnchorX(RectTransform rect, float minX, float maxX)
    {
        Vector2 min = rect.anchorMin;
        Vector2 max = rect.anchorMax;
        min.x = minX;
        max.x = maxX;
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static IEnumerator LerpAnchorX(RectTransform rect, float fromMinX, float fromMaxX, float toMinX, float toMaxX, float overDuration)
    {
        float elapsed = 0f;
        while (elapsed < overDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / overDuration);
            t = t * t * (3f - 2f * t);
            float minX = Mathf.Lerp(fromMinX, toMinX, t);
            float maxX = Mathf.Lerp(fromMaxX, toMaxX, t);
            SetAnchorX(rect, minX, maxX);
            yield return null;
        }
        SetAnchorX(rect, toMinX, toMaxX);
    }
}
