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
    private bool isDashAttack = false;
    private bool isCanDashAttack;

    [SerializeField] private float preAttackDelay = 0.2f;
    [SerializeField] private float afterAttackDelay = 0.4f;

    [SerializeField] private Transform leftAttackTransform, rightAttackTransform, upAttackTransform, downAttackTransform;
    [SerializeField] private Vector2 leftAttackArea, rightAttackArea, upAttackArea, downAttackArea;

    [SerializeField] private LayerMask attackableLayer;

    [Header("Combo Settings")]
    private int comboStep = 0;
    private float lastAttackTime;
    [SerializeField] private float comboResetTime = 0.8f;
    private Coroutine comboResetCoroutine; // 콤보 리셋용
    [SerializeField] private GameObject attackHitEffect;

    public void PerformAttack()
    {
        if (comboStep == 2 && anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") || anim.GetCurrentAnimatorStateInfo(0).IsName("JumpAttack"))
        {
            return;
        }

        // 공격할 때마다 기존 리셋 카운트다운을 취소
        if (comboResetCoroutine != null) StopCoroutine(comboResetCoroutine);

        // 콤보 유효 시간 지났으면 리셋
        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboStep = 0;
        }

        lastAttackTime = Time.time;

        if (IsGrounded())
        {
            comboStep++;
            if (comboStep > 2) comboStep = 1;

            anim.SetInteger("Combo", comboStep);
            anim.SetTrigger("Attacking");
            SoundManager.instance.PlaySFX(SoundType.SFX_Attack);
            LookAtMouse();

            StopCoroutine(nameof(CheckAttackHit));
            StartCoroutine(CheckAttackHit(comboStep));

            anim.SetInteger("Combo", comboStep);

        }
        else if(isSwimming)
        {
            comboStep++;
            if (comboStep > 2) comboStep = 1;

            anim.SetInteger("Combo", comboStep);
            anim.SetTrigger("Attacking");


            StopCoroutine(nameof(CheckAttackHit));
            StartCoroutine(CheckAttackHit(comboStep));

            anim.SetInteger("Combo", comboStep);
        }
        else
        {
            // 점프 공격은 콤보 스텝 영향 안 받게
            anim.SetTrigger("Attacking");
            LookAtMouse();

            StopCoroutine(nameof(CheckAttackHit));
            StartCoroutine(CheckAttackHit(0));
        }

        comboResetCoroutine = StartCoroutine(ResetComboTimer());
    }

    private IEnumerator ResetComboTimer()
    {
        yield return new WaitForSeconds(comboResetTime);
        comboStep = 0;
        anim.SetInteger("Combo", 0);
    }

    private IEnumerator CheckAttackHit(int step)
    {
        isAttacking = true;

        float delay = 0.2f;
        if (step == 0) delay = 0.1f;

        yield return new WaitForSeconds(delay);

        Hit(rightAttackTransform, rightAttackArea);

        yield return new WaitForSeconds(0.4f); // 후딜

        isAttacking = false;
    }

    private void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        if (objectsToHit.Length > 0)
        {
            // Debug.Log("Hit");
        }
        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<EnemyBase>() != null)
            {
                Vector2 dir = (transform.position - objectsToHit[i].transform.position).normalized;
                GameObject _hitEffect = Instantiate(attackHitEffect, objectsToHit[i].transform);
                objectsToHit[i].GetComponent<EnemyBase>().TakeDamage(damage, dir, 10);
                AttackShake();
            }

            if (objectsToHit[i].GetComponent<SunkenWarrior>() != null)
            {
                Vector2 dir = (transform.position - objectsToHit[i].transform.position).normalized;
                GameObject _hitEffect = Instantiate(attackHitEffect, objectsToHit[i].transform);
                objectsToHit[i].GetComponent<SunkenWarrior>().TakeDamage(damage, dir, 10);
                AttackShake();
            }
        }
    }

    public void AttackShake()
    {
        CameraShake.Instance.Shake(0.1f, 0.2f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (rightAttackTransform != null) Gizmos.DrawWireCube(rightAttackTransform.position, rightAttackArea);
        if (groundCheck != null) Gizmos.DrawSphere(groundCheck.position, 0.2f);
        if (wallCheck != null) Gizmos.DrawSphere(wallCheck.position, 0.2f);
    }
}