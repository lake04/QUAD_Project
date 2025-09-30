using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canoe : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
     
    }

    private void IslandMove(int _targetIndex)
    {
        if(GameManager.Instance !=null)
        {
            Debug.Log("카누 이동");
            GameManager.Instance.player.transform.position = GameManager.Instance.canoePoss[_targetIndex].position;
        }   
    }
}
