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

    [Header("Crouch (bite)")]
    [SerializeField] private bool holdToCrouch = true;

    [Header("Game Flow")]
    [SerializeField] private bool pressAnyKeyToStart = true;

    private bool isGrounded;
    private bool isCrouching;
    private bool started;

    void Reset()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1) Start game
        if (!started && (!pressAnyKeyToStart || Input.anyKeyDown))
        {
            started = true;
        }

        // 2) Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 3) Input
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space)
                        || Input.GetMouseButtonDown(0) // Chuột trái
                        || Input.GetKeyDown(KeyCode.UpArrow);

        bool crouchPressed = Input.GetMouseButtonDown(1); // Chuột phải nhấn 1 lần
        bool crouchHeld = Input.GetMouseButton(1);     // Chuột phải giữ

        // 4) Crouch / Bite logic
        if (holdToCrouch)
            isCrouching = crouchHeld && isGrounded;
        else
        {
            if (crouchPressed && isGrounded) isCrouching = !isCrouching;
        }

        // 5) Jump
        if (started && !isCrouching && isGrounded && jumpPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // 6) Horizontal move (endless runner)
        float targetXVel = 0f;
        if (started && autoRun)
            targetXVel = isCrouching ? runSpeed * 0.85f : runSpeed;

        rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, targetXVel, 0.25f), rb.linearVelocity.y);

        // 7) Animator params
        animator.SetBool("idle", !started);
        animator.SetBool("move", started && isGrounded && !isCrouching && Mathf.Abs(rb.linearVelocity.x) > 0.1f);
        animator.SetBool("jump", !isGrounded);
        animator.SetBool("bite", isCrouching);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
