using UnityEngine;

/// <summary>
/// When this scene loads, fades in from black using SceneFader if one exists and no fade is in progress.
/// Add to a GameObject in the OfficeFloor (or any) scene so the first time the scene is shown it fades in.
/// Skips when the scene was loaded via SceneFader.FadeToScene (that path already runs its own fade-in).
/// </summary>
public class FadeInOnLoad : MonoBehaviour
{
    private void Start()
    {
        if (SceneFader.Instance == null)
            return;
        if (SceneFader.Instance.IsFading())
            return;

        SceneFader.Instance.SetToBlack();
        SceneFader.Instance.FadeInNow();
    }
}
