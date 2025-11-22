using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public partial class Player : MonoBehaviour
{
    public Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            isJumpInputBuffered = true;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            TryDash();
        }

        if (isJumpingCancel)
        {
            rb.velocity = Vector2.zero;
            isJumpingCancel = false;
        }

        GetInputs();
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            jumpCount = 0;
        }

        if (!isDashing)
        {
            SlopeCheck();
            Move();
            Jump();
        }
    }

    private void GetInputs()
    {
        if(Input.GetKeyDown(KeyCode.X) && !isAttacking)
        {
          StartCoroutine( Attack());

        }
        xAxis = Input.GetAxis("Horizontal");
        yAxis = Input.GetAxis("Vertical");

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            nowHp--;
            CameraShake.Instance.Shake(0.2f,0.2f);
            StartCoroutine(FlashColorOnHit());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
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
