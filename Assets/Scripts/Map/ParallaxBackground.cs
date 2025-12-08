using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{

    public float backgroundSize;
    [Range(0f, 1f)] public float parallaxSpeed; // 1이면 카메라와 같이 이동(고정된 느낌), 0이면 안 움직임
    public float viewZone = 10;

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

        leftIndex = 0;
        rightIndex = layers.Length - 1;

        // 시작 시 위치 초기화
        ResetParallax();
    }

    // 지역이 바뀌어서 오브젝트가 다시 켜질 때 카메라 위치 갱신
    private void OnEnable()
    {
        if (cameraTransform != null)
        {
            ResetParallax();
        }
    }

    private void ResetParallax()
    {
        lastCameraX = cameraTransform.position.x;
        lastCameraY = cameraTransform.position.y;
    }

    // 카메라 이동 후 배경을 움직이기 위해 LateUpdate 사용
    private void LateUpdate()
    {
        if (cameraTransform == null) return;

        float deltaX = cameraTransform.position.x - lastCameraX;
        float deltaY = cameraTransform.position.y - lastCameraY;

        // 배경 이동 (시차 적용)
        transform.position += Vector3.right * (deltaX * parallaxSpeed);
        transform.position += Vector3.up * (deltaY * parallaxSpeed);

        lastCameraX = cameraTransform.position.x;
        lastCameraY = cameraTransform.position.y;

        // 무한 스크롤 로직
        if (cameraTransform.position.x < (layers[leftIndex].position.x + viewZone))
        {
            ScrollLeft();
        }

        if (cameraTransform.position.x > (layers[rightIndex].position.x - viewZone))
        {
            ScrollRight();
        }
    }

    private void ScrollLeft()
    {
        float currentZ = layers[leftIndex].position.z;
        float currentY = layers[leftIndex].position.y;

        layers[rightIndex].position = new Vector3(layers[leftIndex].position.x - backgroundSize, currentY, currentZ);

        leftIndex = rightIndex;
        rightIndex--;
        if (rightIndex < 0)
        {
            rightIndex = layers.Length - 1;
        }
    }

    private void ScrollRight()
    {
        float currentZ = layers[rightIndex].position.z;
        float currentY = layers[leftIndex].position.y;

        layers[leftIndex].position = new Vector3(layers[rightIndex].position.x + backgroundSize, currentY, currentZ);

        rightIndex = leftIndex;
        leftIndex++;
        if (leftIndex == layers.Length)
        {
            leftIndex = 0;
        }
    }
}