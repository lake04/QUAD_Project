using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedTotem : EnemyBase
{
    [SerializeField]
    private GameObject rootPrefab;

    void Start()
    {
        
    }

  

    protected override void Chasing()
    {
        if (isAttack)
        {
            StartCoroutine(IEAttack());
        }
    }

 
    protected override void Patrolling()
    {
       
    }

   IEnumerator IEAttack()
    {
        isAttack = false;
        
        GameObject rootClone = Instantiate(rootPrefab, playerTarget.transform.position, Quaternion.identity);
        
        yield return new WaitForSeconds(2f);
        
        isAttack = true;
    }
}


