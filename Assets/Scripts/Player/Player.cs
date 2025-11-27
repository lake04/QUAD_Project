using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.U2D;

// 새로운 상태: Swim 추가
public enum PlayerState
{
    Idle,
    Run,
    Jump,
    Attack,
    Dash,
    Swim, // 수영 상태 추가
}

public partial class Player : MonoBehaviour
{
    public Animator anim;

    // FSM 변수 추가
    public PlayerState currentState;

    private CameraFollowObject cameraFollowObject;
    [SerializeField] private GameObject cameraFollowGo;
    [SerializeField] private float fallSpeedYDampingChangeThreshold;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        // 중력 스케일 초기값을 Start에서 설정합니다.
        originalGravityScale = rb.gravityScale;

        // 초기 상태 설정
        ChangeState(PlayerState.Idle);

        cameraFollowObject = cameraFollowGo.GetComponent<CameraFollowObject>();

        fallSpeedYDampingChangeThreshold = CameraManager.instance.fallSpeedYDampingChangeThreshold;
    }

    void Update()
    {
        // === 입력 감지는 Update에서 항상 처리 ===
        GetInputs();

        // Z키 (점프 버퍼링)
        if (Input.GetKeyDown(KeyCode.Z))
        {
            isJumpInputBuffered = true;
        }

        // C키 (대시 시도)
        if (Input.GetKeyDown(KeyCode.C))
        {
            TryDash();
        }

        // 상태 처리 메서드 호출
        HandleState();
        TurnCheck();

        if(rb.velocity.y < fallSpeedYDampingChangeThreshold && !CameraManager.instance.isLerpingYDamping && !CameraManager.instance.lerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }

        if(rb.velocity.y >= 0f && !CameraManager.instance.isLerpingYDamping && CameraManager.instance.lerpedFromPlayerFalling)
        {
            CameraManager.instance.lerpedFromPlayerFalling = false;

            CameraManager.instance.LerpYDamping(false);
        }
    }

    void FixedUpdate()
    {
        // Ground Check는 항상 실행
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 접지 상태에 따른 점프 카운트 초기화
        if (isGrounded)
        {
            jumpCount = 0;
        }

        // 물리 기반 상태 처리
        HandleFixedState();
    }

    // 상태 변경 메서드
    public void ChangeState(PlayerState newState)
    {
        if (currentState == newState) return;

        // Exit State 로직
        if (currentState == PlayerState.Dash)
        {
            rb.gravityScale = originalGravityScale;
            isDashing = false;
        }

        currentState = newState;

        // Enter State 로직
    }

    // Update()에서 상태별 논리/입력 처리
    private void HandleState()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
            case PlayerState.Run:
            case PlayerState.Jump:
                CheckMovementStateTransition();
                break;
            case PlayerState.Attack:
                CheckMovementStateTransition();
                break;
            case PlayerState.Dash:
                // DashCoroutine에서 종료 처리
                break;
            case PlayerState.Swim:
                CheckSwimStateTransition();
                break;
        }

        // X키 (공격 입력 플래그 설정)
        isAttack = Input.GetKeyDown(KeyCode.X);
    }

    // FixedUpdate()에서 상태별 물리 로직 처리
    private void HandleFixedState()
    {
        // Dash 상태가 아닐 때만 경사로 체크
        if (currentState != PlayerState.Dash)
        {
            SlopeCheck();
        }

        switch (currentState)
        {
            case PlayerState.Idle:
            case PlayerState.Run:
            case PlayerState.Attack:
                Move();
                Jump();
                break;
            case PlayerState.Jump:
                Move();
                Jump();
                break;
            case PlayerState.Swim:
                Move();
                break;
            case PlayerState.Dash:
                // DashCoroutine에서 물리 처리
                break;
        }
    }

    // --- FSM Transition Helpers ---

    private void CheckMovementStateTransition()
    {
        if (isDashing) return;

        if (isAttacking)
        {
            // 공격 중 상태 전환은 Attack 코루틴 종료 시점에서 처리
        }
        else if (isGrounded)
        {
            if (xAxis != 0)
            {
                ChangeState(PlayerState.Run);
            }
            else
            {
                ChangeState(PlayerState.Idle);
            }
        }
        else // 공중에 있으면
        {
            ChangeState(PlayerState.Jump);
        }

        // 공격 시작은 모든 비-Dash 상태에서 가능
        if (isAttack && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private void CheckSwimStateTransition()
    {
        // 수영 중 공격
        if (isAttack && !isAttacking)
        {
            // 수영 중 공격 로직이 지상 공격과 동일하다는 가정 하에 호출
            StartCoroutine(Attack());
        }
    }

    private void GetInputs()
    {
        // Input.GetKeyDown(KeyCode.X)는 HandleState에서 isAttack 플래그로 처리
        xAxis = Input.GetAxis("Horizontal");
        yAxis = Input.GetAxis("Vertical");
    }

    // --- OnTrigger 로직 (수영 상태 진입/이탈) ---

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            nowHp--;
            CameraShake.Instance.Shake(0.2f, 0.2f);
            StartCoroutine(FlashColorOnHit());
        }

        // 물 진입 시 상태 변경
        if (((1 << collision.gameObject.layer) & waterLayer) != 0 && currentState != PlayerState.Swim)
        {
            EnterWater();
            ChangeState(PlayerState.Swim);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 물에서 이탈 시 상태 변경
        if (((1 << collision.gameObject.layer) & waterLayer) != 0 && currentState == PlayerState.Swim)
        {
            ExitWater();
            ChangeState(PlayerState.Idle); // 지상으로 복귀
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            nowHp--;
            CameraShake.Instance.Shake(0.2f, 0.2f);
            StartCoroutine(FlashColorOnHit());
        }
    }

    protected virtual IEnumerator FlashColorOnHit()
    {
        Color originalColor = sprite.color;
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = originalColor;
    }
    public void Respawn()
    {
        if (GameManager.Instance.respawnPoint != null)
        {
            transform.position = GameManager.Instance.respawnPoint.position;
        }
    }
}