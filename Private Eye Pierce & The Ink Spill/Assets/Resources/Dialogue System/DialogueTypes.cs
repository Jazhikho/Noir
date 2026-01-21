using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public enum Speaker
{
    Player,
    Other
}

[System.Serializable]
public class DialogueChoice
{
    [Tooltip("Text shown on the choice button")]
    public string choiceText;

    [Tooltip("Index of the dialogue line to jump to when chosen (-1 to end)")]
    public int nextLineIndex = -1;

    [Tooltip("Optional UnityEvent invoked when this choice is selected")]
    public UnityEvent onChoose;
}

[System.Serializable]
public class DialogueLine
{
    [Tooltip("Who is speaking for this line")]
    public Speaker speaker = Speaker.Other;

    [TextArea(2, 6)]
    [Tooltip("The actual spoken text")]
    public string text;

    [Tooltip("Optional portrait for the speaking side (overrides default portraits set on the DialogueAsset)")]
    public Sprite portrait;

    [Tooltip("Speed (seconds per character) for typewriter; 0 to show instantly")]
    public float typingSpeed = 0.02f;

    [Tooltip("If set, will jump to this index after this line. Use -1 to end. Default -1 means go to next index.")]
    public int nextLineIndex = -1;

    [Tooltip("UnityEvent called when this line starts (useful for triggering animations/audio/etc)")]
    public UnityEvent onLineStart;

    [Tooltip("UnityEvent called when this line ends")]
    public UnityEvent onLineEnd;

    [Tooltip("If non-empty, player is presented with these choices instead of auto-continuing")]
    public List<DialogueChoice> choices;
}
