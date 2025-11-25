using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpirit : SpiritBase
{
    [SerializeField] private GameObject fireBall;
    [SerializeField] private float manaCostSkill1 = 10f;

    [SerializeField] private GameObject spawnPos;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public override void Skill1()
    {
        if(curLevel<1)
        {
            return;
        }
        Vector2 fireDirection = Vector2.zero;

        float playerScaleX = GameManager.Instance.player.GetComponent<Player>().transform.localScale.x;

        GameObject fire = Instantiate(fireBall, spawnPos.transform.position, Quaternion.identity);

        if (playerScaleX > 0)
        {
            fireDirection = Vector2.right;
        }
        else 
        {
            fireDirection = Vector2.left;
        }

        fire.GetComponent<FireBall>().Init(fireDirection);
    }

    public override void Skill2()
    {

    }
}
