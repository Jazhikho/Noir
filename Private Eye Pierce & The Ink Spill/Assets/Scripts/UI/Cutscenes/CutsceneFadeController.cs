using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fades the video display in and out. Just handles the visual alpha, nothing else.
/// </summary>
public class CutsceneFadeController : MonoBehaviour
{
    [SerializeField] private RawImage display;

    public float CurrentAlpha => display != null ? display.color.a : 0f;

    public void SetDisplay(RawImage rawImage) => display = rawImage;

    public void SetAlpha(float alpha)
    {
        if (display == null) return;
        Color c = display.color;
        c.a = alpha;
        display.color = c;
    }

    // Smoothly fades to the target alpha over the given duration
    public IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (display == null) yield break;

        float startAlpha = display.color.a;
        float elapsed = 0f;
        Color c = display.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, Mathf.Clamp01(elapsed / duration));
            display.color = c;
            yield return null;
        }

        c.a = targetAlpha;
        display.color = c;
    }
}
