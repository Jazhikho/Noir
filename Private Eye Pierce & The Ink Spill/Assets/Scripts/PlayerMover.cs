using UnityEngine;

public class PlayerMover2D : MonoBehaviour
{
    public float speed = 4f;
    public float stopDistance = 0.02f;
    /// <summary>If set, movement uses MovePosition so wall colliders block the player. Add Rigidbody2D (Kinematic) + Collider2D to the player.</summary>
    public Rigidbody2D body;

    private float? _targetX;

    public void SetTargetX(float x) => _targetX = x;

    private void Update()
    {
        if (_targetX == null) return;

        float dx = _targetX.Value - transform.position.x;
        if (Mathf.Abs(dx) <= stopDistance)
        {
            _targetX = null;
            return;
        }

        float step = Mathf.Sign(dx) * speed * Time.deltaTime;
        if (Mathf.Abs(step) > Mathf.Abs(dx)) step = dx;

        Vector3 nextPosition = transform.position + new Vector3(step, 0f, 0f);

        if (body != null)
            body.MovePosition(nextPosition);
        else
            transform.position = nextPosition;
    }
}