using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Singleton")]
    public static PlayerMove Instance;

    [Header("Move")]
    public float moveSpeed = 5;

    [Header("Jump")]
    public float jumpForce = 10f;
    public float jumpTime = 0.35f;
    public float jumpCutMultiplier = 0.5f;
    private float jumpTimeCounter;
    private bool isJumping;
    private bool isJumpInputBuffered = false;
    private bool isJumpingCancel = false;

    public int maxJumpCount = 2;
    private int jumpCount = 0;

    [Header("Ground Check")]
    public bool isGrounded = false;
    public Transform groundCheck;
    public float groundCheckRadius = 0.7f;
    public LayerMask groundLayer;

    public float slopeCheckDistance = 0.5f;
    public float maxSlopeAngle = 45f;
    private bool isOnSlope = false;
    private float slopeDownAngle;

    [Header("Dash")]
    public float dashPower = 20f;
    public float dashTime = 0.15f;
    public float dashCooldown = 0.5f;
    private bool isDashing = false;
    private float lastDashTime = -999f;

    [Header("Player Stat")]
    public int maxHp;
    private int nowHp;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumpInputBuffered = true;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            TryDash();
        }

        if (isJumpingCancel)
        {
            rb.velocity = Vector2.zero;
            isJumpingCancel = false;
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            jumpCount = 0;
        }

        if (!isDashing)
        {
            SlopeCheck();
            Move();
            Jump();
        }
    }

    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void Jump()
    {
        if (isJumpInputBuffered == true && jumpCount < maxJumpCount)
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;

            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            jumpCount++;
            isJumpInputBuffered = false;
        }

        if (Input.GetKeyUp(KeyCode.Space) || isJumpingCancel)
        {
            isJumping = false;
            isJumpingCancel = false;

            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier);
            }
        }

        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
    }

    void TryDash()
    {
        if (Time.time < lastDashTime + dashCooldown) return;
        StartCoroutine(DashCoroutine());
    }

    IEnumerator DashCoroutine()
    {
        isDashing = true;
        lastDashTime = Time.time;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        float direction = Input.GetAxisRaw("Horizontal");
        if (direction == 0)
            direction = transform.localScale.x > 0 ? 1 : -1;

        rb.velocity = new Vector2(direction * dashPower, 0f);

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    void SlopeCheck()
    {
        Vector2 checkPos = transform.position - new Vector3(0f, 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundLayer);

        if (hit)
        {
            Vector2 normal = hit.normal;
            slopeDownAngle = Vector2.Angle(normal, Vector2.up);
            isOnSlope = slopeDownAngle > 0 && slopeDownAngle <= maxSlopeAngle;

            if (isOnSlope && isGrounded && rb.velocity.y <= 0)
            {
                rb.gravityScale = 0f;
                rb.velocity = new Vector2(rb.velocity.x, 0f);
            }
            else
            {
                rb.gravityScale = 1f;
            }
        }
        else
        {
            isOnSlope = false;
            rb.gravityScale = 1f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            nowHp--;
        }
    }

}
