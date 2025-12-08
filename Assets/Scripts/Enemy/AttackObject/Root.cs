using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    [SerializeField] private int damage;

    private void OnEnable()
    {
        Destroy(gameObject, 2f);
    }

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("플레이어 뿌리 공격");
            Player.instance.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
