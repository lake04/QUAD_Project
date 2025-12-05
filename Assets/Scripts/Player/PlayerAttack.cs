using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    [Header("Attack")]
    private bool isAttack = false;
    public bool isAttacking = false;
    private float timeBetweenAttack = 1f;
    public float damage;
    [SerializeField] private GameObject slashEffect;

    [SerializeField] private Transform leftAttackTransform, rightAttackTransform, upAttackTransform, downAttackTransform;
    [SerializeField] private Vector2 leftAttackArea, rightAttackArea, upAttackArea, downAttackArea;

    [SerializeField] private LayerMask attackableLayer;

    [SerializeField] private Transform effectPos1, effectPos2;

    // Attack 메서드를 단일 코루틴으로 변경 (지상/수영 공격 모두 사용)
    private IEnumerator Attack()
    {
        isAttacking = true;
        anim.SetTrigger("Attacking");
        // localScale.x가 양수(>0)면 오른쪽, 음수(<0)면 왼쪽을 바라봄
        bool isFacingLeft = transform.localScale.x < 0;

        // 공격 방향에 따른 변수 설정: 왼쪽을 바라보면 leftAttackTransform 사용
        Transform currentAttackTransform = isFacingLeft ? leftAttackTransform : rightAttackTransform;
        Vector2 currentAttackArea = isFacingLeft ? leftAttackArea : rightAttackArea;
        Transform currentEffectPos = isFacingLeft ? effectPos1 : effectPos2;

        // 1. 히트 판정 시점까지 대기 
        yield return new WaitForSeconds(0.2f);

        // 2. 히트 판정 실행
        Hit(currentAttackTransform, currentAttackArea);

        // 3. 이펙트 및 카메라 쉐이크 시점까지 대기 (예: 0.1초)
        yield return new WaitForSeconds(0.1f);
        AttackShake();
        // 5. 공격 종료 애니메이션 시간 대기
        yield return new WaitForSeconds(timeBetweenAttack - 0.3f);

        // 6. 공격 종료 및 상태 복귀
        isAttacking = false;
        anim.SetBool("Move",true);
        // Attack 코루틴 종료 후, FSM은 적절한 상태로 복귀
        if (isSwimming)
        {
            ChangeState(PlayerState.Swim);
        }
        else if (isGrounded)
        {
            ChangeState(xAxis != 0 ? PlayerState.Run : PlayerState.Idle);
        }
        else
        {
            ChangeState(PlayerState.Jump);
        }
    }

    // Hit 메서드 (변화 없음)
    private void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        if (objectsToHit.Length > 0)
        {
            Debug.Log("Hit");
        }
        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<EnemyBase>() != null)
            {
                Vector2 dir = (transform.position - objectsToHit[i].transform.position).normalized;
                objectsToHit[i].GetComponent<EnemyBase>().TakeDamage(damage, dir, 10);
            }
            else if (objectsToHit[i].GetComponent<SunkenWarrior>() != null)
            {
                Vector2 dir = (transform.position - objectsToHit[i].transform.position).normalized;
                objectsToHit[i].GetComponent<SunkenWarrior>().TakeDamage(damage, dir, 0);
            }
        }
    }

    public void AttackShake()
    {
        CameraShake.Instance.AttacShake(0.3f, 0.3f, 1.2f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(leftAttackTransform.position, leftAttackArea);
        Gizmos.DrawWireCube(rightAttackTransform.position, rightAttackArea);
        //Gizmos.DrawWireCube(upAttackTransform.position, upAttackArea);
        //Gizmos.DrawWireCube(downAttackTransform.position, downAttackArea);
    }
}