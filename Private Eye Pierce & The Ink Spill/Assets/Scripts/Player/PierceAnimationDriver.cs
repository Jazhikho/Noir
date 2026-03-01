using System.Collections;
using UnityEngine;

public class PierceAnimationDriver : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public PointClickController playerController;

    [Header("Look Up Settings")]
    public bool lookUpEnabled = true;
    public float lookUpThreshold = 1.5f;

    [Header("Animator Parameters")]
    public string inspectTrigger = "Inspect";
    public string releaseInspectBool = "ReleaseInspect";
    public string lookingUpBool = "IsLookingUp";
    /// <summary>Trigger fired when the cursor hovers an interactable (optional highlight reaction).</summary>
    public string highlightReactTrigger = "HighlightReact";

    [Header("Inspect Auto End")]
    public float inspectAutoEndDelay = 0.5f;

    private bool isLookingUp = false;
    private bool autoEndAfterInspect = false;
    private Coroutine autoEndCoroutine;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogWarning("PierceAnimationDriver: No Animator found. Animation features disabled.");

        if (playerController == null)
            playerController = GetComponent<PointClickController>();

        if (playerController == null)
            playerController = FindFirstObjectByType<PointClickController>();
    }

    void Update()
    {
        if (!lookUpEnabled) return;
        if (animator == null) return;

        UpdateLookUp();
    }

    void UpdateLookUp()
    {
        if (playerController != null && playerController.IsMoving())
        {
            SetLookingUp(false);
            return;
        }

        if (Camera.main == null)
            return;

        Vector2 pointerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float playerY = transform.position.y;
        bool shouldLookUp = pointerPos.y > playerY + lookUpThreshold;

        SetLookingUp(shouldLookUp);
    }

    public void PlayInspect()
    {
        if (animator == null)
        {
            Debug.LogWarning("PierceAnimationDriver: Cannot play inspect, no Animator.");
            return;
        }

        animator.SetTrigger(inspectTrigger);
    }

    /// <summary>
    /// Plays the inspect animation and holds on the last frame until ReleaseInspect() is called.
    /// Use for search: call at search start, then ReleaseInspect() when search completes.
    /// Requires an Animator bool "ReleaseInspect": Inspect state should transition to Idle when ReleaseInspect is true.
    /// </summary>
    public void PlayInspectAndHold()
    {
        if (animator == null)
        {
            Debug.LogWarning("PierceAnimationDriver: Cannot play inspect, no Animator.");
            return;
        }

        if (!string.IsNullOrEmpty(releaseInspectBool))
            animator.SetBool(releaseInspectBool, false);
        animator.SetTrigger(inspectTrigger);
    }

    /// <summary>
    /// Releases the inspect hold so the animator can transition from Inspect back to Idle. Call when search completes.
    /// </summary>
    public void ReleaseInspect()
    {
        if (animator == null)
            return;
        if (!string.IsNullOrEmpty(releaseInspectBool))
            animator.SetBool(releaseInspectBool, true);
    }

    public void PlayInspectAutoEnd()
    {
        autoEndAfterInspect = true;

        if (autoEndCoroutine != null)
            StopCoroutine(autoEndCoroutine);

        autoEndCoroutine = StartCoroutine(AutoEndFallback());
        PlayInspect();
    }

    IEnumerator AutoEndFallback()
    {
        yield return new WaitForSeconds(inspectAutoEndDelay);
        OnInspectAnimationComplete();
    }

    public void OnInspectAnimationComplete()
    {
        if (autoEndCoroutine != null)
        {
            StopCoroutine(autoEndCoroutine);
            autoEndCoroutine = null;
        }

        if (!autoEndAfterInspect)
            return;

        autoEndAfterInspect = false;
        Interactable.EndCurrentInteraction();
    }

    public void SetLookingUp(bool value)
    {
        if (isLookingUp == value) return;

        isLookingUp = value;

        if (animator != null)
            animator.SetBool(lookingUpBool, value);
    }

    public bool IsLookingUp() => isLookingUp;

    public void SetLookUpEnabled(bool enabled)
    {
        lookUpEnabled = enabled;

        if (!enabled)
            SetLookingUp(false);
    }

    /// <summary>
    /// Plays the highlight-react animation (e.g. when the cursor hovers an interactable). Add a "HighlightReact" trigger in the Animator if you use this.
    /// </summary>
    public void PlayHighlightReact()
    {
        if (animator == null) return;
        if (string.IsNullOrEmpty(highlightReactTrigger)) return;
        animator.SetTrigger(highlightReactTrigger);
    }
}
