using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;
    public Image fadePanel;
    public float fadeDuration = 1f;

    private bool isFading = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            Color c = fadePanel.color;
            c.a = 1f;
            fadePanel.color = c;
        }
    }

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(string sceneName)
    {
        if (isFading) return;
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    public void FadeToSceneWithCallback(string sceneName, Action onFadeOutComplete)
    {
        if (isFading) return;
        StartCoroutine(FadeOutLoadAndCallback(sceneName, onFadeOutComplete));
    }

    public IEnumerator FadeToBlack(float duration)
    {
        if (fadePanel == null) yield break;
        fadePanel.gameObject.SetActive(true);
        Color color = fadePanel.color;
        float startAlpha = color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 1f, Mathf.Clamp01(elapsed / duration));
            fadePanel.color = color;
            yield return null;
        }

        color.a = 1f;
        fadePanel.color = color;
    }

    public IEnumerator FadeIn()
    {
        if (fadePanel == null) yield break;

        isFading = true;
        fadePanel.gameObject.SetActive(true);
        float elapsed = 0f;
        Color color = fadePanel.color;
        color.a = 1f;
        fadePanel.color = color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = 0f;
        fadePanel.color = color;
        isFading = false;
    }

    public IEnumerator FadeOut()
    {
        if (fadePanel == null) yield break;

        isFading = true;
        fadePanel.gameObject.SetActive(true);
        float elapsed = 0f;
        Color color = fadePanel.color;
        color.a = 0f;
        fadePanel.color = color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = 1f;
        fadePanel.color = color;
        isFading = false;
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        isFading = true;

        // Fade to black
        yield return StartCoroutine(FadeOut());

        // Load scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Wait until scene is ready
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Activate scene
        asyncLoad.allowSceneActivation = true;

        // Wait for scene to fully load
        yield return null;

        // Fade back in
        yield return StartCoroutine(FadeIn());

        isFading = false;
    }

    IEnumerator FadeOutLoadAndCallback(string sceneName, Action onFadeOutComplete)
    {
        isFading = true;

        // Fade to black
        yield return StartCoroutine(FadeOut());

        // Load scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;
        yield return null;

        // Callback after scene loads but before fade in
        onFadeOutComplete?.Invoke();

        yield return StartCoroutine(FadeIn());

        isFading = false;
    }

    public bool IsFading() => isFading;

    public void FadeInNow()
    {
        StartCoroutine(FadeIn());
    }
}
