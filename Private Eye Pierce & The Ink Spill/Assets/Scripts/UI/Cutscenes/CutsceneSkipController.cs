using System;
using UnityEngine;

/// <summary>
/// Tracks hold-to-skip input. The bar fills while the player holds the mouse
/// button and drains smoothly when they let go. Fires RequestSkip when full.
/// Does NOT know anything about cutscene logic - the orchestrator decides
/// what "skip" actually means.
/// </summary>
public class CutsceneSkipController : MonoBehaviour
{
    /// <summary>Fired exactly once when the bar fills to 100%.</summary>
    public event Action RequestSkip;

    /// <summary>Current fill amount, 0 to 1.</summary>
    public float Progress { get; private set; }

    /// <summary>True when the skip bar is accepting input.</summary>
    public bool IsActive { get; private set; }

    private float holdSeconds = 2f;
    private float decaySeconds = 0.35f;
    private bool completed;

    /// <summary>Set the timing from a CutsceneConfig.</summary>
    public void Configure(float holdSec, float decaySec)
    {
        holdSeconds = Mathf.Max(0.1f, holdSec);
        decaySeconds = Mathf.Max(0.01f, decaySec);
    }

    /// <summary>Turn the skip system on or off. Resets progress when disabled.</summary>
    public void SetEnabled(bool active)
    {
        IsActive = active;
        if (!active)
        {
            Progress = 0f;
            completed = false;
        }
    }

    void Update()
    {
        if (!IsActive || completed) return;

        // Uses unscaled time so skip still works during slow-mo or paused time
        if (Input.GetMouseButton(0))
            Progress += Time.unscaledDeltaTime / holdSeconds;
        else
            Progress -= Time.unscaledDeltaTime / decaySeconds;

        Progress = Mathf.Clamp01(Progress);

        if (Progress >= 1f)
        {
            completed = true;
            RequestSkip?.Invoke();
        }
    }
}
