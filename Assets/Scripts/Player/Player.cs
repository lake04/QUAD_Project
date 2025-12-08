using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    public static Player instance;

    public Animator anim;

    private CameraFollowObject cameraFollowObject;
    [SerializeField] private GameObject cameraFollowGo;
    [SerializeField] private float fallSpeedYDampingChangeThreshold;

    [Header("Player Stat")]
    public int maxHp = 5;
    public int nowHp;

    [HideInInspector] public float horizontal;
    [HideInInspector] public float vertical;

    [HideInInspector] public Vector2 mousePos;
    private Camera mainCam;

    [Header("Swim Dash Attack Settings")]
    public GameObject dashDirectionIndicator;
    [Range(0.01f, 1f)] public float swimBulletTimeScale = 0.1f; // 불렛타임 느려지는 정도
    public bool isAimingSwimDash = false; // 현재 조준 중인지 여부
    private float defaultFixedDeltaTime; // 원래 물리 업데이트 시간 저장용

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        originalGravityScale = rb.gravityScale;
        mainCam = Camera.main;

        defaultFixedDeltaTime = Time.fixedDeltaTime;

        cameraFollowObject = cameraFollowGo.GetComponent<CameraFollowObject>();
        fallSpeedYDampingChangeThreshold = CameraManager.instance.fallSpeedYDampingChangeThreshold;

        if (dashDirectionIndicator != null) dashDirectionIndicator.SetActive(false);
    }

    void Update()
    {
        GetInputs();

        if (isAimingSwimDash)
        {
            UpdateAimingIndicator();
        }

        if (!isSwimming && !isAimingSwimDash) // 조준 중이 아닐 때만 카메라 댐핑 적용
        {
            if (rb.velocity.y < fallSpeedYDampingChangeThreshold && !CameraManager.instance.isLerpingYDamping && !CameraManager.instance.lerpedFromPlayerFalling)
            {
                CameraManager.instance.LerpYDamping(true);
            }

            if (rb.velocity.y >= 0f && !CameraManager.instance.isLerpingYDamping && CameraManager.instance.lerpedFromPlayerFalling)
            {
                CameraManager.instance.lerpedFromPlayerFalling = false;
                CameraManager.instance.LerpYDamping(false);
            }
        }

        Movement();
    }

    void FixedUpdate()
    {
        if (!isDashing)
            Move();
    }

    private void Movement()
    {
        if (isAimingSwimDash) return;

        if (!isSwimming)
        {
            Jump();
            WallSlide();
            WallJump();
        }

        if (!isWallJumping)
            TurnCheck();
    }

    private void GetInputs()
    {
        // 조준 중에는 이동 입력 무시 (선택 사항)
        if (!isAimingSwimDash)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }
        else
        {
            horizontal = 0;
            vertical = 0;
        }

        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        if (isSwimming && (canDash || isAimingSwimDash))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (!aimInputLocked)
                {
                    if (!isAimingSwimDash)
                    {
                        StartSwimDashAim();
                    }
                    else
                    {
                        UpdateSwimAiming();
                    }
                }
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                aimInputLocked = false;
                CancelSwimDashAim();
            }
        }
        else if (isAimingSwimDash)
        {
            aimInputLocked = false;
            CancelSwimDashAim();
        }

        isAttack = Input.GetMouseButtonDown(0);
        if (isAttack)
        {
            if (isAimingSwimDash)
            {
                StartCoroutine(ExecuteSwimDashAttack());
            }
            else if (!isAttacking && !isSwimming)
            {
                StartCoroutine(Attack());
            }
        }

        if (!isSwimming && Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }

        if (((1 << collision.gameObject.layer) & waterLayer) != 0 && !isSwimming)
        {
            EnterWater();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & waterLayer) != 0 && isSwimming)
        {
            ExitWater();
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }


    public void TakeDamage(int _damage)
    {
        nowHp--;
        CameraShake.Instance.Shake(0.2f, 0.2f);
        StartCoroutine(FlashColorOnHit());
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