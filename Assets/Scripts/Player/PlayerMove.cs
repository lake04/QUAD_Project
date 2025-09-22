using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Singleton")]
    public static PlayerMove Instance;


    #region 이동 or 점프
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

    [Header("Ground Check")]
    public bool isGrounded = false;
    public Transform groundCheck;
    public float groundCheckRadius = 0.7f;
    public LayerMask groundLayer;
    private bool isJumpCancle=false;

    [Header("Slope Chceck")]
    public float slopeCheckDistance = 0.5f;
    public float maxSlopeAngle = 45f;
    private bool isOnSlope = false;
    private float slopeDownAngle;

    #endregion

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
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isJumpInputBuffered = true;
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
        SlopeCheck();
        Move();
        Jump();
    }

    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);


         
        //spr.flipX = isFlip;
    }

    void Jump()
    {
        if (isJumpInputBuffered == true && isGrounded)
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
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

    void SlopeCheck()
    {
        Vector2 checkPos = transform.position - new Vector3(0f, 0.5f); // 발밑
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
