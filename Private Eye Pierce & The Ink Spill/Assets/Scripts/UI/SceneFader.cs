using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;
    public Image fadePanel;
    public float fadeDuration = 1f;

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
        }
    }

    void Start()
    {
        // Fade in on first scene
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    /// <summary>
    /// Fades the fade panel to black over the given duration. Yields until complete. Does not load a scene or fade in after.
    /// Used by DemoEndSequence and any other flow that needs a fade-to-black without a scene change.
    /// </summary>
    /// <param name="duration">Time in seconds to fade from current alpha to 1.</param>
    /// <returns>Coroutine enumerator.</returns>
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

    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color color = fadePanel.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = 0f;
        fadePanel.color = color;
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        float elapsed = 0f;
        Color color = fadePanel.color;

        // Fade out
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }

        // Load scene
        SceneManager.LoadScene(sceneName);

        // Wait one frame for scene to load
        yield return null;

        // Fade in
        StartCoroutine(FadeIn());
    }
}
