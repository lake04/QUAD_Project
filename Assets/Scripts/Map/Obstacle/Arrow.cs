using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    [SerializeField] private float damage;
    public Rigidbody2D rb;
    [SerializeField] private float speed;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        Invoke("AutoDestroy", 2f);
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void Init(Vector2 _Direction)
    {
        rb.AddForce(_Direction * speed, ForceMode2D.Impulse);
    }

    private void AutoDestroy()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
