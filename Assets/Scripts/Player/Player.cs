using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class Player : MonoBehaviour
{
    public static Player instance;

    public Animator anim;

    private CameraFollowObject cameraFollowObject;
    [SerializeField] private GameObject cameraFollowGo;
    [SerializeField] private float fallSpeedYDampingChangeThreshold;
    [SerializeField] private float aimCameraSize = 7f; 
    [SerializeField] private float cameraZoomSpeed = 5f;

    private float currentCameraDefaultSize;

    [Header("Player Stat")]
    public int maxHp = 5;
    public int curHp;
    private bool isInvincible;
    public float invincibleTime = 1.5f;     // 무적 유지 시간
    public float blinkInterval = 0.6f;    // 깜빡임 간격
    [SerializeField] private Image hitEffect;
    [SerializeField] public GameObject playerHitEettct;

    [HideInInspector] public float horizontal;
    [HideInInspector] public float vertical;

    [HideInInspector] public Vector2 mousePos;
    private Camera mainCam;

    [Header("Swim Dash Attack Settings")]
    public GameObject dashDirectionIndicator;
    [Range(0.01f, 1f)] public float swimBulletTimeScale = 0.1f; // 불렛타임 느려지는 정도
    public bool isAimingSwimDash = false; // 현재 조준 중인지 여부
    private float defaultFixedDeltaTime; // 원래 물리 업데이트 시간 저장용

    public bool isMove;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        InIt();
    }

    void Update()
    {
        GetInputs();

        if(anim != null)
        {
            anim.SetBool("IsGrounded", IsGrounded());

            if(!isSwimming)
            {
                anim.SetFloat("VerticalVelocity", rb.velocity.y);
            }
            else
            {
                anim.SetFloat("VerticalVelocity", 0f);
            }
        }

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

        UpdateCameraZoom();
    }

    void FixedUpdate()
    {
        if (!isDashing)
            Move();
    }

    public void InIt()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        originalGravityScale = rb.gravityScale;
        mainCam = Camera.main;

        if (CameraManager.instance.curCamera != null)
        {
            currentCameraDefaultSize = CameraManager.instance.curCamera.m_Lens.OrthographicSize;
        }

        defaultFixedDeltaTime = Time.fixedDeltaTime;

        cameraFollowObject = cameraFollowGo.GetComponent<CameraFollowObject>();
        fallSpeedYDampingChangeThreshold = CameraManager.instance.fallSpeedYDampingChangeThreshold;
        curHp = maxHp;
        if (dashDirectionIndicator != null) dashDirectionIndicator.SetActive(false);
        moveEffect[1].SetActive(false);

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
            else
            {
                PerformAttack();
            }
        }
      

        if (!isSwimming && Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
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
        GameObject effect = Instantiate(playerHitEettct, gameObject.transform);
        if (isInvincible == true)
        {
            return;
        }
        curHp--;
        if(curHp <=0)
        {
            
        }
        CameraShake.Instance.Shake(0.2f, 0.2f);
        StartCoroutine(HitEffect());
        StartCoroutine(InvincibleBlink());
    }

    private IEnumerator InvincibleBlink()
    {
        isInvincible = true;
        float elapsed = 0f;

        Color originalColor = sprite.color;

        while (elapsed < invincibleTime)
        {
            sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.4f);

            yield return new WaitForSeconds(blinkInterval / 2);

            sprite.color = originalColor; 

            yield return new WaitForSeconds(blinkInterval / 2);

            elapsed += blinkInterval;
        }

        isInvincible = false;

        sprite.color = originalColor;
    }


    private IEnumerator HitEffect()
    {
        Color color = hitEffect.color;
        color.a = 0.4f;
        hitEffect.color = color;

        while (color.a >= 0.0f)
        {
            color.a -= Time.deltaTime;
            hitEffect.color = color;

            yield return null;
        }
    }

    public void Respawn()
    {
        if (GameManager.Instance.respawnPoint != null)
        {
            transform.position = GameManager.Instance.respawnPoint.position;
        }
    }

    private void UpdateCameraZoom()
    {
        CinemachineVirtualCamera activeCamera = CameraManager.instance.curCamera;

        if (activeCamera == null)
        {
            return;
        }

        float targetSize = isAimingSwimDash ? aimCameraSize : currentCameraDefaultSize;

        float currentSize = activeCamera.m_Lens.OrthographicSize;

        if (Mathf.Approximately(currentSize, targetSize))
        {
            return;
        }

        float newSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * cameraZoomSpeed);

        activeCamera.m_Lens.OrthographicSize = newSize;
    }

    public void OnCameraSwitched(CinemachineVirtualCamera newCamera)
    {
        currentCameraDefaultSize = newCamera.m_Lens.OrthographicSize;
    }

    //private void Die()



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

    
}