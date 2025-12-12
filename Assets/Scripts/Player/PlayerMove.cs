using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    [Header("Movement Stats")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private float jumpingPower = 20f;

    [HideInInspector] public bool isFacingRight = true;
    private bool isMoving;

    [Header("WallSliding")]
    private bool isWallSliding;
    [SerializeField] private float wallSlidingSpeed = 2f;
    private float wallJumpAcceleration = 50f;
    private float wallJumpMaxSpeed = 12f;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingCounter;
    [SerializeField] private float wallJumpingTime = 0.2f;
    [SerializeField] private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(10f, 18f);

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [Header("Jump Buffer")]
    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [Header("Dash Settings")]
    public bool canDash = true;
    private bool isDashing;
    private float postDashTimer = 0f;
    [SerializeField] private int currentAirDashCount = 1;
    [SerializeField] private float dashingPower = 30f;
    [SerializeField] private float dashingTime = 0.25f;
    [SerializeField] private float dashingCooldown = 0.2f;
    [SerializeField] private float postDashDuration = 0.3f;
    [SerializeField] private int maxAirDashCount = 1;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    [Header("Swimming Settings")]
    public float swimSpeed = 10f;
    [SerializeField] private float swimDashAttackPower = 40f;
    [SerializeField] private float swimDeceleration = 2f;
    [SerializeField] private float swimDashCooldownTime = 1.5f;
    [SerializeField] private bool isSwimming = false;
    [SerializeField] private LayerMask waterLayer;
    private bool aimInputLocked = false;
    private float originalGravityScale;
    public SpriteRenderer sprite;

    [Header("Swim Dash Attack Hit")]
    [SerializeField] private float dashAttackRadius = 0.8f;
    [SerializeField] private LayerMask dashAttackLayer;
    Vector3 originalScale;

    [Header("Swim Dash Aim Settings")]
    public float maxAimDuration = 2.0f;
    [HideInInspector] public float currentAimTimer;

    [Header("Dash Wall Check")]
    [SerializeField] private float wallCheckDistance = 0.5f; 
    [SerializeField] private float wallStopDeceleration = 50f;

    [SerializeField] private GameObject[] moveEffect;
    [SerializeField] private int curMoveEffectIndex = 0;

    private void Move()
    {
        if (isAimingSwimDash)
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("Move", false);
            return;
        }

        if (isAttacking && !isSwimming)
        {
            if (IsGrounded())
            {
                rb.velocity = Vector2.zero;
                
            }
            else
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }

            anim.SetBool("Move", false);
            return;
        }

        if (isSwimming)
        {
            if (isDashing) return;

            Vector2 inputDir = new Vector2(horizontal, vertical).normalized;

            if (inputDir.magnitude > 0.1f)
            {
                rb.velocity = Vector2.Lerp(rb.velocity, inputDir * swimSpeed, Time.fixedDeltaTime * 5f);
            }
            else
            {
                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, Time.fixedDeltaTime * swimDeceleration);
            }



            bool hasInput = inputDir.magnitude > 0.01f;       // 키보드를 누르고 있는가?
            bool hasVelocity = rb.velocity.magnitude > 0.1f;  // 관성으로 움직이는 중인가? (0.5는 너무 커서 0.1로 낮춤)

            anim.SetBool("Move", hasInput || hasVelocity);

            return;
        }

        float targetSpeed = horizontal * speed;

        bool isRunning = Mathf.Abs(horizontal) > 0.01f; // 이동 중이면 true
        if(isRunning)
        {
        }
        anim.SetBool("Move", isRunning);

        if (isWallJumping)
        {
            // 벽점프 중이면 점진적으로 가속
            float newVelX = Mathf.MoveTowards(rb.velocity.x, wallJumpingDirection * wallJumpMaxSpeed, wallJumpAcceleration * Time.fixedDeltaTime);
            rb.velocity = new Vector2(newVelX, rb.velocity.y);
            return;
        }

        if (!isWallJumping)
        {
            if (isDashing)
            {
                // 지상 대시 중 수평 속도 유지
                rb.velocity = new Vector2(transform.localScale.x * dashingPower, rb.velocity.y);
                return;
            }

            if (postDashTimer > 0f)
            {
                // 대시 후 관성 유지, 점진적 감속
                float decel = 20f;
                rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, 0f, decel * Time.fixedDeltaTime), rb.velocity.y);
                postDashTimer -= Time.fixedDeltaTime;
            }
            else
            {
                rb.velocity = new Vector2(targetSpeed, rb.velocity.y);
            }
        }
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
            currentAirDashCount = maxAirDashCount;
        }
        else coyoteTimeCounter -= Time.deltaTime;

        if (Input.GetButtonDown("Jump")) jumpBufferCounter = jumpBufferTime;
        else jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            jumpBufferCounter = 0f;
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }
    }

    private void WallSlide()
    {
        if (IsWalled() && horizontal != 0f)
        {
            //anim.SetBool("isWallSliding", true);

            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            //anim.SetBool("isWallSliding", false);

            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -Mathf.Sign(transform.localScale.x);
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;

            // 초기 수평 속도는 약간만
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x * 0.5f, wallJumpingPower.y);

            wallJumpingCounter = 0f;

            // Flip 처리
            if (Mathf.Sign(transform.localScale.x) != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void TurnCheck()
    {
        if (isDashing) return;

        // 마우스 회전 충돌 방지
        if (isAttacking || isAimingSwimDash) return;

        // 키보드 입력 회전
        if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f))
        {
            Flip();
        }
    }

    public void LookAtMouse()
    {
        if (isDashing) return;

        // 마우스만 판단
        if (mousePos.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (mousePos.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
    }

    // 실제 몸을 뒤집는 기능만 담당 (조건 체크 없음)
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;

        if (cameraFollowObject != null) cameraFollowObject.CallTurn();
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        bool grounded = IsGrounded();

        if (grounded)
        {
            // 지상 대시
            rb.velocity = new Vector2(transform.localScale.x * dashingPower, rb.velocity.y);
            currentAirDashCount = maxAirDashCount; // 지상에 닿으면 공중 대시 초기화
        }
        else
        {
            // 공중 대시 가능 여부 체크
            if (currentAirDashCount <= 0)
            {
                rb.gravityScale = originalGravity;
                isDashing = false;
                canDash = true;
                yield break;
            }

            rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
            currentAirDashCount--; // 공중 대시 사용
            Debug.Log("대쉬");

        }

        yield return new WaitForSeconds(dashingTime);

        rb.gravityScale = originalGravity;

        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private void StartSwimDashAim()
    {
        if (isAimingSwimDash || aimInputLocked) return;

        isAimingSwimDash = true;
        canDash = false;

        currentAimTimer = maxAimDuration;

        Time.timeScale = swimBulletTimeScale;
        Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;

        if (dashDirectionIndicator != null)
        {
            dashDirectionIndicator.SetActive(true);
            UpdateAimingIndicator();
        }

    }

    public void UpdateSwimAiming()
    {
        LookAtMouse();

        UpdateAimingIndicator();

        currentAimTimer -= Time.unscaledDeltaTime;

        if (currentAimTimer <= 0)
        {
            aimInputLocked = true;
            CancelSwimDashAim();
        }
    }

    private void CancelSwimDashAim()
    {
        if (!isAimingSwimDash) return;

        ResetBulletTime();
        isAimingSwimDash = false;
        canDash = true;
            
        if (dashDirectionIndicator != null) dashDirectionIndicator.SetActive(false);
    }

    private void ResetBulletTime()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDeltaTime;
    }

    private void UpdateAimingIndicator()
    {
        if (dashDirectionIndicator == null) return;

        Vector2 dir = (mousePos - (Vector2)transform.position).normalized;

        if (transform.localScale.x < 0)
        {
            dir.x *= -1;
            dir.y *= -1;
        }

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        dashDirectionIndicator.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }

    private IEnumerator ExecuteSwimDashAttack()
    {
        isAimingSwimDash = false;
        isDashing = true;

        isInvincible = true;

        yield return new WaitForSeconds(0.05f);
        ResetBulletTime();
        if (dashDirectionIndicator != null) dashDirectionIndicator.SetActive(false);

        Vector2 dashDir = (mousePos - (Vector2)transform.position).normalized;
       
        if (dashDir == Vector2.zero) dashDir = new Vector2(transform.localScale.x, 0);

        originalScale = transform.localScale;

        float angle = Mathf.Atan2(dashDir.y, dashDir.x) * Mathf.Rad2Deg;

        Vector3 targetScale = originalScale;
        float finalAngle = angle;

        if (dashDir.x < 0)
        {
            targetScale.y = -Mathf.Abs(originalScale.y);

            if (angle > 0)
                finalAngle = angle - 180f;
            else
                finalAngle = angle + 180f;
        }
        else 
        {
            targetScale.y = Mathf.Abs(originalScale.y);
            finalAngle = angle;
        }
        transform.localScale = targetScale;
        transform.rotation = Quaternion.Euler(0f, 0f, finalAngle); 

        rb.velocity = dashDir * swimDashAttackPower;
        anim.SetTrigger("WaterDash");

        float dashTimer = 0f;

        List<Collider2D> hitEnemies = new List<Collider2D>();

        while (dashTimer < dashingTime)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, dashAttackRadius, dashAttackLayer);

            foreach (Collider2D hit in hits)
            {
                if (hit != null && !hitEnemies.Contains(hit))
                {
                    EnemyBase enemy = hit.GetComponent<EnemyBase>();

                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage, dashDir, 15f);
                        hitEnemies.Add(hit);
                        AttackShake();
                    }

                    SunkenWarrior boss = hit.GetComponent<SunkenWarrior>();

                    if (boss != null)
                    {
                        boss.TakeDamage(damage, dashDir, 15f);
                        hitEnemies.Add(hit);
                        AttackShake();
                    }
                }
            }

            dashTimer += Time.deltaTime;

            yield return null;
        }
        yield return new WaitForSeconds(swimDashCooldownTime);

        canDash = true;
    }

    public void SwimDashPos()
    {
        isDashing = false;
        isInvincible = false;
        transform.localScale = originalScale;
        transform.rotation = Quaternion.identity;
    }

    private void EnterWater()
    {
        isSwimming = true;
        rb.gravityScale = 0f;
        rb.velocity = rb.velocity * 0.5f;

        anim.SetBool("IsSwimming", true);
        currentAirDashCount = maxAirDashCount;

        moveEffect[0].SetActive(false);

        moveEffect[1].SetActive(true);
    }

    private void ExitWater()
    {
        if (isAimingSwimDash) CancelSwimDashAim();

        isSwimming = false;
        rb.gravityScale = originalGravityScale;

        transform.rotation = Quaternion.identity;
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

        anim.SetBool("IsSwimming", false);

        moveEffect[0].SetActive(true);

        moveEffect[1].SetActive(false);
    }

    // 대시 공격 범위 확인용 기즈모
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(dashDirectionIndicator.transform.position, dashAttackRadius);
    }
}