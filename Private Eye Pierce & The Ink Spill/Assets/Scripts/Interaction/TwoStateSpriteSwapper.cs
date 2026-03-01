using UnityEngine;

/// <summary>
/// Swaps a SpriteRenderer to a "checked" sprite when invoked. Use from Interactable On Interact
/// (click = swap) or from SearchableProp OnSearchComplete (search then swap). Optional one-time-only behaviour.
/// </summary>
public class TwoStateSpriteSwapper : MonoBehaviour
{
    [Header("Sprite Renderer")]
    [Tooltip("Leave unset to use SpriteRenderer on this GameObject.")]
    public SpriteRenderer targetRenderer;

    [Header("Sprites")]
    [Tooltip("Sprite to show after interaction (e.g. checked fridge). Required.")]
    public Sprite checkedSprite;

    [Header("Behaviour")]
    [Tooltip("If true, swap only happens once; further calls do nothing.")]
    public bool swapOnlyOnce = true;

    private bool _hasSwapped;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Swaps the target renderer to the checked sprite. Call from Interactable On Interact or SearchableProp OnSearchComplete.
    /// </summary>
    public void SwapToCheckedSprite()
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

        targetRenderer.sprite = checkedSprite;
        _hasSwapped = true;
    }

    /// <summary>
    /// Returns true if the sprite has already been swapped to checked (when swapOnlyOnce is true).
    /// </summary>
    public bool HasSwappedToChecked()
    {
        return _hasSwapped;
    }
}
