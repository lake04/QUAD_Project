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

public enum ESfx
{
    SFX_BUTTON,
    SFX_ENDING,
    SFX_BOTTLE,
    SFX_OPENDOOR,
    SFX_SHOOT
}

[System.Serializable]
public struct SoundEntry
{
    public ESfx type;
    public EventReference fmodEvent;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("ui ¼Ò¸®")]
    public EventReference buttonSound;

    public List<SoundEntry> soundList;

    [SerializeField] EventReference[] bgms;
    private Dictionary<ESfx, EventReference> sfxs = new Dictionary<ESfx, EventReference>();

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
    public void ButtonSound()
    {
        RuntimeManager.CreateInstance(buttonSound).start();
    }

    public void PlaySFX(ESfx esfx)
    {
        if (sfxs.TryGetValue(esfx, out EventReference fmodEvent))
        {
            RuntimeManager.CreateInstance(fmodEvent).start();
        }
    }

    public void PlayBGM(ESfx esfx)
    {
        RuntimeManager.CreateInstance(bgms[(int)esfx]).start();
    }

}
