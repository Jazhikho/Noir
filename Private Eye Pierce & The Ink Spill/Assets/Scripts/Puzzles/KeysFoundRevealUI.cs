using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Shows Pierce's arm with keys in hand (fullscreen or overlay image) when the keys are found.
/// Wire KeyHuntManager.OnKeysFound to ShowReveal(). This component must run on an always-active GameObject:
/// if it is on the same GameObject as the arm image, that object is kept active and only the Image is hidden so the event can fire.
/// </summary>
public class KeysFoundRevealUI : MonoBehaviour
{
    [Header("Display")]
    [Tooltip("UI Image showing arm with keys. Create under HUD Canvas (e.g. fullscreen or centered). Start disabled or image hidden.")]
    public Image armWithKeysImage;

    [Header("Timing")]
    public float displayDuration = 2f;
    public float fadeDuration = 0.5f;

    [Header("Optional")]
    [Tooltip("If set, Pierce is hidden while the reveal is showing (like vending machine).")]
    public GameObject playerRoot;

    private bool _isShowing;
    private bool _imageOnSameObject;

    private void Awake()
    {
        if (armWithKeysImage != null)
        {
            _imageOnSameObject = armWithKeysImage.gameObject == gameObject;
            if (_imageOnSameObject)
                armWithKeysImage.enabled = false;
            else
                armWithKeysImage.gameObject.SetActive(false);
        }

        if (playerRoot == null)
        {
            PointClickController p = FindFirstObjectByType<PointClickController>();
            if (p != null)
                playerRoot = p.gameObject;
        }
    }

    /// <summary>
    /// Call from KeyHuntManager.OnKeysFound. Shows the arm-with-keys image, holds, then fades out.
    /// If this component is on an inactive GameObject (e.g. KeysFound), the coroutine runs on an active host so it can execute.
    /// </summary>
    public void ShowReveal()
    {
        if (_isShowing)
            return;

        if (!gameObject.activeInHierarchy)
        {
            MonoBehaviour host = FindFirstObjectByType<KeyHuntManager>();
            if (host != null)
                host.StartCoroutine(RevealCoroutine());
            else
                Debug.LogError("KeysFoundRevealUI: This GameObject is inactive and no active KeyHuntManager found to run the reveal. Move KeysFoundRevealUI to an always-active GameObject (e.g. under Systems) or enable KeysFound.", this);
            return;
        }

        StartCoroutine(RevealCoroutine());
    }

    private IEnumerator RevealCoroutine()
    {
        _isShowing = true;

        if (playerRoot != null)
            playerRoot.SetActive(false);

        if (armWithKeysImage == null)
        {
            Debug.LogWarning("KeysFoundRevealUI: Arm With Keys Image is not assigned.", this);
            _isShowing = false;
            if (playerRoot != null)
                playerRoot.SetActive(true);
            yield break;
        }

        bool imageOnSameObject = armWithKeysImage.gameObject == gameObject;
        if (imageOnSameObject)
        {
            gameObject.SetActive(true);
            armWithKeysImage.enabled = true;
        }
        else
        {
            armWithKeysImage.gameObject.SetActive(true);
        }
        SetImageAlpha(armWithKeysImage, 1f);
        yield return null;

        yield return new WaitForSeconds(displayDuration);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            SetImageAlpha(armWithKeysImage, alpha);
            yield return null;
        }

        if (imageOnSameObject)
        {
            armWithKeysImage.enabled = false;
            gameObject.SetActive(false);
        }
        else
        {
            armWithKeysImage.gameObject.SetActive(false);
        }

        if (playerRoot != null)
            playerRoot.SetActive(true);

        _isShowing = false;
    }

    private static void SetImageAlpha(Image img, float alpha)
    {
        if (img == null)
            return;
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    /// <summary>True while the reveal is on screen.</summary>
    public bool IsShowing()
    {
        return _isShowing;
    }
}
