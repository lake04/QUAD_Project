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

        Vector3 pos = playerTarget.transform.position;
        pos.y = -  2f;          
        GameObject rootClone = Instantiate(rootPrefab, pos, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        
        isAttack = true;
    }
}


