using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionTrigger : MonoBehaviour
{
    [SerializeField] private GameObject text;
    [SerializeField] private Transform canva;

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
           Instantiate(text, canva);
        }
    }
}
