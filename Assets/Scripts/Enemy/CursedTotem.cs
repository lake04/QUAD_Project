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

  
    /// <summary>
    /// 플레이어가 감지되었을 때 행동
    /// </summary>
    protected override void HandlePlayerDetected()
    {
        if(isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    /// <summary>
    /// 플레이어가 없을 때의 행동
    /// </summary>
    protected override void HandlePatrolling()
    {

    }

    private IEnumerator Attack()
    {
        isAttack = false;
        
        GameObject rootClone = Instantiate(rootPrefab, playerTarget.transform.position, Quaternion.identity);
        
        yield return new WaitForSeconds(2f);
        
        isAttack = true;
    }
}


