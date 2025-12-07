using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RegionTrigger : MonoBehaviour
{
    [SerializeField] private RegionText text;
    [SerializeField] private Image image;
    [SerializeField] private Transform spawnPos;

    [Header("Background Settings")]
    [SerializeField] private GameObject activeBackground;   // 이동할 지역의 배경
    [SerializeField] private GameObject deactiveBackground; // 현재 지역의 배경

    // 화면 암전 후 플레이어를 이동시키고 배경을 교체하는 코루틴
    public IEnumerator InvokeImgCoroutine()
    {
        yield return new WaitForSeconds(0.2f);

        // 화면 페이드 인
        image.gameObject.SetActive(true);
        image.color = new Color(0, 0, 0, 0);
        yield return image.DOFade(1.0f, 1.5f).WaitForCompletion();

        // 플레이어 및 카메라 위치 이동
        GameManager.Instance.player.transform.position = spawnPos.position;
        Camera.main.transform.position = new Vector3(spawnPos.position.x, spawnPos.position.y, -10f);

        // 배경 오브젝트 교체
        if (deactiveBackground != null) deactiveBackground.SetActive(false);
        if (activeBackground != null) activeBackground.SetActive(true);

        // 화면 페이드 아웃
        image.DOFade(0f, 1.5f).OnComplete(() => {
            image.gameObject.SetActive(false);
        });

        // 지역 이름 텍스트 출력
        StartCoroutine(text.InvokeTextCoroutine());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(InvokeImgCoroutine());
        }
    }
}