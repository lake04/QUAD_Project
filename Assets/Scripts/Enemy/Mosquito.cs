using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mosquito : EnemyBase
{
    [Header("Setting")]
    [SerializeField] private float escapeSpeed = 6f;
    [SerializeField] private float descentSpeed = 6f;

    [SerializeField] private float waveAmplitude = 0.5f; // 물결 높이
    [SerializeField] private float waveFrequency = 2f;   // 물결 주기

    [SerializeField] private float xRange;
    private int nextMove = 1;
    private bool isMove = true;

    private bool isDescent = false;
    private bool isEsape = false;

    private float waveTimer = 0f;
    private Vector3 startPos;


    void Start()
    {
        startPos = transform.position;
    }


    protected override void HandlePlayerDetected()
    {
        if (!isDescent || !isEsape)
            StartCoroutine(Descent());
        else
        {
            Move();
        }
            Debug.Log("범위 안");
    }

    protected override void HandlePatrolling()
    {
        Move();
    }

    private void Move()
    {
        if (!isMove) return;

        waveTimer += Time.deltaTime * waveFrequency;
        float yOffset = Mathf.Sin(waveTimer) * waveAmplitude;

        transform.position += new Vector3(nextMove * moveSpeed * Time.deltaTime, yOffset * Time.deltaTime, 0);
        if (transform.position.x <= -xRange)
        {
            nextMove = 1;
        }
        else if (transform.position.x >= xRange)
        {
            nextMove = -1;
        }

    }

    private IEnumerator Descent()
    {
        if (playerTarget == null)
            playerTarget = GameManager.Instance.player;

        isMove = false;
        isDescent = true;
        while (Vector2.Distance(transform.position, playerTarget.transform.position) > 1f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                playerTarget.transform.position,
                descentSpeed * Time.deltaTime
            );

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator Escape()
    {
        isEsape = true;
        Vector2 escapeDir = (Vector2.up + Random.insideUnitCircle * 0.2f).normalized;
        Vector2 escapeTarget = (Vector2)transform.position + escapeDir * 1f;

        while (Vector2.Distance(transform.position, escapeTarget) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                escapeTarget,
                escapeSpeed * Time.deltaTime
            );

            yield return null;
        }

        isMove = true;

        yield return new WaitForSeconds(2f);
        isEsape = false;
        isDescent = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("적 공격");
            if (!isEsape)
            {
                StartCoroutine(Escape());

            }
        }
    }
}
