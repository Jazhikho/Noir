using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class CutscenePlayer : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;
    public VideoClip videoClip;
    public RawImage videoDisplay;

    [Header("Transition")]
    public string nextSceneName = "OfficeFloor";

    [Header("Skip Settings")]
    public bool allowSkip = true;
    public float skipDelay = 1f;
    public GameObject skipPrompt;

    [Header("Timing")]
    public float delayBeforeVideo = 0.5f;
    public float delayAfterVideo = 0.3f;

    private bool canSkip = false;
    private bool isTransitioning = false;
    private bool videoStarted = false;

    void Start()
    {
        // Stop any music from previous scene
        Jukebox jukebox = FindFirstObjectByType<Jukebox>();
        if (jukebox != null)
            jukebox.Stop();

        // Hide skip prompt initially
        if (skipPrompt != null)
            skipPrompt.SetActive(false);

        // Hide video display until ready
        if (videoDisplay != null)
        {
            Color c = videoDisplay.color;
            c.a = 0f;
            videoDisplay.color = c;
        }

        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer == null)
        {
            Debug.LogError("CutscenePlayer: No VideoPlayer found.");
            StartCoroutine(LoadNextSceneAfterDelay(0.5f));
            return;
        }

        StartCoroutine(PlayCutsceneSequence());
    }

    IEnumerator PlayCutsceneSequence()
    {
        // Wait for scene fade-in to complete
        if (SceneFader.Instance != null)
        {
            while (SceneFader.Instance.IsFading())
            {
                yield return null;
            }
        }

        // Small delay before starting video
        yield return new WaitForSeconds(delayBeforeVideo);

        // Set up video
        if (videoClip != null)
            videoPlayer.clip = videoClip;

        videoPlayer.Prepare();

        // Wait for video to prepare
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        // Subscribe to end event
        videoPlayer.loopPointReached += OnVideoEnd;

        // Fade in video display
        if (videoDisplay != null)
        {
            yield return StartCoroutine(FadeVideoDisplay(0f, 1f, 0.3f));
        }

        // Play video
        videoPlayer.Play();
        videoStarted = true;

        // Enable skip after delay
        if (allowSkip)
        {
            StartCoroutine(EnableSkipAfterDelay());
        }
    }

    IEnumerator FadeVideoDisplay(float from, float to, float duration)
    {
        if (videoDisplay == null) yield break;

        float elapsed = 0f;
        Color c = videoDisplay.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            c.a = Mathf.Lerp(from, to, t);
            videoDisplay.color = c;
            yield return null;
        }

        c.a = to;
        videoDisplay.color = c;
    }

    IEnumerator EnableSkipAfterDelay()
    {
        yield return new WaitForSeconds(skipDelay);
        canSkip = true;

        if (skipPrompt != null)
            skipPrompt.SetActive(true);
    }

    void Update()
    {
        if (!videoStarted || isTransitioning) return;

        if (allowSkip && canSkip)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
            {
                SkipCutscene();
            }
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(TransitionToNextScene());
    }

    public void SkipCutscene()
    {
        if (isTransitioning) return;

        if (videoPlayer != null && videoPlayer.isPlaying)
            videoPlayer.Stop();

        StartCoroutine(TransitionToNextScene());
    }

    IEnumerator TransitionToNextScene()
    {
        if (isTransitioning) yield break;
        isTransitioning = true;

        // Hide skip prompt
        if (skipPrompt != null)
            skipPrompt.SetActive(false);

        // Fade out video display
        if (videoDisplay != null)
        {
            yield return StartCoroutine(FadeVideoDisplay(1f, 0f, 0.3f));
        }

        // Small delay before scene transition
        yield return new WaitForSeconds(delayAfterVideo);

        // Load next scene
        if (SceneFader.Instance != null)
            SceneFader.Instance.FadeToScene(nextSceneName);
        else
            SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (SceneFader.Instance != null)
            SceneFader.Instance.FadeToScene(nextSceneName);
        else
            SceneManager.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoEnd;
    }
}
