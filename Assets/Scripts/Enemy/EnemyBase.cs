using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyBase : MonoBehaviour
{
    [Header("적 능력치")]
    [SerializeField] protected int maxHealth = 3; 
    [SerializeField] protected float moveSpeed = 2f; 
    [SerializeField] protected int attackDamage = 1;
    [SerializeField] protected float detectionRange = 5f;
    protected Vector2 direction;
    protected int currentHealth;
    protected bool isDead = false;
    protected bool isAttack = true;

    protected GameObject playerTarget; 

    protected Rigidbody2D rb;
    protected Animator anim;

    protected virtual void Awake()
    {
        Init();
    }

    protected virtual void Update()
    {
        if (isDead) return;

        Search();
    }

    public void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (GameManager.Instance != null)
        {
            playerTarget = GameManager.Instance.player;
        }

        currentHealth = maxHealth;
    }

    protected void Search()
    {
        if (playerTarget == null)
        {
            playerTarget = GameManager.Instance.player;

        }
        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.transform.position);
        direction  = (playerTarget.transform.position - transform.position);

        if (distanceToPlayer <= detectionRange)
        {
            HandlePlayerDetected();
        }
        else
        {
            HandlePatrolling();
        }
    }

    /// <summary>
    /// 플레이어가 감지되었을 때 행동
    /// </summary>
    protected virtual void HandlePlayerDetected()
    {

    }

    /// <summary>
    /// 플레이어가 없을 때의 행동
    /// </summary>
    protected virtual void HandlePatrolling()
    {

    }


    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
  
        StartCoroutine(FlashColorOnHit());

        if (currentHealth <= 0)
        {
            Die();
        }
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

        Destroy(gameObject, 1.5f);

        DropItems();
    }

    protected virtual void DropItems()
    {
       
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

    /// <summary>
    /// 피격 시 색상 깜빡임 효과 
    /// </summary>
    IEnumerator FlashColorOnHit()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color originalColor = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f); 
            sr.color = originalColor;
        }
    }
}
