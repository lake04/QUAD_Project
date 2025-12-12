using UnityEngine;

[ExecuteInEditMode] // 에디터에서 실행되게 하는 핵심
public class ParallaxPreview : MonoBehaviour
{
    [Header("시뮬레이션")]
    public Transform cameraDummy; // 가짜 카메라 (Scene 뷰에 있는 아이콘 등)
    public bool isPreviewOn = false; // 체크하면 미리보기 모드 작동

    // 시뮬레이션 할 배경 그룹들 (ParallaxLayer가 붙은 부모들)
    public ParallaxLayer[] parallaxLayers;

    // 각 레이어의 초기 위치 저장용
    private Vector3[] startPositions;

    private void OnEnable()
    {
        // 현재 씬에 있는 모든 패럴랙스 레이어 찾기
        parallaxLayers = FindObjectsOfType<ParallaxLayer>();

        // 초기 위치 기억
        startPositions = new Vector3[parallaxLayers.Length];
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            startPositions[i] = parallaxLayers[i].transform.position;
        }
    }

    private void Update()
    {
        if (!isPreviewOn || cameraDummy == null) return;

        // 에디터 상의 가짜 카메라 위치
        float currentX = cameraDummy.position.x;
        float currentY = cameraDummy.position.y;

        // 모든 배경 레이어를 강제로 이동시켜봄
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            if (parallaxLayers[i] == null) continue;

            float factor = parallaxLayers[i].parallaxFactor;
            float dist = currentX * factor;

            Vector3 newPos = startPositions[i];
            newPos.x += dist;

            parallaxLayers[i].transform.position = newPos;
        }
    }

    private void OnDisable()
    {
        if (startPositions == null) return;
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            if (parallaxLayers[i] != null)
                parallaxLayers[i].transform.position = startPositions[i];
        }
    }
}