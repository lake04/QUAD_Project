using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("플레이어 소리")]
    public EventReference testSound;
    void Awake()
    {
        if (instance == null) instance = this;
    }
    public void TestSound()
    {
        RuntimeManager.CreateInstance(testSound).start();
    }
}
