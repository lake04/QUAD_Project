using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("패럴랙스 설정")]
    [Range(-1f, 1f)]
    public float parallaxFactor; // 1: 원경(느림), 0: 정지, 음수: 전경(빠름)

    [Tooltip("체크하면 위아래(Y축)로는 움직이지 않음")]
    public bool lockYAxis = false; // Y축 고정 옵션 (여기 체크하면 위아래 무시)

    [Header("무한 스크롤 옵션")]
    public bool useInfiniteScroll = true; // 체크 해제 시 위치 이동만 하고 반복 안 함
    public float backgroundSize;
    public float viewZone = 15;

    private Transform cameraTransform;
    private Transform[] layers;
    private int leftIndex;
    private int rightIndex;
    private float lastCameraX;
    private float lastCameraY;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        layers = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            layers[i] = transform.GetChild(i);
        }

        if (layers.Length > 0)
        {
            leftIndex = 0;
            rightIndex = layers.Length - 1;
        }

        if (cameraTransform != null)
        {
            lastCameraX = cameraTransform.position.x;
            lastCameraY = cameraTransform.position.y;
        }
    }

    private void OnEnable()
    {
        if (cameraTransform != null)
        {
            lastCameraX = cameraTransform.position.x;
            lastCameraY = cameraTransform.position.y;
        }
    }

    private void LateUpdate()
    {
        if (cameraTransform == null) return;

        float deltaX = cameraTransform.position.x - lastCameraX;
        float deltaY = cameraTransform.position.y - lastCameraY;

        // Y축 고정 로직
        // lockYAxis가 켜져 있으면 Y축 변화량(deltaY)을 0으로 만들어버림
        if (lockYAxis)
        {
            deltaY = 0f;
        }

        // 이동 적용
        transform.position += new Vector3(deltaX * parallaxFactor, deltaY * parallaxFactor, 0);

        lastCameraX = cameraTransform.position.x;
        lastCameraY = cameraTransform.position.y;

        // 무한 스크롤 (X축 기준)
        if (useInfiniteScroll && layers.Length > 0)
        {
            if (cameraTransform.position.x < (layers[leftIndex].position.x + viewZone))
                ScrollLeft();

            if (cameraTransform.position.x > (layers[rightIndex].position.x - viewZone))
                ScrollRight();
        }
    }

    private void ScrollLeft()
    {
        Vector3 newPos = layers[rightIndex].position;
        // Y축은 그대로 유지하고 X축만 이동
        newPos.x = layers[leftIndex].position.x - backgroundSize;

        layers[rightIndex].position = newPos;

        leftIndex = rightIndex;
        rightIndex--;
        if (rightIndex < 0) rightIndex = layers.Length - 1;
    }

    private void ScrollRight()
    {
        Vector3 newPos = layers[leftIndex].position;
        newPos.x = layers[rightIndex].position.x + backgroundSize;

        layers[leftIndex].position = newPos;

        rightIndex = leftIndex;
        leftIndex++;
        if (leftIndex == layers.Length) leftIndex = 0;
    }
}