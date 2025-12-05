using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField]  private float fallSpeedYDampingChangeThreshold;

    [Header("Player Stat")]
    public int maxHp = 5;
    public int nowHp;

    public bool canDash;

    [HideInInspector] public float xAxis;
    [HideInInspector] public float yAxis;
    [HideInInspector] public bool jumpDown;
    [HideInInspector] public bool jumpHeld;
    [HideInInspector] public bool jumpUp;

    [HideInInspector] public Vector2 mousePos;
    private Camera mainCam;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        originalGravityScale = rb.gravityScale;
        mainCam = Camera.main;

        // 초기 상태 설정
        ChangeState(PlayerState.Idle);

        cameraFollowObject = cameraFollowGo.GetComponent<CameraFollowObject>();
        fallSpeedYDampingChangeThreshold = CameraManager.instance.fallSpeedYDampingChangeThreshold;
    }

    void Update()
    {
        // === 입력 감지는 Update에서 항상 처리 ===
        GetInputs();

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
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && !wasGrounded)
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
                break;
            case PlayerState.Dash:
                break;
            case PlayerState.Swim:
                CheckSwimStateTransition();
                break;
        }

        isAttack = Input.GetMouseButtonDown(0);
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
                Move();
                Jump();
                ApplyGravity();
                break;
            case PlayerState.Attack:
                rb.velocity = new Vector2(0, rb.velocity.y);
                ApplyGravity(); 
                break;
            case PlayerState.Jump:
                Move();
                Jump();
                ApplyGravity();
                break;
            case PlayerState.Swim:
                Move();
                break;
            case PlayerState.Dash:
                break;
        }
    }

    // --- FSM Transition Helpers ---

    private void CheckMovementStateTransition()
    {
        if (isDashing) { }

        if (isAttacking) return;

        if (isAttack && !isAttacking)
        {
            ChangeState(PlayerState.Attack); // 상태 변경
            StartCoroutine(Attack());
            return; // 공격 시작 시 아래 이동 로직 무시
        }

        else if (isGrounded)
        {
            ChangeState(xAxis != 0 ? PlayerState.Run : PlayerState.Idle);
        }
        else
        {
            ChangeState(PlayerState.Jump);
        }

        if (isAttack && !isAttacking)
        {
            StartCoroutine(Attack());
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            TryDash();
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

        jumpDown = Input.GetKeyDown(KeyCode.Space);
        jumpHeld = Input.GetKey(KeyCode.Space);
        jumpUp = Input.GetKeyUp(KeyCode.Space);

        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
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