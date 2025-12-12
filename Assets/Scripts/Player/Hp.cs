using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hp : MonoBehaviour
{
    [SerializeField] private int heal;
    [SerializeField] private GameObject desroyEffect;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Player.instance.Heal(heal);
            Instantiate(desroyEffect, transform);
            Destroy(gameObject);
        }
    }
}
