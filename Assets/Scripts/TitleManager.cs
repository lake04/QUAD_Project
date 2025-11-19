using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void Play()
    {
        //ㄱㄱ
        SceneManager.LoadScene("Stage1");
    }

    public void Opition()
    {
        //설정 유아이 팝업
    }

    public void Credit()
    {
        //크레딧
    }

    public void Leave()
    {
        //나가기
    }
}
