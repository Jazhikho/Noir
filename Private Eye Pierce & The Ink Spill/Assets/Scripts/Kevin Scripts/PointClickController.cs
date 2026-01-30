using UnityEngine;

public class PointClickController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float stoppingDistance = 0.1f;

    [Header("Floor Settings")]
    public float floorY = -3f;

    [Header("Click Detection")]
    public LayerMask walkableLayer;
    public LayerMask interactableLayer;

    private float targetX;
    private bool isMoving = false;
    private bool movementEnabled = true;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Interactable currentTarget;
    private bool walkingToInteractable = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        targetX = transform.position.x;

        Vector3 pos = transform.position;
        pos.y = floorY;
        transform.position = pos;
    }

    void Update()
    {
        HandleClickInput();

        if (movementEnabled)
        {
            HandleMovement();
        }
    }

    void HandleClickInput()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (!movementEnabled) return;

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Check for interactable objects first
        RaycastHit2D interactableHit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, interactableLayer);

        if (interactableHit.collider != null)
        {
            Interactable interactable = interactableHit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                currentTarget = interactable;
                targetX = interactable.GetInteractionX();
                walkingToInteractable = true;
                isMoving = true;
                return;
            }
        }

        // Check for walkable area
        RaycastHit2D walkableHit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, walkableLayer);

        if (walkableHit.collider != null)
        {
            currentTarget = null;
            walkingToInteractable = false;
            targetX = mouseWorldPos.x;
            isMoving = true;
        }
    }

    void HandleMovement()
    {
        if (!isMoving) return;

        float distanceToTarget = Mathf.Abs(transform.position.x - targetX);

        if (distanceToTarget > stoppingDistance)
        {
            float direction = Mathf.Sign(targetX - transform.position.x);

            Vector3 newPosition = transform.position;
            newPosition.x += direction * moveSpeed * Time.deltaTime;
            newPosition.y = floorY;
            transform.position = newPosition;

            if (direction != 0)
            {
                spriteRenderer.flipX = direction > 0;
            }

            if (animator != null)
            {
                animator.SetBool("IsWalking", true);
            }
        }
        else
        {
            isMoving = false;

            if (animator != null)
            {
                animator.SetBool("IsWalking", false);
            }

            if (walkingToInteractable && currentTarget != null)
            {
                FaceInteractable();
                DisableMovement();
                currentTarget.OnPlayerArrived();
                walkingToInteractable = false;
            }
        }
    }

    void FaceInteractable()
    {
        if (currentTarget == null) return;

        float direction = Mathf.Sign(currentTarget.transform.position.x - transform.position.x);
        if (direction != 0)
        {
            spriteRenderer.flipX = direction > 0;
        }
    }

    public void DisableMovement()
    {
        movementEnabled = false;
        isMoving = false;
        walkingToInteractable = false;

        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
        }
    }

    public void EnableMovement()
    {
        movementEnabled = true;
        currentTarget = null;
    }

    public void TeleportToX(float xPosition)
    {
        Vector3 newPos = transform.position;
        newPos.x = xPosition;
        newPos.y = floorY;
        transform.position = newPos;
        targetX = xPosition;
        isMoving = false;
    }

    public void SetFloorY(float newFloorY)
    {
        floorY = newFloorY;
        Vector3 pos = transform.position;
        pos.y = floorY;
        transform.position = pos;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public bool IsMovementEnabled()
    {
        return movementEnabled;
    }
}