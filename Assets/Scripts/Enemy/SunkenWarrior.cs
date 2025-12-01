using System.Collections;
using UnityEngine;

public enum SunkenWarriorStat
{
    Idle,
    Move,
    Attack, // 돌진(Dash) 공격 상태
    Die
}

public class SunkenWarrior : MonoBehaviour
{
    public SunkenWarriorStat stat;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float moveRange = 8.0f; 

    [Header("Attack")]
    [SerializeField] private float dashSpeed = 15.0f; 
    [SerializeField] private float attackRange = 3.0f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float skill1Cooldown = 2.0f; 
    [SerializeField] private float dashDuration = 0.3f; 

    private bool isAttackReady = true; 

    private Rigidbody2D rb;
    private GameObject playerTarget;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTarget = GameManager.Instance.player;
        ChangeState(SunkenWarriorStat.Idle);
    }

    void Update()
    {
        if (playerTarget == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.transform.position);

        if (distanceToPlayer <= attackRange && isAttackReady)
        {
            StartCoroutine(Skill1_AttackDash()); 
        }
        else if (distanceToPlayer < moveRange && stat != SunkenWarriorStat.Attack)
        {
            ChangeState(SunkenWarriorStat.Move);
        }
        else if (stat != SunkenWarriorStat.Attack)
        {
            ChangeState(SunkenWarriorStat.Idle);
        }
    }

    private void FixedUpdate()
    {
        switch (stat)
        {
            case SunkenWarriorStat.Idle:
                rb.velocity = Vector2.zero;
                break;

            case SunkenWarriorStat.Move:
                Move();
                break;

            case SunkenWarriorStat.Attack:
                break;

        }
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            playerTarget.transform.position,
            moveSpeed * Time.fixedDeltaTime 
        );
    }

    private IEnumerator Skill1_AttackDash()
    {
        isAttackReady = false; 
        ChangeState(SunkenWarriorStat.Attack);

        Vector2 direction = (playerTarget.transform.position - transform.position).normalized;
        rb.velocity = direction * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(skill1Cooldown);

        isAttackReady = true; 
    }

    protected void ChangeState(SunkenWarriorStat _newState)
    {
        if (_newState != SunkenWarriorStat.Attack || stat != SunkenWarriorStat.Attack)
        {
            stat = _newState;
        }
    }
}