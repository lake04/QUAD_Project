using System.Collections;
using UnityEngine;

public enum SunkenWarriorStat
{
    Idle,
    Move,
    Attack,
    Patrolling,
    Die
}

public enum BossPhase
{
    Phase1,
    Phase2,
}

public class SunkenWarrior : MonoBehaviour
{
    public SunkenWarriorStat stat;
    public BossPhase phase = BossPhase.Phase1;

    [SerializeField] private float curHp;
    [SerializeField] private float maxHp;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float moveRange = 8.0f; 

    [Header("Attack")]
    [SerializeField] private float dashSpeed = 15.0f; 
    [SerializeField] private float attackRange = 3.0f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float skill1Cooldown = 2.0f; 
    [SerializeField] private float attackCooldown = 2.0f; 
    [SerializeField] private float dashDuration = 0.3f;

    private int lastPhasePatternIndex = -1;

    private bool isAttack = true;

    [Header("피격 처리")]
    [SerializeField] private float recoilLength;
    [SerializeField] private float recoilFactor;
    [SerializeField] private bool isRecoiling = false;

    private bool isDead = false;
    private SpriteRenderer sp;

    private Rigidbody2D rb;
    private GameObject playerTarget;
    private Animator anim;

    [Header("Prefab")]
    [SerializeField] private GameObject harpoonPrefab;
    [SerializeField] private Transform harpoonSpawnPos;

    [SerializeField] private GameObject waterDrillPrefab;
    [SerializeField] private Transform waterDrillSpawnPos1;
    [SerializeField] private Transform waterDrillSpawnPos2;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTarget = GameManager.Instance.player;
        ChangeState(SunkenWarriorStat.Idle);

        StartCoroutine(PhaseController());

        curHp = maxHp;
    }

    void Update()
    {
        if (playerTarget == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.transform.position);

        if (distanceToPlayer < moveRange && stat != SunkenWarriorStat.Attack)
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

    private void UpdatePhase()
    {
        if (curHp <= maxHp * 0.5f && phase == BossPhase.Phase1)
        {
            phase = BossPhase.Phase2;
        }
    }

    private IEnumerator PhaseController()
    {
        yield return new WaitForSeconds(1.0f);

        while (stat != SunkenWarriorStat.Die)
        {
            if (stat == SunkenWarriorStat.Attack)
            {
                yield return null;
                continue;
            }

            if (phase == BossPhase.Phase1)
            {
                yield return StartCoroutine(Phase1Routine());
            }
            else
            {
                yield return StartCoroutine(Phase2Routine());
            }

        }
    }


    private IEnumerator Phase1Routine()
    {
        ChangeState(SunkenWarriorStat.Attack);
        int patternCount = 3;
        int random;
        random = Random.Range(0, patternCount - 1);

        if (random >= lastPhasePatternIndex)
        {
            random++;
        }
        lastPhasePatternIndex = random;

        isAttack = false;
        switch (random)
        {
            case 0:
                StartCoroutine(SkillBasicStab());
                break;

            case 1:
                StartCoroutine(Skill1AttackDash());
                break;

            case 2:
                StartCoroutine(SkillHarpoon());
                break;
        }
        yield return new WaitForSeconds(attackCooldown);
        isAttack = true;
    }

    #region 패턴1
    private IEnumerator SkillBasicStab()
    {
        rb.velocity = Vector2.zero;


        //TODO : 애니메이션 실행

        yield return new WaitForSeconds(0.5f);

        ChangeState(SunkenWarriorStat.Move);
    }

    private IEnumerator Skill1AttackDash()
    {
        Vector2 direction = (playerTarget.transform.position - transform.position).normalized;
        rb.velocity = direction * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector2.zero;
        ChangeState(SunkenWarriorStat.Move);
    }

    private IEnumerator SkillHarpoon()
    {
        Vector2 direction = (playerTarget.transform.position - transform.position).normalized;
        rb.velocity = Vector2.zero;

        //TODO : 작살 던지는 애니메이션 실행
        GameObject harpoonCIone = Instantiate(harpoonPrefab, harpoonSpawnPos.position, Quaternion.identity);
        harpoonCIone.GetComponent<Harpoon>().Init(direction, harpoonSpawnPos);
        //TODO : 작살 회수하는 애니메이션 실행

        yield return new WaitForSeconds(0.7f);
        Destroy(harpoonCIone);
        ChangeState(SunkenWarriorStat.Move);
    }
    #endregion


    #region 패턴 2
    private IEnumerator Phase2Routine()
    {
        int patternCount = 3;
        int random;

        random = Random.Range(0, patternCount - 1);

        if (random >= lastPhasePatternIndex)
        {
            random++;
        }
        lastPhasePatternIndex = random;

        isAttack = false;

        switch (random)
        {
            case 0:
                StartCoroutine(Skill2BasicStab());
                break;

            case 1:
                StartCoroutine(SkillHarpoonAndDash());
                break;

            case 2:
                StartCoroutine(Skill2WaterDrill());
                break;

            case 3:

                break;
        }
        yield return new WaitForSeconds(attackCooldown);
        isAttack = true;
    }

    private IEnumerator Skill2BasicStab()
    {
        rb.velocity = Vector2.zero;


        //TODO : 애니메이션 실행

        yield return new WaitForSeconds(0.5f);

        ChangeState(SunkenWarriorStat.Move);
    }

    private IEnumerator Skill2WaterDrill()
    {
        Vector2 direction = (playerTarget.transform.position - transform.position).normalized;
        rb.velocity = Vector2.zero;

        //TODO : 작살 던지는 애니메이션 실행
        GameObject waterDrillCIone = Instantiate(waterDrillPrefab, waterDrillSpawnPos1.position, Quaternion.identity);
        GameObject waterDrillCIone2 = Instantiate(waterDrillPrefab, waterDrillSpawnPos2.position, Quaternion.identity);
        waterDrillCIone.GetComponent<WaterDrill>().Init(direction);
        waterDrillCIone2.GetComponent<WaterDrill>().Init(direction);
        //TODO : 작살 회수하는 애니메이션 실행

        yield return new WaitForSeconds(0.7f);
     
        ChangeState(SunkenWarriorStat.Move);
    }

    private IEnumerator SkillHarpoonAndDash()
    {
        Vector2 direction = (playerTarget.transform.position - transform.position).normalized;
        rb.velocity = Vector2.zero;

        //TODO : 작살 던지는 애니메이션 실행
        GameObject harpoonCIone = Instantiate(harpoonPrefab, harpoonSpawnPos.position,Quaternion.identity);
        harpoonCIone.GetComponent<Harpoon>().Init(direction, harpoonSpawnPos);
        //TODO : 작살 회수하는 애니메이션 실행

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(DashToTarget(harpoonCIone.transform.position, 0.3f));
        Destroy(harpoonCIone);
        ChangeState(SunkenWarriorStat.Move);
    }

    private IEnumerator DashToTarget(Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;

        while (elapsedTime < duration)
        {
          
            transform.position = Vector3.Lerp(startPos, targetPosition, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null; 
        }
        transform.position = targetPosition;
    }

    #endregion

    protected void ChangeState(SunkenWarriorStat _newState)
    {
        if (_newState != SunkenWarriorStat.Attack || stat != SunkenWarriorStat.Attack)
        {
            stat = _newState;
        }
    }

    public  void TakeDamage(float _damage, Vector2 _hitDirecticon, float _hitForce)
    {
        if (isDead) return;
        Debug.Log("공격 받음");
        curHp -= _damage;

        StartCoroutine(FlashColorOnHit());
        StartCoroutine(Recoiling(_hitDirecticon, _hitForce));
        UpdatePhase();
        if (curHp <= 0)
        {
            Die();
        }
    }

    private IEnumerator Recoiling(Vector2 _hitDirecticon, float _hitForce)
    {
        isRecoiling = true;
        rb.AddForce(_hitDirecticon * _hitForce * _hitDirecticon);
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

        Destroy(gameObject, 1.5f);

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
            ChangeState(SunkenWarriorStat.Patrolling);
        }
    }
}