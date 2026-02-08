using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public GameObject pauseMenu;

    /// <summary>
    /// True when the pause/quit menu is open. Game scripts should skip input and movement when this is true.
    /// </summary>
    public static bool IsPaused { get; private set; }

    void Start()
    {
        // Make sure pause menu starts hidden and game is unpaused
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        //ApplyPausedState(false);
    }

    /// <summary>
    /// Shows the pause/quit menu, pauses the game, and enables the cursor for menu interaction.
    /// </summary>
    public void ShowPauseMenu()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(true);
        //ApplyPausedState(true);
    }

    /// <summary>
    /// Hides the pause menu and unpauses the game (e.g. Continue or top-left button to close).
    /// </summary>
    public void HidePauseMenu()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
        //ApplyPausedState(false);
    }

    /// <summary>
    /// Toggles the pause menu: when opened, game pauses and cursor is active for menu; when closed, game unpauses.
    /// </summary>
    public void TogglePauseMenu()
    {
        if (pauseMenu == null) return;
        bool willBeVisible = !pauseMenu.activeSelf;
        pauseMenu.SetActive(willBeVisible);
        //ApplyPausedState(willBeVisible);
    }

    /// <summary>
    /// Applies time scale, cursor visibility/lock, and IsPaused based on whether the game is paused (menu open).
    /// </summary>
    /// <param name="paused">True when pause menu is open, false when playing.</param>
    /*private void ApplyPausedState(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }*/

    /// <summary>
    /// Loads the main game scene (OfficeFloor). Uses SceneFader when available for a fade transition. Fades out the Jukebox over the same duration as the screen fade.
    /// </summary>
    public void StartGame()
    {
        if (SceneFader.Instance == null)
        {
            SceneManager.LoadScene("OfficeFloor");
            return;
        }

        Jukebox jukebox = FindFirstObjectByType<Jukebox>();
        if (jukebox != null)
            jukebox.FadeOut(SceneFader.Instance.fadeDuration);

        SceneFader.Instance.FadeToScene("OfficeFloor");
    }

    /// <summary>
    /// Loads a scene. When loading MainMenu or OfficeFloor, fades out the Jukebox over the same duration as the screen fade.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (SceneFader.Instance == null)
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        string mainMenuScene = "MainMenu";
        string officeScene = "OfficeFloor";
        if (sceneName == mainMenuScene || sceneName == officeScene)
        {
            Jukebox jukebox = FindFirstObjectByType<Jukebox>();
            if (jukebox != null)
                jukebox.FadeOut(SceneFader.Instance.fadeDuration);
        }

        SceneFader.Instance.FadeToScene(sceneName);
    }

    /// <summary>
    /// Quits the application. Restores time scale before quitting so the game exits cleanly.
    /// Call from the pause menu's "Quit" option (not the top-left button that opens the menu).
    /// </summary>
    public void QuitGame()
    {
        Time.timeScale = 1f;
        if (UnityEditor.EditorApplication.isPlaying == true)
        { 
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }
}
