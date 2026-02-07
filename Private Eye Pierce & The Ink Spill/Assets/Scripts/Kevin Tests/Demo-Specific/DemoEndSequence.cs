using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DemoEndSequence : MonoBehaviour
{
    [Header("Fade")]
    public Image fadeImage;
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1.5f;

    [Header("To Be Continued")]
    public Text toBeContinuedText;
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

        if (toBeContinuedText != null)
        {
            Color c = toBeContinuedText.color;
            c.a = 0f;
            toBeContinuedText.color = c;
            toBeContinuedText.gameObject.SetActive(false);
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
        if (toBeContinuedText == null) yield break;

        toBeContinuedText.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < textFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / textFadeDuration);

            Color c = toBeContinuedText.color;
            c.a = t;
            toBeContinuedText.color = c;

            yield return null;
        }

        Color finalColor = toBeContinuedText.color;
        finalColor.a = 1f;
        toBeContinuedText.color = finalColor;

        Debug.Log("DEMO END: To be continued...");

        yield return new WaitForSeconds(textDisplayDuration);
    }
}
