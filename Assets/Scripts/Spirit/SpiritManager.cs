using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritManager : MonoBehaviour
{
    public static SpiritManager Instance;

    private SpiritBase equippedSpirit;

    [SerializeField] private FireSpirit fireSpirit;
    //[SerializeField] private WindSpirit windSpirit;
    //[SerializeField] private WaterSpirit waterSpirit;
    //[SerializeField] private GroundSpirit groundSpirit;

    [SerializeField] private float swapCooldown = 1.0f;
    [SerializeField] private float lastSwapTime = 1.0f;

    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            UseSkill1();
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                UseSkill2();
            }
        }
    }

    public void EquipSpirit(SpiritBase _newSpirit)
    {
        if (Time.deltaTime < lastSwapTime + swapCooldown)
        {
            return;
        }

        equippedSpirit = _newSpirit;
        lastSwapTime = Time.deltaTime;
    }


    private void UseSkill1()
    {
        if (equippedSpirit != null)
        {
            equippedSpirit.Skill1();
        }
    }

    private void UseSkill2()
    {
        if (equippedSpirit != null)
        {
            equippedSpirit.Skill2();
        }
    }

}
