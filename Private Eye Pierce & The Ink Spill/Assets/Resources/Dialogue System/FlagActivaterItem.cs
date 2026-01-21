using UnityEngine;
using UnityEngine.InputSystem;

public class ForageableItemFlag : MonoBehaviour
{
    [Header("Flags to Turn ON")]
    [SerializeField] private EventFlag[] flagsToEnable;

    [Header("Flags to Turn OFF")]
    [SerializeField] private EventFlag[] flagsToDisable;

    public InventoryItemSO item;
    public int quantity = 1;

    [Header("Visual Feedback")]
    public GameObject glowEffect;  // <- assign in Inspector

    private bool playerInRange = false;
    private Inventory playerInventory;

    private void Start()
    {
        if (glowEffect != null)
        {
            glowEffect.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInventory = other.GetComponent<Inventory>();
            playerInRange = true;

            if (glowEffect != null)
                glowEffect.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInventory = null;
            playerInRange = false;

            if (glowEffect != null)
                glowEffect.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Keyboard.current != null)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame) // Replace later with proper input action
            {
                Forage();
            }
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
    private void Forage()
    {
        if (playerInventory != null && item != null)
        {
            // Add item quantity times (current inventory system doesn't support quantity natively)
            for (int i = 0; i < quantity; i++)
            {
                playerInventory.AddToInventory(item);
            }
            ActivateFlags();
            Destroy(gameObject);  // destroys both base + glow
        }
    }
}
