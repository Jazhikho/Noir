using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class FadeGroup : MonoBehaviour
{
    [Header("Renderers")]
    public List<SpriteRenderer> renderers = new List<SpriteRenderer>();

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;
    public AnimationCurve fadeCurve;

    [Header("On Complete")]
    public bool disableOnComplete = false;
    public GameObject rootToDisable;

    [Header("Events")]
    public UnityEvent OnFadeOutComplete;
    public UnityEvent OnFadeInComplete;

    private MaterialPropertyBlock propBlock;
    private Coroutine fadeCoroutine;
    private static readonly int ColorID = Shader.PropertyToID("_Color");

    void Awake()
    {
        propBlock = new MaterialPropertyBlock();

        if (renderers.Count == 0)
            renderers.AddRange(GetComponentsInChildren<SpriteRenderer>());

        if (rootToDisable == null)
            rootToDisable = gameObject;

        if (fadeCurve == null || fadeCurve.keys.Length == 0)
            fadeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    }

    public void FadeOut()
    {
        StartFade(0f, OnFadeOutComplete);
    }

    public void FadeIn()
    {
        StartFade(1f, OnFadeInComplete);
    }

    public void SetAlpha(float alpha)
    {
        foreach (var sr in renderers)
        {
            if (sr == null) continue;

            sr.GetPropertyBlock(propBlock);
            Color color = sr.color;
            color.a = alpha;
            propBlock.SetColor(ColorID, color);
            sr.SetPropertyBlock(propBlock);
        }
    }

    void StartFade(float targetAlpha, UnityEvent onComplete)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeCoroutine(targetAlpha, onComplete));
    }

    IEnumerator FadeCoroutine(float targetAlpha, UnityEvent onComplete)
    {
        float startAlpha = GetCurrentAlpha();
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float curveT = fadeCurve.Evaluate(t);
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, curveT);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(targetAlpha);

        if (disableOnComplete && targetAlpha <= 0f && rootToDisable != null)
            rootToDisable.SetActive(false);

        onComplete?.Invoke();
        fadeCoroutine = null;
    }

    float GetCurrentAlpha()
    {
        if (renderers.Count > 0 && renderers[0] != null)
            return renderers[0].color.a;
        return 1f;
    }
}
