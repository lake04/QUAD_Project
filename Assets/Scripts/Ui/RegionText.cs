using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionText : MonoBehaviour
{
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void Start()
    {
        StartCoroutine(InvokeTextCoroutine());
    }

    private IEnumerator InvokeTextCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        Sequence sequence = DOTween.Sequence();

        text.color = new Color(0, 0, 0, 0);

        text.gameObject.SetActive(true);

        sequence.Append(text.DOFade(1.0f, 1.1f));
    }
}
