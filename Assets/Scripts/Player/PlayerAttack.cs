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

    [SerializeField] private float preAttackDelay = 0.2f;
    [SerializeField] private float afterAttackDelay = 0.4f;

    [SerializeField] private Transform leftAttackTransform, rightAttackTransform, upAttackTransform, downAttackTransform;
    [SerializeField] private Vector2 leftAttackArea, rightAttackArea, upAttackArea, downAttackArea;

    [SerializeField] private LayerMask attackableLayer;

    // Attack 메서드를 단일 코루틴으로 변경 (지상/수영 공격 모두 사용)
    private IEnumerator Attack()
    {
        isAttacking = true;
        LookAtMouse();
        anim.SetTrigger("Attacking");

        yield return new WaitForSeconds(preAttackDelay);

        bool isFacingLeft = !isFacingRight;
        Transform currentAttackTransform = isFacingLeft ? leftAttackTransform : rightAttackTransform;
        Vector2 currentAttackArea = isFacingLeft ? leftAttackArea : rightAttackArea;

        Hit(currentAttackTransform, currentAttackArea);

        yield return new WaitForSeconds(afterAttackDelay);

        isAttacking = false;
        isAttack = false;
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
        Gizmos.DrawSphere(groundCheck.position, 0.2f);
        Gizmos.DrawSphere(wallCheck.position, 0.2f);

    }
}