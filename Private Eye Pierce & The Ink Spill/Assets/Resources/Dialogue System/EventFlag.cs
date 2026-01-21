using UnityEngine;
using System;

/// <summary>
/// Event flag for tracking game state and triggering events.
/// Used by the dialogue system and other game systems to track boolean state.
/// </summary>
[CreateAssetMenu(menuName = "Dialogue System/Event Flag")]
public class EventFlag : ScriptableObject
{
    [SerializeField] private bool _isActive = false;

    /// <summary>
    /// Event fired when the active state changes
    /// </summary>
    public event Action<bool> IsActiveChanged;

    /// <summary>
    /// Whether the flag is currently active
    /// </summary>
    public bool isActive
    {
        get { return _isActive; }
        private set
        {
            if (_isActive != value)
            {
                _isActive = value;
                IsActiveChanged?.Invoke(_isActive);
            }
        }
    }

    /// <summary>
    /// Toggle the flag's active state
    /// </summary>
    public void Toggle()
    {
        isActive = !isActive;
    }

    /// <summary>
    /// Set the flag to active (true)
    /// </summary>
    public void SetActive()
    {
        isActive = true;
    }

    /// <summary>
    /// Set the flag to inactive (false)
    /// </summary>
    public void SetInactive()
    {
        isActive = false;
    }

    /// <summary>
    /// Reset the flag to its default state (inactive)
    /// </summary>
    public void ResetEvent()
    {
        isActive = false;
    }
}
