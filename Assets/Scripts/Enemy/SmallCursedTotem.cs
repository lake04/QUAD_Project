using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallCursedTotem : GroundEnemy
{
    private void Start()
    {
        startPosition = transform.position;
        sp = GetComponent<SpriteRenderer>();
    }

    protected override void FixedUpdate()
    {
        if (isDead) return;

        HandleState();

    }



    protected override void Chasing()
    {
        PatrollingMove();
    }


    protected override void Patrolling()
    {
        PatrollingMove();
    }

    protected override void Attack()
    {
      
        StartCoroutine(IEAttack());
    }

    IEnumerator IEAttack()
    {
        isAttack = false;
        ChangeState(EnemyState.Attacking);
        rb.velocity = Vector3.zero;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(1.8f);
        ChangeState(EnemyState.Chasing);

        yield return new WaitForSeconds(attackCooldown);
        isAttack = true;
    }
}
