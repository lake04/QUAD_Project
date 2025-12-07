using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionText : MonoBehaviour
{
    [SerializeField] private Text text;

    private void OnEnable()
    {
        text = GetComponent<Text>();
    }

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void Start()
    {
       
    }

    public IEnumerator InvokeTextCoroutine()
    {
        yield return new WaitForSeconds(0.2f);

        Sequence sequence = DOTween.Sequence();

        text.color = new Color(255, 255, 255, 0);

        text.gameObject.SetActive(true);

        sequence.Append(text.DOFade(1.0f, 1.5f));
        sequence.Append(text.DOFade(0f, 1.5f));
    }
}
