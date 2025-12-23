using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

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

}

