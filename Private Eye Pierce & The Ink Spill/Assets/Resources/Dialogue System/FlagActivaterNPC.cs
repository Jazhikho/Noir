using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagActivaterNPC : MonoBehaviour
{
    [Header("Flags to Turn ON")]
    [SerializeField] private EventFlag[] flagsToEnable;

    [Header("Flags to Turn OFF")]
    [SerializeField] private EventFlag[] flagsToDisable;

    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";   // Tag your player "Player"

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            ActivateFlags();
        }
    }

    private void ActivateFlags()
    {
        // Enable selected flags
        foreach (var flag in flagsToEnable)
        {
            if (flag != null)
                flag.Toggle();
        }

        // Disable selected flags
        foreach (var flag in flagsToDisable)
        {
            if (flag != null)
                flag.ResetEvent();
        }

        Debug.Log("Flag toggle trigger activated.");
    }
}