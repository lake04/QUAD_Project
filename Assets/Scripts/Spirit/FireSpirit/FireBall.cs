using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : ProjectileBase
{
   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyBase>().TakeDamage(damage,Vector2.zero,0f);
            Destroy(gameObject);
        }
    }
}
