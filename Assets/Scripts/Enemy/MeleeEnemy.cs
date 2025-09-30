using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyBase
{


    protected override void HandlePlayerDetected()
    {
        Move();
    }

    private void Move()
    {
        Debug.Log("¿Ãµø¡ﬂ");
        direction.Normalize();
        direction.y = 0f;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
    }
}
