using UnityEngine;

/// <summary>
/// Swaps a SpriteRenderer to a "checked" sprite when invoked. Use from Interactable On Interact
/// (click = swap) or from SearchableProp OnSearchComplete (search then swap).
/// </summary>
public class TwoStateSpriteSwapper : MonoBehaviour
{
    [Header("Sprite Renderer")]
    [Tooltip("Leave unset to use SpriteRenderer on this GameObject.")]
    public SpriteRenderer targetRenderer;

    [Header("Sprites")]
    [Tooltip("Sprite to show after interaction (e.g. checked fridge). Required.")]
    public Sprite checkedSprite;

    [Tooltip("Optional: sprite to restore when resetting. If unset, we cache whatever sprite was on the renderer at Awake.")]
    public Sprite uncheckedSpriteOverride;

    [Header("Behaviour")]
    [Tooltip("If true, swap only happens once; further calls do nothing.")]
    public bool swapOnlyOnce = true;

    [Header("Interaction")]
    [Tooltip("If true, ends the current Interactable interaction after swapping so the player can move again.")]
    public bool endInteractionAfterSwap = true;

    private bool _hasSwapped;
    private Sprite _cachedUncheckedSprite;
    private bool _cachedUncheckedSpriteValid;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<SpriteRenderer>();

        if (targetRenderer == null)
            return;

        if (uncheckedSpriteOverride != null)
        {
            _cachedUncheckedSprite = uncheckedSpriteOverride;
            _cachedUncheckedSpriteValid = true;
        }
        else
        {
            // KEVIN EDIT - Cache the initial sprite so we can safely reset later if needed.
            // This keeps the component true to its name (it actually has two states now).
            _cachedUncheckedSprite = targetRenderer.sprite;
            _cachedUncheckedSpriteValid = _cachedUncheckedSprite != null;
        }
    }

    /// <summary>
    /// Swaps the target renderer to the checked sprite. Call from Interactable On Interact or SearchableProp OnSearchComplete.
    /// </summary>
    public void SwapToCheckedSprite()
    {
        try
        {
            if (swapOnlyOnce && _hasSwapped)
                return;

            if (targetRenderer == null)
            {
                Debug.LogWarning("TwoStateSpriteSwapper: No SpriteRenderer assigned and none on this GameObject.", this);
                return;
            }

            if (checkedSprite == null)
            {
                Debug.LogWarning("TwoStateSpriteSwapper: Checked Sprite is not assigned.", this);
                return;
            }

            if (targetRenderer.sprite == checkedSprite)
            {
                // KEVIN EDIT - Avoid doing extra work (and avoid side effects if you add events later).
                // Still mark swapped so one-time props do not keep trying forever.
                _hasSwapped = true;
                return;
            }

            targetRenderer.sprite = checkedSprite;
            _hasSwapped = true;
        }
        finally
        {
            if (endInteractionAfterSwap)
            {
                // KEVIN EDIT - PointClickController disables player movement on arrival at an Interactable.
                // If this action is the only thing wired to OnInteract, we must end the interaction or the player freezes.
                Interactable.EndCurrentInteraction();
            }
        }
    }

    /// <summary>
    /// Restores the renderer to the cached "unchecked" sprite (or override). This also clears the one-time flag.
    /// </summary>
    public void ResetToUncheckedSprite()
    {
        try
        {
            if (targetRenderer == null)
            {
                Debug.LogWarning("TwoStateSpriteSwapper: No SpriteRenderer assigned and none on this GameObject.", this);
                return;
            }

            if (!_cachedUncheckedSpriteValid)
            {
                Debug.LogWarning("TwoStateSpriteSwapper: No unchecked sprite available to reset to.", this);
                return;
            }

            targetRenderer.sprite = _cachedUncheckedSprite;
            _hasSwapped = false;
        }
        finally
        {
            if (endInteractionAfterSwap)
            {
                // KEVIN EDIT - Same interaction rule as swapping: never let a click-based action lock the player.
                // Reset might be used by an Interactable in some puzzles or debug setups.
                Interactable.EndCurrentInteraction();
            }
        }
    }

    /// <summary>
    /// Returns true if the sprite has already been swapped to checked (when swapOnlyOnce is true).
    /// </summary>
    public bool HasSwappedToChecked()
    {
        return _hasSwapped;
    }
}