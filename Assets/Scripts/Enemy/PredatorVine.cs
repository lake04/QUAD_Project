using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorVine : EnemyBase
{
    private int nextMove = 1;
    public LayerMask checkLayer;
    [SerializeField] private bool isAtcive = false;


    protected override void Update()
    {
        base.Update();
        Move();
    }

    protected override void HandlePlayerDetected()
    {
        isAtcive = true;
    }
   

    private void Move()
    {
        if (isAtcive == false)
        {
            return;
        }

        Debug.Log("이동중");
        rb.velocity = new Vector2(nextMove, rb.velocity.y);

        //바닥 체크
        Vector2 downVec = new Vector2(rb.position.x + nextMove * 0.3f, rb.position.y);
        Debug.DrawRay(downVec, Vector3.down * 1f, Color.green);
        RaycastHit2D groundRayHit = Physics2D.Raycast(downVec, Vector3.down, 1f, LayerMask.GetMask("Ground"));

        //앞 장애물 체크
        Vector2 frontVec = new Vector2(rb.position.x + nextMove * 0.3f, rb.position.y);
        Debug.DrawRay(frontVec, Vector2.right * nextMove * 0.4f, Color.red);
        RaycastHit2D wallRayHit = Physics2D.Raycast(frontVec, Vector2.right * nextMove * 0.4f, 1f, checkLayer);


        if (groundRayHit.collider == null || wallRayHit.collider != null)
        {
            nextMove *= -1;
        }
    }
}
