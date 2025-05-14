using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI pressAnyKeyText;
    [SerializeField] private Image loadingBackGroundImage;
    [SerializeField] private Sprite tutorialSceneBackGroundImage;
    [SerializeField] private Sprite[] hotelSceneBackGroundImage;
    [SerializeField] private Sprite endingNone;
    [SerializeField] private Sprite endingA;
    [SerializeField] private Sprite endingB;
    [SerializeField] private Image progressBar;
    [SerializeField] private GameObject loadingSpinner;

    private void Start()
    {
        loadingBackGroundImage.DOFade(0f, 0f);
        pressAnyKeyText.gameObject.SetActive(false);
        pressAnyKeyText.DOFade(0f, 0f);
        progressBar.fillAmount = 0.15f;
        var nextScene = GameManager.Instance.NextScene;
        if (nextScene == GameManager.SceneName.Tutorial)
        {
            loadingBackGroundImage.sprite = tutorialSceneBackGroundImage;
        }
        else if (nextScene == GameManager.SceneName.Ending_None)
        {
            loadingBackGroundImage.sprite = endingNone;
        }
        else if (nextScene == GameManager.SceneName.Ending_A)
        {
            loadingBackGroundImage.sprite = endingA;
        }
        else if (nextScene == GameManager.SceneName.Ending_B)
        {
            loadingBackGroundImage.sprite = endingB;
        }
        else
        {
            loadingBackGroundImage.sprite = hotelSceneBackGroundImage[Random.Range(0, hotelSceneBackGroundImage.Length)];
        }
        StartCoroutine(LoadScene((int)nextScene));
    }


    IEnumerator LoadScene(int sceneIndex)
    {
        float timer = 0.0f;
        bool fadeComplete = false;
        loadingBackGroundImage.DOFade(1f, 7f).OnComplete(() => fadeComplete = true);
        yield return new WaitUntil(() => fadeComplete);

        Application.backgroundLoadingPriority = ThreadPriority.High;

        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
        op.allowSceneActivation = false;
        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                if (progressBar.fillAmount >= 0.9f)
                {
                    progressBar.fillAmount = 1.0f;
                    break;
                }
            }
        }
        
        fadeComplete = false;
        loadingSpinner.SetActive(false);
        progressBar.gameObject.SetActive(false);
        pressAnyKeyText.gameObject.SetActive(true);
        pressAnyKeyText.DOFade(1f, 1f).OnComplete(() => fadeComplete = true);
        yield return new WaitUntil(() => fadeComplete && Input.anyKey);
        
        
        fadeComplete = false;
        pressAnyKeyText.DOFade(0f, 0.5f);
        loadingBackGroundImage.DOFade(0f, 1f).OnComplete(() => fadeComplete = true);
        yield return new WaitUntil(() => fadeComplete);

        op.allowSceneActivation = true;
    }
}
