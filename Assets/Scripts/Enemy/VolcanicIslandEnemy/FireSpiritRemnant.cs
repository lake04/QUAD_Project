using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FireSpiritRemnant : GroundEnemy
{

    protected override void Attack()
    {
        StartCoroutine(Explosion());
    }

    

    private IEnumerator Explosion()
    {
        isAttack = false;
        Debug.Log("ÆøÆÈ!!!!");
        yield return new WaitForSeconds(attackCooldown);
        isAttack = true;
    }
}
