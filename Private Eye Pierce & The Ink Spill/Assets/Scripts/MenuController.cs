using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles main menu actions: start game and quit.
/// </summary>
public class MenuController : MonoBehaviour
{
    /// <summary>
    /// Loads the main game scene (OfficeFloor).
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("OfficeFloor");
    }

    /// <summary>
    /// Exits the application. In the editor, stops play mode.
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}