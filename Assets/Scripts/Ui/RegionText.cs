using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionText : MonoBehaviour
{
    [SerializeField] private Image image;

    private void OnEnable()
    {
        image = GetComponent<Image>();
    }

    private void Awake()
    {
    }

    private void Start()
    {
       
    }

    public IEnumerator InvokeTextCoroutine()
    {
        yield return new WaitForSeconds(0.2f);

        Sequence sequence = DOTween.Sequence();

        image.color = new Color(255, 255, 255, 0);

        gameObject.SetActive(true);

        sequence.Append(image.DOFade(1.0f, 1.5f));
        sequence.Append(image.DOFade(0f, 1.5f));
    }


}
