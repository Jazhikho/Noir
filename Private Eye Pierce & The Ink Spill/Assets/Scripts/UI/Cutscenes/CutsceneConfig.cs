using UnityEngine;

/// <summary>
/// Drop one of these into the CutscenePlayer to define how a cutscene behaves.
/// Right-click in Project > Create > Cutscenes > Cutscene Config.
/// </summary>
[CreateAssetMenu(fileName = "NewCutsceneConfig", menuName = "Cutscenes/Cutscene Config")]
public class CutsceneConfig : ScriptableObject
{
    [Header("Where does the player go after the cutscene?")]
    public string nextSceneName = "OfficeFloor";

    [Header("Timing")]
    [Tooltip("Brief pause before the video starts playing")]
    public float delayBeforeVideo = 0.5f;

    [Tooltip("How long the video takes to appear")]
    public float videoFadeInDuration = 0.3f;

    [Tooltip("How long the video takes to disappear")]
    public float videoFadeOutDuration = 0.3f;

    [Tooltip("Pause after video ends, before the scene switches")]
    public float delayAfterVideo = 0.3f;

    [Header("Skip Settings")]
    [Tooltip("Can the player skip this cutscene at all?")]
    public bool skipEnabled = true;

    [Tooltip("Seconds to wait before the skip bar appears")]
    public float skipAppearDelay = 1f;

    [Tooltip("How long the player has to hold the button to skip")]
    [Range(0.5f, 5f)]
    public float skipHoldSeconds = 2f;

    [Tooltip("How fast the skip bar drains when they let go")]
    [Range(0.05f, 2f)]
    public float skipDecaySeconds = 0.35f;
}
