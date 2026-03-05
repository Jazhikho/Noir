using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Runs a cutscene from start to finish using a state machine.
/// Assign a CutsceneConfig asset to control timing, transitions, and skip behavior.
/// The skip bar, fade, and scene loading are handled by separate components.
/// </summary>
public class CutsceneOrchestrator : MonoBehaviour
{
    // Every state the cutscene can be in, shown in the Inspector for debugging
    public enum State
    {
        Idle,
        WaitingForFadeIn,
        PreDelay,
        Preparing,
        FadingInDisplay,
        Playing,
        FadingOutDisplay,
        PostDelay,
        LoadingScene,
        Complete
    }

    [Header("Cutscene Setup")]
    [Tooltip("The config asset that defines timing, skip, and target scene")]
    public CutsceneConfig config;

    [Header("Video")]
    public VideoPlayer videoPlayer;
    [Tooltip("Optional override - leave empty to use the VideoPlayer's clip")]
    public VideoClip videoClip;
    public RawImage videoDisplay;

    [Header("Components (auto-found if left empty)")]
    public CutsceneFadeController fadeController;
    public CutsceneSkipController skipController;
    public CutsceneSkipUI skipUI;

    [Header("Debug")]
    [Tooltip("Prints state changes to the console")]
    [SerializeField] private bool debugLogging = false;

    // Visible in the Inspector so QA can see where the cutscene is at
    [Header("Runtime (read-only)")]
    [SerializeField] private State currentState = State.Idle;

    public State CurrentState => currentState;

    private bool skipRequested;
    private bool videoEnded;

    void Start()
    {
        if (config == null)
        {
            Debug.LogError("CutsceneOrchestrator: No CutsceneConfig assigned! Can't run the cutscene.");
            return;
        }

        // Silence any music left over from the previous scene
        Jukebox jukebox = FindFirstObjectByType<Jukebox>();
        if (jukebox != null)
            jukebox.Stop();

        // Auto-wire components if they weren't dragged in manually
        if (fadeController == null) fadeController = GetComponent<CutsceneFadeController>();
        if (skipController == null) skipController = GetComponent<CutsceneSkipController>();
        if (skipUI == null) skipUI = GetComponentInChildren<CutsceneSkipUI>();

        // Point the fade controller at our video display
        if (fadeController != null)
        {
            if (videoDisplay != null)
                fadeController.SetDisplay(videoDisplay);
            fadeController.SetAlpha(0f);
        }
        else if (videoDisplay != null)
        {
            Color c = videoDisplay.color;
            c.a = 0f;
            videoDisplay.color = c;
        }

        // Find the video player
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer == null)
        {
            Debug.LogError("CutsceneOrchestrator: No VideoPlayer found. Skipping to next scene.");
            LoadFinalScene();
            return;
        }

        // Get the skip system ready but don't turn it on yet
        if (skipController != null)
        {
            skipController.Configure(config.skipHoldSeconds, config.skipDecaySeconds);
            skipController.RequestSkip += HandleSkipRequest;
            skipController.SetEnabled(false);
        }

        // Wire up the skip UI
        if (skipUI != null && skipController != null)
            skipUI.SetController(skipController);

        StartCoroutine(RunCutscene());
    }

    void SetState(State newState)
    {
        currentState = newState;
        if (debugLogging)
            Debug.Log($"[Cutscene] {newState}");
    }

    // The main cutscene pipeline - each step runs in order
    IEnumerator RunCutscene()
    {
        // Wait for the scene to finish fading in from black
        SetState(State.WaitingForFadeIn);
        if (SceneFader.Instance != null)
        {
            while (SceneFader.Instance.IsFading())
                yield return null;
        }
        if (skipRequested) { yield return FinalizeCutscene(); yield break; }

        // Brief pause before the video
        SetState(State.PreDelay);
        yield return new WaitForSeconds(config.delayBeforeVideo);
        if (skipRequested) { yield return FinalizeCutscene(); yield break; }

        // Load the video into memory
        SetState(State.Preparing);
        if (videoClip != null)
            videoPlayer.clip = videoClip;
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
            yield return null;
        videoPlayer.loopPointReached += OnVideoEnd;
        if (skipRequested) { yield return FinalizeCutscene(); yield break; }

        // Fade the video display in
        SetState(State.FadingInDisplay);
        if (fadeController != null)
            yield return fadeController.FadeTo(1f, config.videoFadeInDuration);
        if (skipRequested) { yield return FinalizeCutscene(); yield break; }

        // Play the video and show the skip bar after a delay
        SetState(State.Playing);
        videoPlayer.Play();
        videoEnded = false;

        if (config.skipEnabled && skipController != null)
            StartCoroutine(EnableSkipAfterDelay());

        // Sit here until the video finishes or the player skips
        while (!videoEnded && !skipRequested)
            yield return null;

        yield return FinalizeCutscene();
    }

    // Safely ends the cutscene from any state
    IEnumerator FinalizeCutscene()
    {
        // Turn off skip input
        if (skipController != null)
            skipController.SetEnabled(false);

        // Stop the video if it's still going
        if (videoPlayer != null && videoPlayer.isPlaying)
            videoPlayer.Stop();

        // Fade out the video display (if it's still visible)
        if (fadeController != null && fadeController.CurrentAlpha > 0.01f)
        {
            SetState(State.FadingOutDisplay);
            yield return fadeController.FadeTo(0f, config.videoFadeOutDuration);
        }

        // Small pause before switching scenes
        SetState(State.PostDelay);
        yield return new WaitForSeconds(config.delayAfterVideo);

        // Go to the next scene
        SetState(State.LoadingScene);
        LoadFinalScene();
        SetState(State.Complete);
    }

    void LoadFinalScene()
    {
        if (SceneFader.Instance != null)
            SceneFader.Instance.FadeToScene(config.nextSceneName);
        else
            SceneManager.LoadScene(config.nextSceneName);
    }

    IEnumerator EnableSkipAfterDelay()
    {
        yield return new WaitForSeconds(config.skipAppearDelay);
        // Only activate if we're still playing (might have ended naturally)
        if (currentState == State.Playing && skipController != null)
            skipController.SetEnabled(true);
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        videoEnded = true;
    }

    void HandleSkipRequest()
    {
        skipRequested = true;
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoEnd;
        if (skipController != null)
            skipController.RequestSkip -= HandleSkipRequest;
    }
}
