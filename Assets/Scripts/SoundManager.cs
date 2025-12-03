using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("ui ¼Ò¸®")]
    public EventReference buttonSound;
    void Awake()
    {
        if (instance == null) instance = this;
    }
    public void ButtonSound()
    {
        RuntimeManager.CreateInstance(buttonSound).start();
    }
}
