using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField] protected float damage;
    public Rigidbody2D rb;
    [SerializeField] protected float speed;

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
}
