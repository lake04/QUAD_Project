using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] private float curHp;
    [SerializeField] private float maxHp;



    void Start()
    {
        
    }

    void Update()
    {
        
    }


    public void TakeDamage(float _damage)
    {
        curHp-=_damage;
        if (curHp <=0)
        {
            Destroy(gameObject);
        }
    }
}
