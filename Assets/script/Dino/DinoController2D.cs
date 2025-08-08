using UnityEngine;

public class DinoController2D : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    [Header("Move")]
    [SerializeField] private bool autoRun = true;
    [SerializeField] private float runSpeed = 6f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Game Flow")]
    [SerializeField] private bool pressAnyKeyToStart = true;

    private bool isGrounded;
    private bool started;

    void Reset()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Start
        if (!started && (!pressAnyKeyToStart || Input.anyKeyDown)) started = true;

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Input jump (Space / Left mouse / Up)
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space)
                        || Input.GetMouseButtonDown(0)
                        || Input.GetKeyDown(KeyCode.UpArrow);

        // Jump
        if (started && isGrounded && jumpPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // Endless run (giữ chạy kể cả trên không)
        float targetXVel = (started && autoRun) ? runSpeed : 0f;
        rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, targetXVel, 0.25f), rb.linearVelocity.y);

        // Animator: luôn Move khi đã start
        animator.SetBool("move", started && Mathf.Abs(rb.linearVelocity.x) > 0.05f);

        // (Optional) điều chỉnh tốc độ animation khi trên không cho đẹp
        animator.speed = (isGrounded ? 1f : 1.0f); // tăng lên 1.1–1.2 nếu muốn chân quay nhanh hơn khi nhảy
    }

    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
