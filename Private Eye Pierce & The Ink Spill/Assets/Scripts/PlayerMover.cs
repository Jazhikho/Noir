using UnityEngine;

/// <summary>
/// Handles 2D player movement along the X axis with optional physics integration.
/// Supports sprite flipping, animation states, and floor Y-locking for side-scrolling adventure games.
/// </summary>
public class PlayerMover2D : MonoBehaviour
{
    [Header("Movement Settings")]
    // Movement speed in units per second.
    public float speed = 4f;
    
    // Distance threshold at which the player is considered to have reached the target.
    public float stopDistance = 0.02f;
    
    // If set, movement uses MovePosition so wall colliders block the player. Add Rigidbody2D (Kinematic) + Collider2D to the player.
    [SerializeField] private Rigidbody2D body;

    [Header("Floor Settings")]
    // If true, locks the player's Y position to floorY value.
    [SerializeField] private bool lockFloorY = false;
    
    // The Y position to lock the player to when lockFloorY is enabled.
    [SerializeField] private float floorY = 0f;

    [Header("Visual Feedback")]
    // If set, automatically flips the sprite based on movement direction.
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    // If true, sprite faces right when flipX is false. Set to false if your sprite naturally faces left.
    [SerializeField] private bool spriteFacesRight = true;
    
    // If set, updates the "IsWalking" bool parameter based on movement state.
    [SerializeField] private Animator animator;

    private float? _targetX;
    private bool _movementEnabled = true;

    /// <summary>
    /// Sets the target X position for the player to move towards.
    /// </summary>
    /// <param name="x">The target X coordinate.</param>
    public void SetTargetX(float x)
    {
        if (!_movementEnabled) return;
        _targetX = x;
    }

    /// <summary>
    /// Disables player movement. Player will stop moving and ignore new movement commands.
    /// </summary>
    public void DisableMovement()
    {
        _movementEnabled = false;
        _targetX = null;
        
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
        }
    }

    /// <summary>
    /// Enables player movement. Player can receive and execute movement commands.
    /// </summary>
    public void EnableMovement()
    {
        _movementEnabled = true;
    }

    /// <summary>
    /// Returns whether the player is currently moving towards a target.
    /// </summary>
    /// <returns>True if moving, false if idle.</returns>
    public bool IsMoving()
    {
        return _targetX != null;
    }

    /// <summary>
    /// Returns whether movement is currently enabled.
    /// </summary>
    /// <returns>True if movement is enabled, false if disabled.</returns>
    public bool IsMovementEnabled()
    {
        return _movementEnabled;
    }

    /// <summary>
    /// Immediately teleports the player to the specified X position without animation.
    /// </summary>
    /// <param name="x">The X coordinate to teleport to.</param>
    public void TeleportToX(float x)
    {
        Vector3 newPos = transform.position;
        newPos.x = x;
        
        if (lockFloorY)
        {
            newPos.y = floorY;
        }
        
        transform.position = newPos;
        _targetX = null;
    }

    /// <summary>
    /// Sets the floor Y position and optionally snaps the player to it immediately.
    /// </summary>
    /// <param name="newFloorY">The new floor Y coordinate.</param>
    /// <param name="snapImmediately">If true, immediately moves player to the new floor Y.</param>
    public void SetFloorY(float newFloorY, bool snapImmediately = true)
    {
        floorY = newFloorY;
        
        if (snapImmediately)
        {
            Vector3 pos = transform.position;
            pos.y = floorY;
            transform.position = pos;
        }
    }

    /// <summary>
    /// Handles player movement towards the target position each frame.
    /// </summary>
    private void Update()
    {
        if (!_movementEnabled || _targetX == null) 
        {
            if (animator != null)
            {
                animator.SetBool("IsWalking", false);
            }
            return;
        }

        float dx = _targetX.Value - transform.position.x;
        
        if (Mathf.Abs(dx) <= stopDistance)
        {
            _targetX = null;
            
            if (animator != null)
            {
                animator.SetBool("IsWalking", false);
            }
            return;
        }

        // Update animation state
        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
        }

        // Update sprite facing direction
        if (spriteRenderer != null)
        {
            float direction = Mathf.Sign(dx);
            
            if (spriteFacesRight)
            {
                spriteRenderer.flipX = direction < 0;
            }
            else
            {
                spriteRenderer.flipX = direction > 0;
            }
        }

        // Calculate movement step
        float step = Mathf.Sign(dx) * speed * Time.deltaTime;
        if (Mathf.Abs(step) > Mathf.Abs(dx)) step = dx;

        Vector3 nextPosition = transform.position + new Vector3(step, 0f, 0f);

        // Lock Y position if enabled
        if (lockFloorY)
        {
            nextPosition.y = floorY;
        }

        // Apply movement
        if (body != null)
        {
            body.MovePosition(nextPosition);
        }
        else
        {
            transform.position = nextPosition;
        }
    }
}
