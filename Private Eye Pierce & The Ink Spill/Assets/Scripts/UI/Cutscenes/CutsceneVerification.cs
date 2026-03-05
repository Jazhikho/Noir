using UnityEngine;

/// <summary>
/// Attach to the same GameObject as CutsceneOrchestrator during testing.
/// Logs key events so QA can verify everything fires in the right order.
/// Safe to remove in builds - it only logs when enabled.
/// </summary>
public class CutsceneVerification : MonoBehaviour
{
    [Tooltip("Turn this on to see detailed logs in the console")]
    public bool enableLogs = true;

    private CutsceneOrchestrator orchestrator;
    private CutsceneSkipController skipController;
    private CutsceneOrchestrator.State lastState;

    void Start()
    {
        orchestrator = GetComponent<CutsceneOrchestrator>();
        skipController = GetComponent<CutsceneSkipController>();

        if (orchestrator == null)
        {
            Log("No CutsceneOrchestrator found on this object.");
            return;
        }

        if (skipController != null)
            skipController.RequestSkip += () => Log("SKIP TRIGGERED - player held long enough");

        Log("Verification active. Checklist:");
        Log("  [ ] Fade-in from black completes before video starts");
        Log("  [ ] Pre-delay matches config value");
        Log("  [ ] Video fades in smoothly (no pop)");
        Log("  [ ] Video plays to completion OR player skips");
        Log("  [ ] Skip bar appears after configured delay");
        Log("  [ ] Skip bar fills while holding, drains when released");
        Log("  [ ] Skip bar drain is smooth, not instant");
        Log("  [ ] On skip: video stops, display fades out, scene loads");
        Log("  [ ] On natural end: same fade-out and scene load");
        Log("  [ ] No audio continues after transition");
        Log("  [ ] Next scene loads with fade-in from black");
        Log("  [ ] Player controls are restored in the new scene");
    }

    void Update()
    {
        if (orchestrator == null || !enableLogs) return;

        if (orchestrator.CurrentState != lastState)
        {
            lastState = orchestrator.CurrentState;
            Log($"State: {lastState}");

            if (lastState == CutsceneOrchestrator.State.Complete)
                Log("Cutscene complete. Check all items above.");
        }

        if (skipController != null && skipController.IsActive && skipController.Progress > 0f)
            Log($"Skip progress: {skipController.Progress:F2}");
    }

    void Log(string message)
    {
        if (enableLogs)
            Debug.Log($"[CutsceneVerify] {message}");
    }
}
