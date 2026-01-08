using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    [SerializeField] private GameObject setting;
    [SerializeField] private GameObject popUp;

    public GameObject[] playerHeart;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        for (int i = 0; i < playerHeart.Length; i++)
        {
            playerHeart[i].SetActive(false);
        }
        MaxHpUpdate();
    }
  

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PopUp();
        }
    }

    public void PopUp()
    {
        popUp.SetActive(!popUp.activeSelf);
    }

    public void Setting()
    {
        setting.SetActive(!setting.activeSelf);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void MaxHpUpdate()
    {
        for (int i = 0; i < Player.instance.maxHp; i++)
        {
            playerHeart[i].SetActive(true);
        }
    }

    public void PlayerHp()
    {
        int hp = Mathf.FloorToInt(Player.instance.curHp);

        for (int i = 0; i < playerHeart.Length; i++)
        {
            if (!playerHeart[i].gameObject.activeInHierarchy)
            {
                continue;
            }

            if (i < hp)
            {
                playerHeart[i].GetComponent<Image>().color = Color.white;
            }
            else
            {
                playerHeart[i].GetComponent<Image>().color = new Color(0, 0, 0, 0.3f);
            }
        }
    }

}

