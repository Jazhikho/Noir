using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Captures the current screen, then peels it away to reveal the newly transitioned room.
/// Assign a RawImage (with RectTransform) for the overlay; optionally assign a Canvas and capture Camera.
/// </summary>
public class PageTurnTransition : MonoBehaviour
{
    [Tooltip("RawImage that displays the captured screen. Must be fullscreen under a Canvas.")]
    public RawImage captureDisplay;
    [Tooltip("Canvas to reparent the overlay to (e.g. Screen Space Overlay). If unset, uses the display's parent canvas.")]
    public Canvas targetCanvas;
    [Tooltip("Camera to capture from. If unset, uses Camera.main.")]
    public Camera captureCamera;
    [Tooltip("Material with PagePeel shader. If unset, uses a default from Resources or creates one at runtime.")]
    public Material pagePeelMaterial;
    [Tooltip("Duration of the peel animation in seconds.")]
    public float duration = 0.6f;
    [Tooltip("Softness of the peel edge (0.01–0.3).")]
    [Range(0.01f, 0.3f)]
    public float peelSoftness = 0.05f;
    [Tooltip("Optional. Plays once when the transition starts. Uses AudioSource on this GameObject; adds one if missing.")]
    public AudioClip transitionSound;
    [Tooltip("Volume scale for Transition Sound (0 = silent, 1 = full). Note: multiplied by the AudioSource's Volume.")]
    [Range(0f, 1f)]
    public float transitionSoundVolume = 1f;

    private static readonly int CurlAmountId = Shader.PropertyToID("_CurlAmount");
    private static readonly int CurlFromRightId = Shader.PropertyToID("_CurlFromRight");
    private static readonly int PeelSoftnessId = Shader.PropertyToID("_PeelSoftness");

    private bool _isPlaying;
    private Material _materialInstance;
    private RenderTexture _captureRt;

    /// <summary>
    /// Plays the page peel, calls onMidTransition when ready (switch room here), then peels away to reveal the new scene.
    /// </summary>
    /// <param name="rightToLeft">True = peel from right (page curls left). False = peel from left.</param>
    /// <param name="onMidTransition">Called when the capture is displayed; perform the room switch here.</param>
    public void PlayTransition(bool rightToLeft, Action onMidTransition)
    {
        if (captureDisplay == null)
        {
            onMidTransition?.Invoke();
            return;
        }
        if (_isPlaying)
            return;
        StartCoroutine(PlayTransitionCoroutine(rightToLeft, onMidTransition));
    }

    private IEnumerator PlayTransitionCoroutine(bool rightToLeft, Action onMidTransition)
    {
        _isPlaying = true;

        if (transitionSound != null)
        {
            AudioSource src = GetComponent<AudioSource>();
            if (src == null)
                src = gameObject.AddComponent<AudioSource>();
            // PlayOneShot's volumeScale is a per-call multiplier (also multiplied by AudioSource.volume).
            src.PlayOneShot(transitionSound, transitionSoundVolume);
        }

        Camera cam = captureCamera != null ? captureCamera : Camera.main;
        if (cam == null)
        {
            Debug.LogError("[PageTurnTransition] No camera. Assign Capture Camera or ensure Camera.main exists.", this);
            onMidTransition?.Invoke();
            _isPlaying = false;
            yield break;
        }

        Rect viewportRect = cam.rect;
        int w = Mathf.RoundToInt(viewportRect.width * Screen.width);
        int h = Mathf.RoundToInt(viewportRect.height * Screen.height);
        if (w < 1) w = 1;
        if (h < 1) h = 1;

        if (_captureRt == null || _captureRt.width != w || _captureRt.height != h)
        {
            if (_captureRt != null)
                _captureRt.Release();
            _captureRt = new RenderTexture(w, h, 24);
            _captureRt.Create();
        }

        Rect prevRect = cam.rect;
        cam.rect = new Rect(0f, 0f, 1f, 1f);
        RenderTexture prevTarget = cam.targetTexture;
        cam.targetTexture = _captureRt;
        cam.Render();
        cam.targetTexture = prevTarget;
        cam.rect = prevRect;

        captureDisplay.texture = _captureRt;
        captureDisplay.color = Color.white;
        captureDisplay.gameObject.SetActive(true);

        RectTransform rect = captureDisplay.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Transform originalParent = rect.parent;
        Canvas canvas = targetCanvas != null ? targetCanvas : rect.GetComponentInParent<Canvas>();
        if (canvas != null && canvas.transform.localScale == Vector3.zero)
        {
            foreach (Canvas c in FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (c.renderMode == RenderMode.ScreenSpaceOverlay && c.transform.localScale != Vector3.zero)
                {
                    canvas = c;
                    break;
                }
            }
        }
        if (canvas != null && canvas.transform != originalParent)
        {
            rect.SetParent(canvas.transform, false);
            rect.SetAsLastSibling();
        }

        Material mat = GetOrCreateMaterial();
        mat.SetFloat(CurlFromRightId, rightToLeft ? 1f : 0f);
        mat.SetFloat(PeelSoftnessId, peelSoftness);
        mat.SetFloat(CurlAmountId, 0f);
        captureDisplay.material = mat;

        yield return null;
        onMidTransition?.Invoke();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t);
            mat.SetFloat(CurlAmountId, t);
            yield return null;
        }

        mat.SetFloat(CurlAmountId, 1f);
        captureDisplay.gameObject.SetActive(false);
        captureDisplay.material = null;
        captureDisplay.texture = null;

        if (originalParent != null)
            rect.SetParent(originalParent, false);

        _isPlaying = false;
    }

    private Material GetOrCreateMaterial()
    {
        if (pagePeelMaterial != null)
        {
            if (_materialInstance == null)
                _materialInstance = new Material(pagePeelMaterial);
            return _materialInstance;
        }

        Shader peelShader = Shader.Find("UI/PagePeel");
        if (peelShader == null)
        {
            Debug.LogError("[PageTurnTransition] PagePeel shader not found. Assign Page Peel Material in the Inspector.", this);
            return null;
        }

        if (_materialInstance == null)
            _materialInstance = new Material(peelShader);
        return _materialInstance;
    }

    private void OnDestroy()
    {
        if (_captureRt != null)
        {
            _captureRt.Release();
            _captureRt = null;
        }
        if (_materialInstance != null)
        {
            Destroy(_materialInstance);
            _materialInstance = null;
        }
    }
}
