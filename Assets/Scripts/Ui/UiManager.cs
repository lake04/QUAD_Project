using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{

    public static UiManager Instance;

    [Header("PlayerHp")]
    public GameObject[] playerHeart;

    [SerializeField] private GameObject setting;
    [SerializeField] private GameObject popUp;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

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
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PopUp();
        }
    }

    public void PlayerHp()
    {
        int hp = Player.instance.curHp;

        for (int i = 0; i < playerHeart.Length; i++)
        {
            if (!playerHeart[i].gameObject.activeInHierarchy)
                continue;

            if (i < hp)
            {
                playerHeart[i].GetComponent<Image>().color = Color.white;
            }
            else
                playerHeart[i].GetComponent<Image>().color= new Color(0,0,0,0.3f);
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
        Application.Quit(); // 어플리케이션 종료
#endif
    }

    public void Title()
    {
        LoadingManager.instance.Loading("Title");
    }
}

