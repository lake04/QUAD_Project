using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class End : MonoBehaviour
{
    [SerializeField] private Text text;

    private void OnEnable()
    {
        text = GetComponent<Text>();
    }

    private void Awake()
    {
    }

    private void Start()
    {
        StartCoroutine(InvokeTextCoroutine());
    }

    public IEnumerator InvokeTextCoroutine()
    {
        yield return new WaitForSeconds(0.2f);

        Sequence sequence = DOTween.Sequence();

        //text.color = new Color(255, 255, 255, 0);

        gameObject.SetActive(true);

        sequence.Append(text.DOFade(1.0f, 1.5f));
        sequence.Append(text.DOFade(0f, 1.5f));

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Title");
    }

}
