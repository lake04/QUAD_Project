using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    [Header("Attack")]
    private bool isAttack = false;
    public bool  isAttacking = false;
    private float timeBetweenAttack = 1f;
    public float damage;
    [SerializeField] private GameObject slashEffect;

    [SerializeField] private Transform  leftAttackTransform, rightAttackTransform,upAttackTransform, downAttackTransform;
    [SerializeField] private Vector2    leftAttackArea, rightAttackArea, upAttackArea, downAttackArea;

    [SerializeField] private LayerMask attackableLayer;

    [SerializeField] private Transform effectPos1, effectPos2;


    private IEnumerator Attack()
    {
        isAttacking = true;
        anim.SetTrigger("Attacking");
        if (sprite.flipX)
        {
            StartCoroutine(SlashEffectAtAngle(slashEffect, 0, effectPos1));
            Hit(rightAttackTransform, rightAttackArea);
        }
        else
        {
            StartCoroutine(SlashEffectAtAngle(slashEffect, 0, effectPos2));
            Hit(leftAttackTransform, leftAttackArea);
        }


        //if (yAxis == 0 || yAxis < 0 && isGrounded)
        //{
        //    Hit(leftAttackTransform, leftAttackArea);
        //    SlashEffectAtAngle(slashEffect, 180, leftAttackTransform);
        //}
        //else if (yAxis > 0)
        //{
        //    Hit(upAttackTransform, upAttackArea);
        //    SlashEffectAtAngle(slashEffect, 0, upAttackTransform);
        //}
        //else if (yAxis < 0 && isGrounded == false)
        //{
        //    Hit(downAttackTransform, downAttackArea);
        //    SlashEffectAtAngle(slashEffect, 0, downAttackTransform);
        //}

        yield return new WaitForSeconds(timeBetweenAttack);
        isAttacking = false;
    }

    private void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        if(objectsToHit.Length > 0 )
        {
            Debug.Log("Hit");
        }
        for(int i=0;i<objectsToHit.Length ;i++)
        {
            if (objectsToHit[i].GetComponent<EnemyBase>() != null)
            {
                Vector2 dir = (transform.position - objectsToHit[i].transform.position).normalized;
                objectsToHit[i].GetComponent<EnemyBase>().TakeDamage(damage, dir,10);
            }
        }
    }

    private IEnumerator SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle,Transform _attackTransform)
    {
        yield return new WaitForSeconds(0.6f);
        CameraShake.Instance.Shake(0.2f, 0.4f);

        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        if(sprite.flipX)
        {
            _slashEffect.GetComponent<SpriteRenderer>().flipX = true;

        }
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x,transform.localScale.y);
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
