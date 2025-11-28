using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    public Vector3 shakeOffset = Vector3.zero;


    private void Start()
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

    private void LateUpdate()
    {
    }

    public void Shake(float duration, float xMagnitude = 1f, float yMagnitude = 1f)
    {
        StartCoroutine(DoShake(duration, xMagnitude, yMagnitude));
    }

    private IEnumerator DoShake(float duration, float xMagnitude =1f, float yMagnitude = 1f)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Debug.Log("쉐이킹중");
            // 흔들림 오프셋을 랜덤하게 계산
            shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * xMagnitude,
                Random.Range(-1f, 1f) * yMagnitude,
                0f
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
    }
}
