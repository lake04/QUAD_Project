using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager instance;

    public Slider loadingBar;
    public string nextSceneName;
    public float loadingDuration = 3f;
    [SerializeField] private Text txt_loading;
    [SerializeField] private GameObject img_loading;
    [SerializeField] private GameObject img_loadingBk;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        img_loadingBk.SetActive(false);
        loadingBar.enabled = false;
        txt_loading.enabled = false;
        img_loading.SetActive(false);
    }

    public void Loading(string _SceneName)
    {
        nextSceneName = _SceneName;

        img_loadingBk.SetActive(true);
        loadingBar.enabled = true;
        txt_loading.enabled = true;
        img_loading.SetActive(true);

        StartCoroutine(FakeLoading());
        StartCoroutine(LoadingTextAnim());
    }

    IEnumerator FakeLoading()
    {
        float timer = 0f;

        while (timer < loadingDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / loadingDuration;

            loadingBar.value = progress;

            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator LoadingTextAnim()
    {
        string loadingText = "Loading";
        int dotCount = 0;

        while (dotCount < 3)
        {
            for (int i = 0; i <= 3; i++)
            {
                txt_loading.text = loadingText + new string('.', i);
                yield return new WaitForSeconds(0.2f);
            }

            txt_loading.text = loadingText;
            yield return new WaitForSeconds(0.3f);

            dotCount++;

        }
    }
}
