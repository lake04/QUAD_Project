using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    private Rigidbody2D rb;

    [Header("Move Settings")]
    public float maxSpeed = 10f;
    public float maxAcceleration = 52f;
    public float maxDecceleration = 50f;
    public float maxTurnSpeed = 90f;

    [Header("Air Stats")]
    public float maxAirAcceleration = 52f;
    public float maxAirDeceleration = 52f;
    public float maxAirTurnSpeed = 80f;

    [Header("Jump Settings")]
    public float jumpHeight = 4.5f;
    public float timeToJumpApex = 0.4f;
    public float upwardMovementMultiplier = 1f;      
    public float downwardMovementMultiplier = 6.17f; 
    public float jumpCutOff = 2f;
    public int maxAirJumps = 2;
    public float speedLimit = 20f;

    [Header("Assists")]
    public float coyoteTime = 0.15f;
    public float jumpBuffer = 0.1f;

    [Header("State Checks")]
    public bool isFacingRight = true;
    public bool isGrounded = false;
    public bool isOnSlope = false;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private int jumpCount = 0;
    private float defaultGravityScale = 1f;

    [Header("Ground Check References")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;
    public float slopeCheckDistance = 0.5f;
    public float maxSlopeAngle = 45f;
    private float slopeDownAngle;

    [Header("Dash Settings")]
    public float dashPower = 20f;
    public float dashTime = 0.2f;
    public float dashCooldown = 0.5f;
    private bool isDashing = false;
    private float lastDashTime = -999f;

    [Header("Swimming Settings")]
    public float swimSpeed = 3.5f;
    public float rotationSpeed = 500f;
    public bool isSwimming = false;
    public LayerMask waterLayer;
    private float originalGravityScale;
    public SpriteRenderer sprite;

    private void Move()
    {
        if (isSwimming)
        {
            Vector2 inputVector = new Vector2(xAxis, yAxis).normalized;
            rb.velocity = transform.right * inputVector.magnitude * swimSpeed;
            return;
        }

        float targetSpeed = xAxis * maxSpeed;
        float acceleration, deceleration, turnSpeed;

        // 지상/공중 상태에 따라 다른 스탯 적용
        if (isGrounded)
        {
            acceleration = maxAcceleration;
            deceleration = maxDecceleration;
            turnSpeed = maxTurnSpeed;
        }
        else
        {
            acceleration = maxAirAcceleration;
            deceleration = maxAirDeceleration;
            turnSpeed = maxAirTurnSpeed;
        }

        float maxSpeedChange;

        if (xAxis != 0)
        {            
            if (Mathf.Sign(xAxis) != Mathf.Sign(rb.velocity.x) && Mathf.Abs(rb.velocity.x) > 0.1f)
            {
                maxSpeedChange = turnSpeed * Time.deltaTime;
            }
            else
            {
                maxSpeedChange = acceleration * Time.deltaTime;
            }
        }
        else
        {
            maxSpeedChange = deceleration * Time.deltaTime;
        }

        float currentSpeed = Mathf.MoveTowards(rb.velocity.x, targetSpeed, maxSpeedChange);

        if (isOnSlope && isGrounded && xAxis == 0) currentSpeed = 0;

        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);

        anim.SetBool("Move", xAxis != 0);
    }

    private void Jump()
    {
        // 코요테 타임 계산
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            jumpCount = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        
        if (jumpDown) jumpBufferCounter = jumpBuffer;
        else jumpBufferCounter -= Time.deltaTime;

        bool canJump = (isGrounded || coyoteTimeCounter > 0) || (jumpCount < maxAirJumps);

        if (jumpBufferCounter > 0 && canJump)
        {            
            if (!isGrounded && coyoteTimeCounter <= 0)
            {                
                if (jumpCount >= maxAirJumps) return;
            }

            PerformJump();
        }
    }

    private void PerformJump()
    {
        jumpBufferCounter = 0;
        coyoteTimeCounter = 0;
        jumpCount++;

        float gravity = (-2 * jumpHeight) / (timeToJumpApex * timeToJumpApex);
        float targetGravity = Physics2D.gravity.y * (gravity / Physics2D.gravity.y);
        float jumpSpeed = Mathf.Sqrt(-2f * targetGravity * jumpHeight);

        if (rb.velocity.y > 0f)
        {
            jumpSpeed = Mathf.Max(jumpSpeed - rb.velocity.y, 0f);
        }
        else if (rb.velocity.y < 0f)
        {
            jumpSpeed += Mathf.Abs(rb.velocity.y);
        }

        rb.velocity += new Vector2(0, jumpSpeed);

        anim.SetTrigger("Jump");
    }

    private void ApplyGravity()
    {
        // 중력 무시
        if (isSwimming || isDashing) { rb.gravityScale = 0; return; }
        if (isOnSlope && isGrounded && xAxis == 0) { rb.gravityScale = 0; return; }

        Vector2 newGravity = new Vector2(0, (-2 * jumpHeight) / (timeToJumpApex * timeToJumpApex));
        float baseGravityScale = (newGravity.y / Physics2D.gravity.y);

        float gravMultiplier = 1f;

        if (rb.velocity.y > 0.01f)
        {
            if (isGrounded)
            {
                gravMultiplier = defaultGravityScale;
            }
            else
            {
                if (jumpHeld)
                {
                    gravMultiplier = upwardMovementMultiplier;
                }
                else
                {
                    gravMultiplier = jumpCutOff;
                }
            }
        }
        else if (rb.velocity.y < -0.01f)
        {
            if (isGrounded)
            {
                gravMultiplier = defaultGravityScale;
            }
            else
            {
                gravMultiplier = downwardMovementMultiplier;
            }
        }
        else
        {
            gravMultiplier = defaultGravityScale;
        }

        rb.gravityScale = baseGravityScale * gravMultiplier;

        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -speedLimit, 100));
    }

    private void SlopeCheck()
    {
        if (isSwimming) return;
        Vector2 checkPos = transform.position - new Vector3(0f, 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundLayer);

        if (hit)
        {
            Vector2 normal = hit.normal;
            slopeDownAngle = Vector2.Angle(normal, Vector2.up);
            isOnSlope = slopeDownAngle > 0 && slopeDownAngle <= maxSlopeAngle;
        }
        else isOnSlope = false;
    }

    private void TurnCheck()
    {
        // 공격 중에는 키 입력으로 회전하지 않도록 방지
        if (isAttacking) return;

        if (xAxis > 0 && !isFacingRight)
        {
            Turn();
        }
        else if (xAxis < 0 && isFacingRight)
        {
            Turn();
        }
    }

    public void LookAtMouse()
    {
        if (mousePos.x > transform.position.x && !isFacingRight)
        {
            Turn();
        }
        else if (mousePos.x < transform.position.x && isFacingRight)
        {
            Turn();
        }
    }

    private void Turn()
    {
        isFacingRight = !isFacingRight;
        Vector3 rotator = transform.rotation.eulerAngles;
        rotator.y = isFacingRight ? 0f : 180f;
        transform.rotation = Quaternion.Euler(rotator);

        if (cameraFollowObject != null) cameraFollowObject.CallTurn();
    }

    private void TryDash()
    {
        if (isSwimming || Time.time < lastDashTime + dashCooldown) return;
        ChangeState(PlayerState.Dash);
        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        lastDashTime = Time.time;

        float direction = xAxis;
        if (direction == 0) direction = isFacingRight ? 1 : -1;

        rb.gravityScale = 0;
        rb.velocity = new Vector2(direction * dashPower, 0f);

        yield return new WaitForSeconds(dashTime);

        isDashing = false;
        ChangeState(PlayerState.Idle);
    }

    private void EnterWater()
    {
        isSwimming = true;
        rb.velocity = Vector2.zero;
    }

    private void ExitWater()
    {
        isSwimming = false;
        transform.rotation = Quaternion.identity;
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        jumpCount = 0;
    }
}