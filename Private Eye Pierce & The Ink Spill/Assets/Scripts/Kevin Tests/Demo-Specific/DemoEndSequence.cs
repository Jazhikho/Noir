using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class DemoEndSequence : MonoBehaviour
{
    [Header("Fade")]
    public Image fadeImage;
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1.5f;

    [Header("To Be Continued (Use TMP or Legacy Text)")]
    public TMP_Text tmpText;
    public Text legacyText;
    public float textDisplayDuration = 3f;
    public float textFadeDuration = 0.5f;

    [Header("Scene Transition")]
    public string mainMenuSceneName = "MainMenu";
    public float sceneLoadDelay = 1f;

    public static DemoEndSequence Instance { get; private set; }

    private bool isPlaying = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.gameObject.SetActive(false);
        }

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.gameObject.SetActive(false);
        }

        if (tmpText != null)
        {
            Color c = tmpText.color;
            c.a = 0f;
            tmpText.color = c;
            tmpText.gameObject.SetActive(false);
        }

        if (legacyText != null)
        {
            Color c = legacyText.color;
            c.a = 0f;
            legacyText.color = c;
            legacyText.gameObject.SetActive(false);
        }
    }

    public void PlayToBeContinuedAndReturnToMenu()
    {
        if (isPlaying) return;
        StartCoroutine(EndSequenceCoroutine());
    }

    public void PlayEndSequence()
    {
        PlayToBeContinuedAndReturnToMenu();
    }

    IEnumerator EndSequenceCoroutine()
    {
        isPlaying = true;

        Interactable.EndCurrentInteraction();
        var player = FindFirstObjectByType<PointClickController>();
        if (player != null)
            player.DisableMovement();

        if (AdventureHUDController.Instance != null)
            AdventureHUDController.Instance.SetHUDEnabled(false);

        Debug.Log("DEMO END: Starting end sequence...");

        yield return StartCoroutine(FadeToBlack());

        yield return StartCoroutine(ShowToBeContinued());

        yield return new WaitForSeconds(sceneLoadDelay);

        Debug.Log("DEMO END: Loading " + mainMenuSceneName + "...");

        if (!string.IsNullOrEmpty(mainMenuSceneName))
            SceneManager.LoadScene(mainMenuSceneName);
        else
            Debug.LogWarning("DemoEndSequence: No main menu scene specified.");
    }

    IEnumerator FadeToBlack()
    {
        if (fadeImage != null)
            fadeImage.gameObject.SetActive(true);

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.gameObject.SetActive(true);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = t;
                fadeImage.color = c;
            }

            if (fadeCanvasGroup != null)
                fadeCanvasGroup.alpha = t;

            yield return null;
        }

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
        }

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 1f;
    }

    IEnumerator ShowToBeContinued()
    {
        if (tmpText == null && legacyText == null) yield break;

        if (tmpText != null)
            tmpText.gameObject.SetActive(true);

        if (legacyText != null)
            legacyText.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < textFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / textFadeDuration);

            if (tmpText != null)
            {
                Color c = tmpText.color;
                c.a = t;
                tmpText.color = c;
            }

            if (legacyText != null)
            {
                Color c = legacyText.color;
                c.a = t;
                legacyText.color = c;
            }

            yield return null;
        }

        if (tmpText != null)
        {
            Color c = tmpText.color;
            c.a = 1f;
            tmpText.color = c;
        }

        if (legacyText != null)
        {
            Color c = legacyText.color;
            c.a = 1f;
            legacyText.color = c;
        }

        Debug.Log("DEMO END: To be continued...");

        yield return new WaitForSeconds(textDisplayDuration);
    }
}