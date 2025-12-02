using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpiritRemnant : EnemyBase
{

    protected override void Attack()
    {
        StartCoroutine(Explosion());
    }

    private IEnumerator Explosion()
    {
        isAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        isAttack = true;
    }
}
