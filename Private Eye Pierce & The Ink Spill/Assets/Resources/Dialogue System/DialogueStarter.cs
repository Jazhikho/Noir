using UnityEngine;
using UnityEngine.InputSystem; // Required for new Input System

[RequireComponent(typeof(Collider2D))]
public class DialogueStarter : MonoBehaviour
{
    [Tooltip("Optional reference to a DialogueRunner in the scene")]
    public DialogueRunner runner;

    public DialogueAsset dialogueToStart;

    [Tooltip("Start automatically on Awake")]
    public bool startOnAwake = false;

    private bool playerInRange = false;
    private PlayerInput playerInput;
    private InputAction interactAction;

    void Awake()
    {
        if (runner == null)
            runner = FindFirstObjectByType<DialogueRunner>();

    }

    void Start()
    {

        if (startOnAwake && dialogueToStart != null)
        {
            runner.StartDialogue(dialogueToStart);
        }

        // Find the PlayerInput component in the scene (on the player)
        playerInput = FindFirstObjectByType<PlayerInput>();

        if (playerInput != null)
        {
            // Make sure this matches your Input Actions map name and action name!
            interactAction = playerInput.actions["Interact"];
            interactAction.performed += OnInteract;
        }
        else
        {
            Debug.LogWarning("No PlayerInput found in scene � DialogueStarter won't detect interactions.");
        }
    }

    private void OnDestroy()
    {
        if (interactAction != null)
            interactAction.performed -= OnInteract;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (playerInRange && dialogueToStart != null && runner != null)
        {
            runner.StartDialogue(dialogueToStart);
        }
    }
}
