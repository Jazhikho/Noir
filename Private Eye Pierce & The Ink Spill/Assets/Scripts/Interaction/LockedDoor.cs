using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Door that can be locked by an EventFlag. When unlocked, teleports the player to targetRoom and notifies RoomManager.
/// Used for e.g. tutorial door that opens after puzzle completion.
/// </summary>
public class LockedDoor : MonoBehaviour
{
    [Header("Destination")]
    public RoomDefinition targetRoom;
    public string targetEntryPointName = "Left";

    [Header("Lock Settings")]
    public bool isLocked = false;
    public EventFlag unlockFlag;
    public bool requireFlagTrue = true;

    [Header("Events")]
    public UnityEvent OnLockedAttempt;
    public UnityEvent OnEntered;

    [Header("Trigger Safety")]
    [Tooltip("Ignore door use for this long after teleport (seconds). Prevents double-fire.")]
    [SerializeField] private float _teleportCooldown = 0.25f;
    private static readonly Dictionary<int, float> _nextAllowedUse = new Dictionary<int, float>();

    [Header("UI")]
    [Tooltip("Optional. GameObject to show when player is in trigger range (e.g. glow).")]
    public GameObject glowEffect;

    [Header("HUD Integration")]
    public bool showExitArrow = true;

    /// <summary>
    /// Called when the player uses this door (e.g. from TutorialPuzzle). Teleports to target room if unlocked; applies cooldown.
    /// </summary>
    public void UseDoor()
    {
        if (IsCurrentlyLocked())
        {
            if (targetRoom != null)
            {
                Debug.Log("LockedDoor: Door to " + targetRoom.roomId + " is locked.");
            }
            OnLockedAttempt?.Invoke();
            Interactable.EndCurrentInteraction();
            return;
        }

        if (RoomManager.Instance == null)
        {
            Debug.LogError("[LockedDoor] RoomManager.Instance is null. Add a RoomManager to the scene.", this);
            Interactable.EndCurrentInteraction();
            return;
        }
        if (targetRoom == null)
        {
            Debug.LogWarning("LockedDoor: TargetRoom is not assigned.");
            Interactable.EndCurrentInteraction();
            return;
        }

        PointClickController playerController = FindFirstObjectByType<PointClickController>();
        if (playerController == null)
        {
            Interactable.EndCurrentInteraction();
            return;
        }

        int key = playerController.GetInstanceID();
        float now = Time.time;
        if (_nextAllowedUse.TryGetValue(key, out float gate) && now < gate)
        {
            Interactable.EndCurrentInteraction();
            return;
        }
        _nextAllowedUse[key] = now + _teleportCooldown;

        Transform spawnPoint = targetRoom.GetEntryPoint(targetEntryPointName);
        if (spawnPoint != null)
        {
            playerController.TeleportToX(spawnPoint.position.x);
        }

        RoomManager.Instance.EnterRoom(targetRoom);
        OnEntered?.Invoke();
        Interactable.EndCurrentInteraction();
    }

    /// <summary>
    /// Returns true if this door should refuse use (locked by flag or isLocked).
    /// </summary>
    public bool IsCurrentlyLocked()
    {
        if (unlockFlag != null)
        {
            if (requireFlagTrue)
                return !unlockFlag.isActive;
            else if (unlockFlag.isActive)
                return false;
        }

        return isLocked;
    }

    /// <summary>Sets the door to locked state.</summary>
    public void Lock()
    {
        isLocked = true;
    }

    /// <summary>Sets the door to unlocked state.</summary>
    public void Unlock()
    {
        isLocked = false;
    }

    /// <summary>Flips the current locked state.</summary>
    public void ToggleLock()
    {
        isLocked = !isLocked;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && glowEffect != null)
        {
            glowEffect.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && glowEffect != null)
        {
            glowEffect.SetActive(false);
        }
    }
}
