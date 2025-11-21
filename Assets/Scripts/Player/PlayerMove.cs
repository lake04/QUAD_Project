using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

public partial class Player
{
    private Rigidbody2D rb;

    [Header("Move")]
    public float moveSpeed = 5;
    private float xAxis;
    private float yAxis;

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

    [Header("Swimming")]
    public float swimSpeed = 3.5f;
    public float rotationSpeed = 500f; 
    public bool isSwimming = false;
    public LayerMask waterLayer;

    private float originalGravityScale;

    private SpriteRenderer sprite;


    private void Move()
    {
        if (isSwimming)
        {
            // 수영 이동 로직
            Vector2 inputVector = new Vector2(xAxis, yAxis).normalized;

            // 회전 로직
            if (inputVector != Vector2.zero)
            {
                float targetAngle = Mathf.Atan2(inputVector.y, inputVector.x) * Mathf.Rad2Deg;

                // 현재 회전 값과 목표 각도 사이보간
                Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // 입력 방향에 따라 Sprite 뒤집기
                //sprite.flipX = xAxis < 0; 
            }

            rb.velocity = transform.right * inputVector.magnitude * swimSpeed;
        }
        else
        {
            if (xAxis != 0f)
            {
                anim.SetBool("Move", true);
            }
            else if (yAxis == 0)
            {
                anim.SetBool("Move", false);
            }

            rb.velocity = new Vector2(xAxis * moveSpeed, rb.velocity.y);

            if (xAxis != 0)
            {
                sprite.flipX = xAxis > 0;
                transform.rotation = Quaternion.identity;
            }
        }
    }

    private void Jump()
    {
        if (isSwimming) return;

        if (isJumpInputBuffered == true && jumpCount < maxJumpCount)
        {
            anim.SetBool("Move", false);

            isJumping = true;
            jumpTimeCounter = jumpTime;

            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            jumpCount++;
            isJumpInputBuffered = false;
        }

        if (Input.GetKeyUp(KeyCode.Z) || isJumpingCancel)
        {
            isJumping = false;
            isJumpingCancel = false;

            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier);
            }
        }

        if (Input.GetKey(KeyCode.Z) && isJumping)
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

    private void TryDash()
    {
        if (isSwimming) return;

        if (Time.time < lastDashTime + dashCooldown) return;
        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
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

    private void SlopeCheck()
    {
        if (isSwimming) // 수영 중에는 슬로프 체크 비활성화
        {
            isOnSlope = false;
            rb.gravityScale = 0f;
            return;
        }

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
    private void EnterWater()
    {
        isSwimming = true;
        originalGravityScale = rb.gravityScale; 
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;

        //anim.SetBool("IsSwimming", true);
    }

    private void ExitWater()
    {
        isSwimming = false;
        rb.gravityScale = originalGravityScale;

        //anim.SetBool("IsSwimming", false);
    }


}
