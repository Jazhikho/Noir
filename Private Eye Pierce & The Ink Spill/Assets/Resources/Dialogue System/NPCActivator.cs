using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCActivator : MonoBehaviour
{
    [SerializeField] private EventFlag appearFlag;

    private void Start()
    {
        gameObject.SetActive(appearFlag.isActive);

        appearFlag.IsActiveChanged += OnFlagChanged;
    }

    private void OnDestroy()
    {
        appearFlag.IsActiveChanged -= OnFlagChanged;
    }

    private void OnFlagChanged(bool value)
    {
        gameObject.SetActive(value);
    }
}