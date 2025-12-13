using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform respawnPoint;
    public Transform[] canoePoss;

    public GameObject player;
    public Transform bossRoom;
    public CinemachineVirtualCamera boosRoom;

    [SerializeField] private GameObject forestBackgroundObject;

    private void Awake()
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


    void Start()
    {
        RegionTrigger.currentActiveMap = forestBackgroundObject;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F12))
        {
            BoossRoomMove();
        }
    }

    private void BoossRoomMove()
    {
        player.transform.position = bossRoom.position;
        //CameraManager.instance.curCamera = boosRoom;
    }
}
