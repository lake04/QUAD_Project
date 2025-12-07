using FMOD;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBgm
{
    BGM_TITLE,
    BGM_GAME,
}

public enum SoundType
{
    SFX_BUTTON,
    SFX_ENDING,
    SFX_BOTTLE,
    SFX_OPENDOOR,
    SFX_SHOOT,
    SFX_Attack,
    BOSS_BGM
}

[System.Serializable]
public struct SoundEntry
{
    public SoundType type;
    public EventReference fmodEvent;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("ui ¼Ò¸®")]
    public EventReference buttonSound;
    public EventReference bgm;

    public List<SoundEntry> soundList;

    [SerializeField] EventReference[] sfx;
    private Dictionary<SoundType, EventReference> sfxs = new Dictionary<SoundType, EventReference>();

    void Awake()
    {
        if (instance == null) instance = this;

        foreach (var entry in soundList)
        {
            if (!sfxs.ContainsKey(entry.type))
            {
                sfxs.Add(entry.type, entry.fmodEvent);
            }
        }
    }

    private void Start()
    {
        RuntimeManager.CreateInstance(bgm).start();
    }

    public void ButtonSound()
    {
        RuntimeManager.CreateInstance(buttonSound).start();
    }

    public void PlaySFX(SoundType esfx)
    {
        if (sfxs.TryGetValue(esfx, out EventReference fmodEvent))
        {
            RuntimeManager.CreateInstance(fmodEvent).start();
        }
    }

}
