using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows the "hold to skip" progress bar. This widget is intentionally dumb -
/// it just reads progress from the SkipController and fills the bar.
/// It doesn't know what cutscene is playing or what skipping does.
///
/// Uses a CanvasGroup to show/hide so it never disables its own Update loop.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class CutsceneSkipUI : MonoBehaviour
{
    [Tooltip("The SkipController to read progress from")]
    [SerializeField] private CutsceneSkipController skipController;

    [Tooltip("The fill bar image (set Image Type to Filled)")]
    [SerializeField] private Image fillBar;

    private CanvasGroup canvasGroup;

    public void SetController(CutsceneSkipController controller) => skipController = controller;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        SetVisible(false);
    }

    void Update()
    {
        if (skipController == null) return;

        bool visible = skipController.IsActive;
        SetVisible(visible);

        if (fillBar != null && visible)
            fillBar.fillAmount = skipController.Progress;
    }

    void SetVisible(bool show)
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = show ? 1f : 0f;
        canvasGroup.blocksRaycasts = show;
    }
}
