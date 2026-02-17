using UnityEngine;

/// <summary>
/// Optional feedback when the cursor hovers an interactable: Pierce animation, object highlight sprite or animation, and a sound cue.
/// Add to the same GameObject (or a child) as an IInteractable; ClickController2D will call NotifyHover when hover starts or ends.
/// </summary>
public class HoverFeedback : MonoBehaviour
{
    [Header("Pierce Animation")]
    /// <summary>If set, PlayHighlightReact() is called when hover starts. If unset, found in scene at Start.</summary>
    public PierceAnimationDriver pierceAnimationDriver;

    [Header("Object Highlight")]
    /// <summary>If set, this sprite is shown on hover and restored on hover end.</summary>
    public SpriteRenderer spriteRenderer;
    /// <summary>Sprite to show while hovering. If unset, no sprite swap.</summary>
    public Sprite highlightSprite;

    [Header("Object Animation")]
    /// <summary>If set, this trigger is fired when hover starts.</summary>
    public Animator objectAnimator;
    /// <summary>Trigger parameter name to set when hover starts.</summary>
    public string objectHoverTrigger = "Highlight";

    [Header("Sound")]
    /// <summary>Played when hover starts. If unset, no sound.</summary>
    public AudioClip hoverSound;
    /// <summary>If hoverSound is set and this is unset, a temporary AudioSource is used (one-shot).</summary>
    public AudioSource audioSource;

    private Sprite _originalSprite;

    private void Start()
    {
        if (pierceAnimationDriver == null)
        {
            pierceAnimationDriver = FindFirstObjectByType<PierceAnimationDriver>();
        }
    }

    /// <summary>
    /// Called by ClickController2D when the cursor enters or leaves this interactable. Triggers Pierce animation, object highlight, and sound.
    /// </summary>
    /// <param name="isHovering">True when cursor enters, false when it leaves.</param>
    public void NotifyHover(bool isHovering)
    {
        if (isHovering)
        {
            if (pierceAnimationDriver != null)
                pierceAnimationDriver.PlayHighlightReact();

            if (spriteRenderer != null && highlightSprite != null)
            {
                _originalSprite = spriteRenderer.sprite;
                spriteRenderer.sprite = highlightSprite;
            }

            if (objectAnimator != null && !string.IsNullOrEmpty(objectHoverTrigger))
                objectAnimator.SetTrigger(objectHoverTrigger);

            if (hoverSound != null)
            {
                if (audioSource != null)
                    audioSource.PlayOneShot(hoverSound);
                else
                    AudioSource.PlayClipAtPoint(hoverSound, Camera.main != null ? Camera.main.transform.position : Vector3.zero);
            }
        }
        else
        {
            if (spriteRenderer != null && _originalSprite != null)
                spriteRenderer.sprite = _originalSprite;
        }
    }
}
