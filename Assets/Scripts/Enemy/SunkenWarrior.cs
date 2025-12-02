using System.Collections;
using UnityEngine;

public enum SunkenWarriorStat
{
    Idle,
    Move,
    Attack, // 돌진(Dash) 공격 상태
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

    [SerializeField] private int curHp;
    [SerializeField] private int maxHp;

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

    private Rigidbody2D rb;
    private GameObject playerTarget;

    [Header("Prefab")]
    [SerializeField] private GameObject harpoonPrefab;
    [SerializeField] private Transform harpoonSpawnPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTarget = GameManager.Instance.player;
        ChangeState(SunkenWarriorStat.Idle);

        StartCoroutine(PhaseController());
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
        GameObject harpoonCIone = Instantiate(harpoonPrefab,harpoonSpawnPos);
        harpoonCIone.GetComponent<Harpoon>().Init(direction);
        yield return new WaitForSeconds(1f);

        //TODO : 작살 회수하는 애니메이션 실행
        harpoonCIone.transform.position = Vector2.MoveTowards(harpoonCIone.transform.position,harpoonSpawnPos.position,2f);


        ChangeState(SunkenWarriorStat.Move);
    }



    #endregion



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

                break;

            case 1:
                break;

            case 2:

                break;

            case 3:

                break;
        }
        yield return new WaitForSeconds(attackCooldown);
        isAttack = true;
    }

   

    protected void ChangeState(SunkenWarriorStat _newState)
    {
        if (_newState != SunkenWarriorStat.Attack || stat != SunkenWarriorStat.Attack)
        {
            stat = _newState;
        }
    }
}