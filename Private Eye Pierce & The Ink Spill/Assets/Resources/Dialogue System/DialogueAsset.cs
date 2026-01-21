using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue Asset", fileName = "New Dialogue")]
public class DialogueAsset : ScriptableObject
{
    [Header("Basic info")]
    public string dialogueName;

    [Tooltip("Default portrait for the player (left). Individual lines can override with their own portrait.")]
    public Sprite defaultPlayerPortrait;

    [Tooltip("Default portrait for the other speaker (right).")]
    public Sprite defaultOtherPortrait;

    [Tooltip("Player display name")]
    public string playerName = "Player";

    [Tooltip("Other speaker display name")]
    public string otherName = "NPC";

    [Header("Lines")]
    public List<DialogueLine> lines = new List<DialogueLine>();

    [Tooltip("Index to start at when this dialogue is played")]
    public int startingLine = 0;
}
