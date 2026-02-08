using UnityEngine;
using UnityEngine.Events;

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

    [Header("HUD Integration")]
    public bool showExitArrow = true;

    public void UseDoor()
    {
        if (IsCurrentlyLocked())
        {
            Debug.Log("LockedDoor: Door to " + targetRoom.roomId + " is locked.");
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
        if (targetRoom != null)
        {
            var player = FindFirstObjectByType<PointClickController>();

            Transform spawnPoint = targetRoom.GetEntryPoint(targetEntryPointName);
            if (spawnPoint != null && player != null)
            {
                player.TeleportToX(spawnPoint.position.x);
                player.SetFloorY(spawnPoint.position.y);
            }

            RoomManager.Instance.EnterRoom(targetRoom);
            OnEntered?.Invoke();
        }
        else
        {
            Debug.LogWarning("LockedDoor: TargetRoom is not assigned.");
        }

        Interactable.EndCurrentInteraction();
    }

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

    public void Lock()
    {
        isLocked = true;
    }

    public void Unlock()
    {
        isLocked = false;
    }

    public void ToggleLock()
    {
        isLocked = !isLocked;
    }
}
