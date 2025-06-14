using UnityEngine;

public class RobotVacuumController : MonoBehaviour
{
    public float moveSpeed = 2f; // Adjustable speed in Inspector
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float collisionTimer = 0f; // Tracks time since last collision
    private int collisionCount = 0; // Counts rapid collisions
    private const float STUCK_THRESHOLD = 0.2f; // Faster response
    private const int MAX_COLLISIONS = 2; // Quicker escape
    private Vector2 lastPosition; // Track position to detect stuck
    private float stuckCheckTimer = 0f; // Timer for stuck check
    private const float STUCK_DISTANCE = 0.05f; // Smaller distance for stuck detection

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveDirection = Random.insideUnitCircle.normalized;
        lastPosition = transform.position;
    }

    void Update()
    {
        // Move the robot
        rb.linearVelocity = moveDirection * moveSpeed;

        // Decrease collision timer
        if (collisionTimer > 0)
        {
            collisionTimer -= Time.deltaTime;
        }

        // Check if stuck (not moving)
        stuckCheckTimer += Time.deltaTime;
        if (stuckCheckTimer >= 0.3f)
        {
            if (Vector2.Distance(transform.position, lastPosition) < STUCK_DISTANCE)
            {
                Debug.Log("Stuck detected, attempting escape");
                collisionCount = MAX_COLLISIONS;
                TryEscape();
            }
            lastPosition = transform.position;
            stuckCheckTimer = 0f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Collided with: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Furniture"))
        {
            collisionTimer = STUCK_THRESHOLD;
            collisionCount++;

            if (collisionCount >= MAX_COLLISIONS)
            {
                TryEscape();
                collisionCount = 0;
            }
            else
            {
                ChangeDirection();
            }
        }
    }

    void TryEscape()
    {
        // Try to find a clear direction
        if (TryClearDirection())
        {
            rb.AddForce(moveDirection * 1f, ForceMode2D.Impulse); // Small nudge
            return;
        }

        // Fallback: Move toward room center
        Vector2 roomCenter = Vector2.zero; // Adjust if different
        moveDirection = (roomCenter - (Vector2)transform.position).normalized;
        rb.AddForce(moveDirection * 2f, ForceMode2D.Impulse); // Stronger nudge
    }

    bool TryClearDirection()
    {
        for (int i = 0; i < 10; i++) // More attempts
        {
            Vector2 newDirection = Random.insideUnitCircle.normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, newDirection, 1.5f, LayerMask.GetMask("Wall", "Furniture"));
            if (!hit.collider)
            {
                moveDirection = newDirection;
                return true;
            }
        }
        return false;
    }

    public void ChangeDirection()
    {
        moveDirection = Random.insideUnitCircle.normalized;
    }

    public void SetSpeed(float newSpeed)
    {
        moveSpeed = Mathf.Clamp(newSpeed, 0.5f, 10f);
    }
}