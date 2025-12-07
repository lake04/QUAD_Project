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
        LookAtMouse(); 
        anim.SetTrigger("Attacking");

        bool isFacingLeft = !isFacingRight;

        // 공격 방향에 따른 변수 설정: 왼쪽을 바라보면 leftAttackTransform 사용
        Transform currentAttackTransform = isFacingLeft ? leftAttackTransform : rightAttackTransform;
        Vector2 currentAttackArea = isFacingLeft ? leftAttackArea : rightAttackArea;
        Transform currentEffectPos = isFacingLeft ? effectPos1 : effectPos2;

        yield return new WaitForSeconds(0.2f);

        Hit(currentAttackTransform, currentAttackArea);

        isAttacking = false;

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
                AttackShake();
            }
        }
    }

    public void AttackShake()
    {
        CameraShake.Instance.AttacShake(0.3f, 0.15f, 0.47f);
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