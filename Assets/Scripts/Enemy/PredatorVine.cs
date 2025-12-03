using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PredatorVine : EnemyBase
{
    private int nextMove = 1;
    public LayerMask checkLayer;
    private bool isAtice = false;
    [SerializeField] private float maxMoveDistance = 5f;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    protected override void Update()
    {
        base.Update();
    }

    private void FixedUpdate()
    {
        if (enemyState == EnemyState.Chasing)
        {
            isAtice = true;
        }
        else if (enemyState == EnemyState.Patrolling)
        {
            Patrolling();
        }
        Move();

    }

    protected override void Patrolling() 
    {
        rb.velocity = Vector2.zero;
    }


    protected override void Chasing() 
    {
      
    }

    private void Move()
    {
        if (!isAtice)
        {
            return;
        }
        rb.velocity = new Vector2(nextMove * moveSpeed, rb.velocity.y);

        Vector2 downVec = new Vector2(rb.position.x + nextMove * 0.3f, rb.position.y);
        RaycastHit2D groundRayHit = Physics2D.Raycast(downVec, Vector3.down, 1f, LayerMask.GetMask("Ground"));

        Vector2 frontVec = new Vector2(rb.position.x + nextMove * 0.3f, rb.position.y);
        RaycastHit2D wallRayHit = Physics2D.Raycast(frontVec, Vector2.right * nextMove, 0.4f, checkLayer);

        float curDistance = transform.position.x - startPosition.x;
        bool isTurn = false;

        if (groundRayHit.collider == null || wallRayHit.collider != null)
        {
            isTurn = true;
        }

        if (Mathf.Abs(curDistance) >= maxMoveDistance)
        {
            if (nextMove > 0 && curDistance > 0)
            {
                isTurn = true;
            }
            else if (nextMove < 0 && curDistance < 0)
            {
                isTurn = true;
            }
        }

        if (isTurn)
        {
            nextMove *= -1;
            if (nextMove > 0)
            {
                sp.flipX = true;
            }
            else
            {
                sp.flipX = false;
            }
        }

    }
}