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
        Vector2 fireDirection = Vector2.zero;

        GameObject fire = Instantiate(fireBall,spawnPos.transform.position, Quaternion.identity);
        fire.transform.position = spawnPos.transform.position;

        if (GameManager.Instance.player.GetComponent<Player>().sprite.flipX)
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
