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
    public int cuerHp;

    [HideInInspector] public float horizontal;
    [HideInInspector] public float vertical;
    [HideInInspector] public bool jumpDown;
    [HideInInspector] public bool jumpHeld;
    [HideInInspector] public bool jumpUp;

    [HideInInspector] public Vector2 mousePos;
    private Camera mainCam;

    private void Awake()
    {
        if (instance == null) instance = this;
    }
    void Start()
    {
        Initialized();
    }

    void Update()
    {
        GetInputs();

        Movement();

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

    void FixedUpdate()
    {
        if (!isDashing)
            Move();
    }

    private void Movement()
    {
        Jump();

        WallSlide();
        WallJump();

        if (!isWallJumping)
            TurnCheck();
    }

    private void GetInputs()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Debug.Log("xAxis: " + horizontal + ", yAxis: " + vertical);

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            StartCoroutine(Dash());

        isAttack = Input.GetMouseButtonDown(0);

        if (isAttack && !isAttacking)
        {
            StartCoroutine(Attack());
        }

        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
    }

    // --- OnTrigger 로직 (수영 상태 진입/이탈) ---

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }

        // 물 진입 시 상태 변경
        //if (((1 << collision.gameObject.layer) & waterLayer) != 0 && currentState != PlayerState.Swim)
        //{
        //    EnterWater();
        //    ChangeState(PlayerState.Swim);
        //}
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    // 물에서 이탈 시 상태 변경
    //    if (((1 << collision.gameObject.layer) & waterLayer) != 0 && currentState == PlayerState.Swim)
    //    {
    //        ExitWater();
    //        ChangeState(PlayerState.Idle); // 지상으로 복귀
    //    }
    //}

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
        cuerHp--;
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

    public void Initialized()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        originalGravityScale = rb.gravityScale;
        mainCam = Camera.main;

        cameraFollowObject = cameraFollowGo.GetComponent<CameraFollowObject>();
        fallSpeedYDampingChangeThreshold = CameraManager.instance.fallSpeedYDampingChangeThreshold;
        cuerHp = maxHp;
    }
}