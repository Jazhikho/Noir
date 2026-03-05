using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public GameObject pauseMenu;

    [Header("Scene Names")]
    public string gameSceneName = "Cutscene";
    /// <summary>Scene to load when skipping cutscene on WebGL (e.g. "OfficeFloor"). Must be in build.</summary>
    public string webglSkipToSceneName = "OfficeFloor";
    public string mainMenuSceneName = "MainMenu";

    public static bool IsPaused { get; private set; }

    void Start()
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    public void ShowPauseMenu()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(true);
    }

    public void HidePauseMenu()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
    }

    public void TogglePauseMenu()
    {
        if (pauseMenu == null) return;
        bool willBeVisible = !pauseMenu.activeSelf;
        pauseMenu.SetActive(willBeVisible);
    }

    /// <summary>
    /// Starts the game. On WebGL skips the cutscene and loads Game directly to avoid hang.
    /// </summary>
    public void StartGame()
    {
#if UNITY_WEBGL
        SceneManager.LoadScene(webglSkipToSceneName);
#else
        PlayCutsceneNormally();
#endif
    }

    /// <summary>
    /// Loads and plays the cutscene scene (used on non-WebGL platforms).
    /// </summary>
    private void PlayCutsceneNormally()
    {
        Jukebox jukebox = FindFirstObjectByType<Jukebox>();

        if (SceneFader.Instance == null)
        {
            if (jukebox != null)
                jukebox.Stop();
            SceneManager.LoadScene(gameSceneName);
            return;
        }

        if (jukebox != null)
            jukebox.FadeOut(SceneFader.Instance.fadeDuration);

        SceneFader.Instance.FadeToScene(gameSceneName);
    }

    public void LoadScene(string sceneName)
    {
        if (SceneFader.Instance == null)
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        if (sceneName == mainMenuSceneName || sceneName == "OfficeFloor")
        {
            Jukebox jukebox = FindFirstObjectByType<Jukebox>();
            if (jukebox != null)
                jukebox.FadeOut(SceneFader.Instance.fadeDuration);
        }

        SceneFader.Instance.FadeToScene(sceneName);
    }

    public void ReturnToMainMenu()
    {
        LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}
