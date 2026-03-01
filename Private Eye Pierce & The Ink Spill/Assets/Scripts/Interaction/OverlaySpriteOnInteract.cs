using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Shows or hides an overlay GameObject (e.g. a child with a sprite) on top of the current object.
/// Use ShowOverlay() when "opening" and HideOverlay() when "closing". OnOverlayShown fires when
/// the overlay is shown (e.g. wire to play "Fridge Opened" sound). For "close unless searching",
/// put Interactable + SearchableProp on the overlay child so clicking the open fridge runs search;
/// put open/close on the parent so clicking the door/frame toggles the overlay.
/// </summary>
public class OverlaySpriteOnInteract : MonoBehaviour
{
    [Header("Overlay")]
    [Tooltip("GameObject to show on top (e.g. child with SpriteRenderer). Must start disabled.")]
    public GameObject overlayToShow;

    [Header("Behaviour")]
    [Tooltip("If true, ShowOverlay() does nothing after the first show until HideOverlay() is called (allows open/close toggle).")]
    public bool showOnlyOnce = false;

    [Header("Events")]
    [Tooltip("Fires when the overlay is shown. Use for e.g. Fridge Opened sound.")]
    public UnityEvent OnOverlayShown;
    [Tooltip("Fires when the overlay is hidden. Use for e.g. Fridge Closed sound.")]
    public UnityEvent OnOverlayHidden;

    private bool _hasShown;

    /// <summary>
    /// Shows the overlay GameObject and invokes OnOverlayShown. Call from Interactable On Interact (e.g. open fridge).
    /// </summary>
    public void ShowOverlay()
    {
        if (showOnlyOnce && _hasShown)
            return;

        if (overlayToShow == null)
        {
            Debug.LogWarning("OverlaySpriteOnInteract: Overlay To Show is not assigned.", this);
            return;
        }

        overlayToShow.SetActive(true);
        _hasShown = true;
        OnOverlayShown?.Invoke();
    }

    /// <summary>
    /// If overlay is visible, hides it; otherwise shows it. Use on the parent so one Interactable can open/close. Clicking the overlay (child) can then run search instead.
    /// </summary>
    public void ToggleOverlay()
    {
        if (IsOverlayVisible())
            HideOverlay();
        else
            ShowOverlay();
    }

    /// <summary>
    /// Hides the overlay and allows it to be shown again. Invokes OnOverlayHidden (e.g. for close sound).
    /// </summary>
    public void HideOverlay()
    {
        if (overlayToShow == null)
            return;

        overlayToShow.SetActive(false);
        _hasShown = false;
        OnOverlayHidden?.Invoke();
    }

    /// <summary>
    /// Returns true if the overlay is currently visible.
    /// </summary>
    public bool IsOverlayVisible()
    {
        return overlayToShow != null && overlayToShow.activeSelf;
    }

    /// <summary>
    /// Returns true if the overlay has been shown at least once this "cycle" (resets after HideOverlay).
    /// </summary>
    public bool HasOverlayBeenShown()
    {
        return _hasShown;
    }
}
