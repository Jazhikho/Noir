using UnityEngine;
using System.Collections;

// This is mainly for the invisible guy but really you can use it for game object you want to have fade in (or pop in) after a flag is triggered. 
public class ActivateOnFlag : MonoBehaviour
{
    [Tooltip("The EventFlag to listen to.")]
    public EventFlag flag;

    [Tooltip("GameObjects to activate when the flag becomes true. If empty, affects this GameObject.")]
    public GameObject[] targets;

    [Tooltip("If true, targets are shown when the flag is true and hidden when false. If false, the reverse.")]
    public bool activeWhenFlagTrue = true;

    [Header("Fade")]
    [Tooltip("Fade duration in seconds. 0 = instant.")]
    public float fadeDuration = 0f;

    private SpriteRenderer _sr;
    private Collider2D _col;
    private bool _isSelfTarget;
    private Coroutine _fadeCoroutine;

    void Awake()
    {
        _isSelfTarget = targets == null || targets.Length == 0;
        if (!_isSelfTarget)
        {
            foreach (GameObject t in targets)
            {
                if (t == gameObject) { _isSelfTarget = true; break; }
            }
        }

        if (_isSelfTarget)
        {
            _sr = GetComponent<SpriteRenderer>();
            _col = GetComponent<Collider2D>();
        }
    }

    void OnEnable()
    {
        if (flag == null) return;
        flag.IsActiveChanged += OnFlagChanged;
        ApplyState(flag.isActive, instant: true);
    }

    void OnDisable()
    {
        if (flag == null) return;
        flag.IsActiveChanged -= OnFlagChanged;
    }

    private void OnFlagChanged(bool flagValue)
    {
        ApplyState(flagValue, instant: false);
    }

    private void ApplyState(bool flagValue, bool instant)
    {
        bool desired = activeWhenFlagTrue ? flagValue : !flagValue;

        if (_isSelfTarget && _sr != null)
        {
            // Keep the GameObject active so the component stays subscribed.
            // Control visibility via SpriteRenderer alpha and collider toggle.
            if (_col != null) _col.enabled = desired;

            float targetAlpha = desired ? 1f : 0f;
            if (instant || fadeDuration <= 0f)
            {
                SetAlpha(targetAlpha);
            }
            else
            {
                if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = StartCoroutine(Fade(_sr.color.a, targetAlpha));
            }
        }
        else
        {
            if (targets != null && targets.Length > 0)
            {
                foreach (GameObject go in targets)
                {
                    if (go != null) go.SetActive(desired);
                }
            }
            else
            {
                gameObject.SetActive(desired);
            }
        }
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, elapsed / fadeDuration));
            yield return null;
        }
        SetAlpha(to);
        _fadeCoroutine = null;
    }

    private void SetAlpha(float alpha)
    {
        Color c = _sr.color;
        c.a = alpha;
        _sr.color = c;
    }
}
