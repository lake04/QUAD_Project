using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionTrigger : MonoBehaviour
{
    [SerializeField] private RegionText text;
    [SerializeField] private Image image;
    [SerializeField] private Transform spawnPos;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public IEnumerator InvokeImgCoroutine()
    {
        yield return new WaitForSeconds(0.2f);

        Sequence sequence = DOTween.Sequence();
        image.color = new Color(0, 0, 0, 0);

        //gameObject.SetActive(true);
        image.enabled = true;
        image.gameObject.SetActive(true);
        sequence.Append(image.DOFade(1.0f, 1.5f));
        sequence.Append(image.DOFade(0f, 1.5f));


        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.player.transform.position = spawnPos.position;

        StartCoroutine(text.InvokeTextCoroutine());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            StartCoroutine(InvokeImgCoroutine());
        }
    }
}
