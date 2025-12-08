using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("적 능력치")]
    [SerializeField] protected float maxHealth = 3; 
    [SerializeField] protected float curHealth; 
    [SerializeField] protected float moveSpeed = 2f; 
    [SerializeField] protected int attackDamage = 1;
    [SerializeField] protected float detectionRange = 5f;
    [SerializeField] protected float attackRange = 1f;
    [SerializeField] protected float attackCooldown;
    protected Vector2 direction;
    protected bool isDead = false;
    protected bool isAttack = true;

    protected GameObject playerTarget; 

    protected Rigidbody2D rb;
    protected Animator anim;

    [SerializeField] protected EnemyState enemyState;

    [Header("피격 처리")]
    [SerializeField] private float recoilLength;
    [SerializeField] private float recoilFactor;
    [SerializeField] private bool isRecoiling = false;

    [SerializeField] protected SpriteRenderer sp;

    protected virtual void Awake()
    {
        Init();
    }


    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {
        if (isDead) return;

        HandleState();

    }

    protected void HandleState()
    {
        if (playerTarget == null && GameManager.Instance != null)
        {
            playerTarget = GameManager.Instance.player;
        }

        if (playerTarget == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.transform.position);

        if (enemyState != EnemyState.Hurt && enemyState != EnemyState.Die || enemyState != EnemyState.Attacking)
        {
            if (distanceToPlayer <= attackRange)
            {
                ChangeState(EnemyState.Attack);
            }
            else if (distanceToPlayer <= detectionRange)
            {
                ChangeState(EnemyState.Chasing);

            }
            else
            {
                ChangeState(EnemyState.Patrolling);
            }
        }
        else if (enemyState == EnemyState.Hurt || enemyState == EnemyState.Die || enemyState == EnemyState.Attacking)
        {
            rb.velocity = Vector2.zero;
        }

        switch (enemyState)
        {
            case EnemyState.Attacking:
                break;
            case EnemyState.Patrolling:
                Patrolling();
                break;

            case EnemyState.Chasing:
                Chasing();
                break;

            case EnemyState.Attack:
                if (isAttack)
                {
                    Attack();
                }
                break;

            case EnemyState.Die:
                Die();
                break;
        }
    }

    public void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();

        if (GameManager.Instance != null)
        {
            playerTarget = GameManager.Instance.player;
        }

        curHealth = maxHealth;
    }


    /// <summary>
    /// 플레이어가 감지되었을 때 행동
    /// </summary>
    protected virtual void Chasing()
    {

    }

    /// <summary>
    /// 플레이어가 없을 때의 행동
    /// </summary>
    protected virtual void Patrolling()
    {

    }

    protected virtual void Attack()
    {

    }


    public virtual void TakeDamage(float _damage,Vector2 _hitDirecticon,float _hitForce)
    {
        if (isDead) return;

        curHealth -= _damage;
  
        StartCoroutine(FlashColorOnHit());
        StartCoroutine(Recoiling(_hitDirecticon,_hitForce));
        if (curHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator Recoiling(Vector2 _hitDirecticon, float _hitForce)
    {
        isRecoiling = true;
        rb.AddForce(_hitDirecticon * recoilFactor * _hitDirecticon);
        yield return new WaitForSeconds(recoilLength);
        isRecoiling = false;
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true; 
        }

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        Destroy(gameObject);

        DropItems();
    }

    protected virtual void DropItems()
    {
       
    }

    protected void ChangeState(EnemyState _newState)
    {
        if (enemyState == _newState) return; 


        enemyState = _newState;
    }

    protected virtual IEnumerator FlashColorOnHit()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color originalColor = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = originalColor;
        }

        if (!isDead)
        {
            ChangeState(EnemyState.Patrolling);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Player player = collision.gameObject.GetComponent<Player>();
            //if (player != null)
            //{
            //    player.TakeDamage(attackDamage);
            //}
        }
    }


}
