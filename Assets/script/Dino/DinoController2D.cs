using UnityEngine;

public class DinoController2D : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    [Header("Move")]
    [SerializeField] private bool autoRun = true;
    [SerializeField] private float runSpeed = 6f;     // tốc độ khởi đầu
    [SerializeField] private float maxRunSpeed = 20f; // trần tốc độ
    [SerializeField] private float acceleration = 2f; // đơn vị: units/second

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Game Flow")]
    [SerializeField] private bool pressAnyKeyToStart = true;

    private bool isGrounded;
    private bool started;
    private float currentSpeed; // tốc độ đang chạy (tăng dần tới max)

    void Reset()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        currentSpeed = runSpeed;
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

        // Tăng tốc theo thời gian nhưng không vượt maxRunSpeed
        if (started && autoRun)
        {
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxRunSpeed);
        }
        else
        {
            currentSpeed = 0f;
        }

        // Đẩy rigidbody tới tốc độ mục tiêu (giữ mượt)
        float targetXVel = currentSpeed;
        rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, targetXVel, 0.25f), rb.linearVelocity.y);

        // Animator
        animator.SetBool("move", started && Mathf.Abs(rb.linearVelocity.x) > 0.05f);
        animator.speed = 1f; // chỉnh nếu muốn nhanh/chậm hơn
    }

    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
