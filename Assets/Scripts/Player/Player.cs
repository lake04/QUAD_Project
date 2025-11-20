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
        if (Input.GetKeyDown(KeyCode.Space))
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
        Attack();
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
        isAttack = Input.GetKeyDown(KeyCode.X);
        xAxis = Input.GetAxis("Horizontal");
        yAxis = Input.GetAxis("Vertical");

    }
    public void Respawn()
    {
        if (GameManager.Instance.respawnPoint != null)
        {
            transform.position = GameManager.Instance.respawnPoint.position;
        }
    }
}
