using FMODUnity;
using UnityEngine;

public class CharacterSFXController : MonoBehaviour
{
    [Header("Looping SFX Emitters")]
    public StudioEventEmitter walkEventEmitter;
    public StudioEventEmitter swimEventEmitter;

    private bool isMoving = false;
    private bool isSwimming = false;

    public void SetMovementState(bool moving, bool swimming)
    {
        isMoving = moving;
        isSwimming = swimming;

        UpdateLoopingSFX();
    }

    private void UpdateLoopingSFX()
    {
        if (isMoving && !isSwimming)
        {
            if (!walkEventEmitter.IsPlaying())
            {
                walkEventEmitter.Play();
            }
        }
        else
        {
            // 걷고 있지 않거나, 수영 중이라면 걷기 사운드 정지
            if (walkEventEmitter.IsPlaying())
            {
                walkEventEmitter.Stop();
            }
        }

        if (isSwimming)
        {
            if (!swimEventEmitter.IsPlaying())
            {
                swimEventEmitter.Play();
            }
        }
        else
        {
            if (swimEventEmitter.IsPlaying())
            {
                swimEventEmitter.Stop();
            }
        }
    }

    private void OnDestroy()
    {
        if (walkEventEmitter != null) walkEventEmitter.Stop();
        if (swimEventEmitter != null) swimEventEmitter.Stop();
    }
}