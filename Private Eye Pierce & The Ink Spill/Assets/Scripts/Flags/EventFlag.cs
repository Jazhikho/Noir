using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewEventFlag", menuName = "Game/Event Flag", order = 1)]
public class EventFlag : ScriptableObject
{
    [SerializeField] private bool _isActive = false;
    [SerializeField] private bool resetOnPlay = true;
    [SerializeField] private bool defaultValue = false;

    public bool isActive
    {
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                IsActiveChanged?.Invoke(value);
            }
        }
    }

    public event Action<bool> IsActiveChanged;

    public void Toggle()
    {
        isActive = true;
    }

    public void SetActive(bool value)
    {
        isActive = value;
    }

    public void ResetEvent()
    {
        isActive = false;
    }

    public void Reset()
    {
        isActive = defaultValue;
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
    {
        if (state == UnityEditor.PlayModeStateChange.ExitingEditMode && resetOnPlay)
        {
            _isActive = defaultValue;
        }
    }
#endif
}
