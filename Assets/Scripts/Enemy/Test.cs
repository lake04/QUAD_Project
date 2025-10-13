using System.Collections;
using UnityEngine;

public class Test : EnemyBase
{
    [Header("Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float waveAmplitude = 0.5f; // 물결 높이
    [SerializeField] private float waveFrequency = 2f;   // 물결 주기
    [SerializeField] private float xRange = 3f;
    [SerializeField] private float descentSpeed = 6f;
    [SerializeField] private float escapeSpeed = 8f;

    private int nextMove = 1;
    private bool isDescending = false;
    private bool isEscaping = false;
    private float waveTimer = 0f;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        if (!isDescending && !isEscaping)
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        // 좌우 이동 + 상하 파동
        waveTimer += Time.deltaTime * waveFrequency;
        float yOffset = Mathf.Sin(waveTimer) * waveAmplitude;

        transform.position += new Vector3(nextMove * patrolSpeed * Time.deltaTime, yOffset * Time.deltaTime, 0);

        // 좌우 경계
        if (transform.position.x <= startPos.x - xRange)
            nextMove = 1;
        else if (transform.position.x >= startPos.x + xRange)
            nextMove = -1;

        // 플레이어 감지
        if (playerTarget == null)
            playerTarget = GameManager.Instance.player;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.transform.position);
        if (distanceToPlayer < detectionRange)
        {
            StartCoroutine(Descent());
        }
    }

    private IEnumerator Descent()
    {
        isDescending = true;
        Vector3 start = transform.position;
        Vector3 target = playerTarget.transform.position;

        float t = 0f;
        float descentDuration = 1.2f; // 낙하 시간

        while (t < 1f)
        {
            t += Time.deltaTime / descentDuration;

            // 포물선 이동 (부드러운 급강하)
            Vector3 mid = (start + target) / 2f + Vector3.down * 2f;
            Vector3 m1 = Vector3.Lerp(start, mid, t);
            Vector3 m2 = Vector3.Lerp(mid, target, t);
            transform.position = Vector3.Lerp(m1, m2, t);

            // 플레이어 방향 회전
            Vector3 dir = (target - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), 0.2f);

            yield return null;
        }

        yield return new WaitForSeconds(0.3f);
        StartCoroutine(Escape());
    }

    private IEnumerator Escape()
    {
        isDescending = false;
        isEscaping = true;

        Vector2 escapeDir = (Vector2.up + Random.insideUnitCircle * 0.5f).normalized;
        Vector2 escapeTarget = (Vector2)transform.position + escapeDir * 5f;

        while (Vector2.Distance(transform.position, escapeTarget) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                escapeTarget,
                escapeSpeed * Time.deltaTime
            );
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDescending && collision.CompareTag("Player"))
        {
            Debug.Log("자원 훔침!");
            StartCoroutine(Escape());
        }
    }
}
