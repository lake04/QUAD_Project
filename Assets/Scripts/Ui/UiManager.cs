using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("PlayerHp")]
    public GameObject[] playerHeart;

    private void Start()
    {
        for (int i = 0; i < playerHeart.Length; i++) //전체 비활성화
        {
            playerHeart[i].SetActive(false);
        }
        MaxHpUpdate();
    }
    public void MaxHpUpdate()
    {
        for (int i = 0; i < Player.instance.maxHp; i++) //maxHp 값만큼 활성화
        {
            playerHeart[i].SetActive(true);
        }
    }

    private void Update()
    {
        PlayerHp();
    }

    void PlayerHp()
    {
        int hp = Player.instance.cuerHp;

        for (int i = 0; i < playerHeart.Length; i++)
        {
            if (!playerHeart[i].gameObject.activeInHierarchy)
                continue;

            if (i < hp)
                playerHeart[i].GetComponent<Image>().color = Color.white;
            else
                playerHeart[i].GetComponent<Image>().color= new Color(0,0,0,0.3f);
        }
    }
}
