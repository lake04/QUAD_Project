using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
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
    public bool isFacingRight = true;

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

    public SpriteRenderer sprite;
    public bool canDash;


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
                Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // 입력 방향에 따라 Sprite 뒤집기 (수영 중에는 회전이 메인이므로 flipX 로직은 제외)
            }

            rb.velocity = transform.right * inputVector.magnitude * swimSpeed;
        }
        else
        {
            // 지상/공중 이동 로직
            if (xAxis != 0f)
            {
                anim.SetBool("Move", true);
            }
            else
            {
                anim.SetBool("Move", false);
            }

            rb.velocity = new Vector2(xAxis * moveSpeed, rb.velocity.y);
        }
    }

    private void TurnCheck()
    {
        if(xAxis>0 && !isFacingRight)
        {
            Turn();
        }
        else if(xAxis < 0 && isFacingRight)
        {
            Turn();
        }
    }

    private void Turn()
    {
        if(isFacingRight)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            isFacingRight = !isFacingRight;

            cameraFollowObject.CallTurn();
        }
        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            isFacingRight = !isFacingRight;

            cameraFollowObject.CallTurn();
        }
    }

    private void Jump()
    {
        // Jump 실행 로직
        if (isJumpInputBuffered == true && jumpCount < maxJumpCount)
        {
            anim.SetBool("Move", false);

            isJumping = true;
            jumpTimeCounter = jumpTime;

            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            jumpCount++;
            isJumpInputBuffered = false;
        }

        // Jump Cut 로직
        if (Input.GetKeyUp(KeyCode.Z) || isJumpingCancel)
        {
            isJumping = false;
            isJumpingCancel = false;

            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier);
            }
        }

        // Jump Hold 로직
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
        // isSwimming 체크는 Player.cs의 HandleState에서 처리
        if (Time.time < lastDashTime + dashCooldown) return;

        ChangeState(PlayerState.Dash);
        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        lastDashTime = Time.time;

        float direction = Input.GetAxisRaw("Horizontal");
        if (direction == 0)
            direction = transform.localScale.x > 0 ? 1 : -1;

        // FSM에서 ExitDash 상태를 처리하기 위해 중력 설정 로직 제거
        rb.gravityScale = 0;

        rb.velocity = new Vector2(direction * dashPower, 0f);

        yield return new WaitForSeconds(dashTime);

        // Dash 종료 후 상태 복귀
        ChangeState(PlayerState.Idle);
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
                rb.gravityScale = originalGravityScale;
            }
        }
        else
        {
            isOnSlope = false;
            rb.gravityScale = originalGravityScale;
        }
    }

    // 물 진입 로직
    private void EnterWater()
    {
        isSwimming = true;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;

        //anim.SetBool("IsSwimming", true);
    }

    // 물 이탈 로직
    private void ExitWater()
    {
        isSwimming = false;
        rb.gravityScale = originalGravityScale; // 원래 중력으로 복귀

        transform.rotation = Quaternion.identity;

        rb.velocity = new Vector2(rb.velocity.x, 0f);

        jumpCount = 0;
        isJumping = false;
        isJumpInputBuffered = false;
        jumpTimeCounter = 0f;

        //anim.SetBool("IsSwimming", false);
    }
}