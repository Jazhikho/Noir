using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Shows or hides an overlay GameObject (e.g. a child with a sprite) on top of the current object.
/// Use ShowOverlay() when "opening" and HideOverlay() when "closing". OnOverlayShown fires when
/// the overlay is shown (e.g. wire to play "Fridge Opened" sound).
/// </summary>
public class OverlaySpriteOnInteract : MonoBehaviour
{
    [Header("Overlay")]
    [Tooltip("GameObject to show on top (e.g. child with SpriteRenderer).")]
    public GameObject overlayToShow;

    [Header("Behaviour")]
    [Tooltip("If true, ShowOverlay() does nothing after the first show until HideOverlay() is called (allows open/close toggle).")]
    public bool showOnlyOnce = false;

    [Tooltip("If true, forces overlayToShow inactive on Awake/OnValidate to avoid starting the scene already open.")]
    public bool enforceStartHidden = true;

    [Header("Interaction")]
    [Tooltip("If true, ends the current Interactable interaction after showing/hiding so the player can move again.")]
    public bool endInteractionAfterAction = true;

    [Header("Events")]
    [Tooltip("Fires when the overlay is shown. Use for e.g. Fridge Opened sound.")]
    public UnityEvent OnOverlayShown;

    [Tooltip("Fires when the overlay is hidden. Use for e.g. Fridge Closed sound.")]
    public UnityEvent OnOverlayHidden;

    private bool _hasShown;

    private void Awake()
    {
        if (overlayToShow == null)
            return;

        if (enforceStartHidden && overlayToShow.activeSelf)
        {
            // KEVIN EDIT - Hard-enforce the intended prefab setup.
            // Starting open makes the first interaction feel broken (and can fire "open" SFX when nothing changed). Oops.
            overlayToShow.SetActive(false);
        }

        _hasShown = overlayToShow.activeSelf;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (overlayToShow == null)
            return;

        if (!Application.isPlaying && enforceStartHidden && overlayToShow.activeSelf)
        {
            // KEVIN EDIT - Prevent accidentally saving the prefab/scene in an "already open" state.
            // This is the most common cause of any weird first-click behavior and duplicate open/close audio.
            overlayToShow.SetActive(false);
        }
    }
#endif

    /// <summary>
    /// Shows the overlay GameObject and invokes OnOverlayShown. Call from Interactable On Interact (e.g. open fridge).
    /// </summary>
    public void ShowOverlay()
    {
        try
        {
            if (showOnlyOnce && _hasShown)
                return;

            if (overlayToShow == null)
            {
                Debug.LogWarning("OverlaySpriteOnInteract: Overlay To Show is not assigned.", this);
                return;
            }

            if (overlayToShow.activeSelf)
            {
                // KEVIN EDIT - Avoid double-firing events when something calls ShowOverlay twice.
                // Audio and other listeners should only run when the state actually changes.
                _hasShown = true;
                return;
            }

            overlayToShow.SetActive(true);
            _hasShown = true;
            OnOverlayShown?.Invoke();
        }
        finally
        {
            if (endInteractionAfterAction)
            {
                // KEVIN EDIT - PointClickController disables player movement on arrival at an Interactable.
                // If we do not end the interaction for quick "toggle" props, the player stays frozen after clicking.
                Interactable.EndCurrentInteraction();
            }
        }
    }

    /// <summary>
    /// If overlay is visible, hides it; otherwise shows it.
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
        try
        {
            if (overlayToShow == null)
                return;

            if (!overlayToShow.activeSelf)
            {
                // KEVIN EDIT - Same reasoning as ShowOverlay: do not double-fire "close" logic.
                // We still release movement in finally so an interaction cannot lock the player.
                _hasShown = false;
                return;
            }

            overlayToShow.SetActive(false);
            _hasShown = false;
            OnOverlayHidden?.Invoke();
        }
        finally
        {
            if (endInteractionAfterAction)
            {
                // KEVIN EDIT - Always release movement after open/close interactions.
                // Even when nothing changes, the click still counts as an interaction and should not lock the player.
                Interactable.EndCurrentInteraction();
            }
        }
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