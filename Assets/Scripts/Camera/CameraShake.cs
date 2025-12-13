using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    public Vector3 shakeOffset = Vector3.zero;
    private Coroutine currentShakeCoroutine;

    // [추가] 쉐이크 효과를 적용할 대상 (카메라가 따라다니는 대상)
    [SerializeField] private Transform targetToShake;
    private Vector3 initialLocalPos;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 만약 targetToShake를 연결 안 했으면, 이 스크립트가 붙은 객체를 흔듦
        if (targetToShake == null)
        {
            targetToShake = transform;
        }
        initialLocalPos = targetToShake.localPosition;
    }

    // [추가] 계산된 shakeOffset을 실제로 오브젝트에 적용하는 부분
    private void LateUpdate()
    {
        if (targetToShake != null)
        {
            // 원래 위치 + 흔들림 값 적용
            // 주의: Cinemachine을 쓴다면 카메라는 이 오브젝트를 따라다녀야 함
            targetToShake.localPosition = initialLocalPos + shakeOffset;
        }
    }

    public void Shake(float intensity, float time)
    {
        if (currentShakeCoroutine != null) StopCoroutine(currentShakeCoroutine);
        currentShakeCoroutine = StartCoroutine(DoShake(intensity, time));
    }

    public void AttackShake(float intensity, float time)
    {
        if (currentShakeCoroutine != null) StopCoroutine(currentShakeCoroutine);
        currentShakeCoroutine = StartCoroutine(DoAttackShake(intensity, time));
    }

    private IEnumerator DoShake(float intensity, float time)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * intensity,
                Random.Range(-1f, 1f) * intensity,
                0f
            );

            // [수정] 히트스탑 중에도 흔들리게 하려면 unscaledDeltaTime 사용
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        shakeOffset = Vector3.zero;
        targetToShake.localPosition = initialLocalPos; // 위치 복구
    }

    private IEnumerator DoAttackShake(float intensity, float time)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * intensity,
                Random.Range(0f, 1f) * intensity, // 위로 튀는 연출
                0f
            );

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        shakeOffset = Vector3.zero;
        targetToShake.localPosition = initialLocalPos; // 위치 복구
    }
}