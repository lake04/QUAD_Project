using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundEnemy : EnemyBase
{
    private int nextMove = 1;
    public LayerMask checkLayer;
    [SerializeField] private float maxMoveDistance = 5f;
    private Vector3 startPosition;


    private void Start()
    {
        startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        //if (enemyState == EnemyState.Patrolling)
        //{
        //    Move();
        //} 
        Move();
    }

    private void Move()
    {
        rb.velocity = new Vector2(nextMove, rb.velocity.y);

        //바닥 체크
        Vector2 downVec = new Vector2(rb.position.x + nextMove * 0.3f, rb.position.y);
        Debug.DrawRay(downVec, Vector3.down * 1f, Color.green);
        RaycastHit2D groundRayHit = Physics2D.Raycast(downVec, Vector3.down, 1f, LayerMask.GetMask("Ground"));

        //앞 장애물 체크
        Vector2 frontVec = new Vector2(rb.position.x + nextMove * 0.3f, rb.position.y);
        Debug.DrawRay(frontVec, Vector2.right * nextMove * 0.4f, Color.red);
        RaycastHit2D wallRayHit = Physics2D.Raycast(frontVec, Vector2.right * nextMove * 0.4f, 1f, checkLayer);

        float curDistance = transform.position.x - startPosition.x;
        bool isTurn = false;

        if (groundRayHit.collider == null || wallRayHit.collider != null)
        {
            isTurn = true;
        }


        if (Mathf.Abs(curDistance) >= maxMoveDistance)
        {
            if(nextMove > 0 && curDistance >0)
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
        }
    }
}
