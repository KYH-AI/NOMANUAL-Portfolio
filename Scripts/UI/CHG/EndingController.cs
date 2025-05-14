using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class EndingController : MonoBehaviour
{
    public GameObject[] EndingImage;
    public VideoHandler _VideoHandler;
    public DemoMain_GlitchController glitchController;
    public float WaitTime = 3f;
    //public GameObject SkipUIObject;
    public UI_SkipButton SkipUIObject;
    public Image TabImage;
    private float EndingVideoTime;
    private int num = 0;
    private bool IsPlayingEndingVideo;

    IEnumerator waitVideoIEnumerator;
    
    
    // Start is called before the first frame update
    void Start()
    {
        NextEndingImage();
        waitVideoIEnumerator = WaitVideo();
    }

    private void Update()
    {
        if(IsPlayingEndingVideo)
        {
            if (SkipUIObject.UpdateSkipUiButton())
            {
                StopCoroutine(waitVideoIEnumerator);
                GameManager.Instance.OpenScene(GameManager.SceneName.Main);
            }
            
            /*
            if (!SkipUIObject.activeSelf)
            {
                if (Input.anyKeyDown)
                {
                    SkipUIObject.SetActive(true);
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.Tab))
                {
                    //점점 채우기
                    TabImage.fillAmount = Mathf.Clamp(TabImage.fillAmount + Time.deltaTime / 2f, 0, 1);
                    if(Mathf.Approximately(TabImage.fillAmount, 1))
                    {
                        StopCoroutine(waitVideoIEnumerator);
                        GameManager.Instance.OpenScene(GameManager.SceneName.Main);
                    }
                }
                else
                {
                    //게이지 점점 줄어듬
                    TabImage.fillAmount = Mathf.Clamp(TabImage.fillAmount - Time.deltaTime, 0, 1);
                }
            }
            */
        }
    }

    private void NextEndingImage()
    {
        if(num < EndingImage.Length)
        {
            StartCoroutine(DisplayImage());
        }
        else
        {
            EndingVideoTime = (float)_VideoHandler.StartPrepareAndReturnTime(VideoName.EndingCreditVideo);
            IsPlayingEndingVideo = true;
            StartCoroutine(waitVideoIEnumerator);
            //영상 재생, 끝나면 데모로 이동
        }
    }
    IEnumerator DisplayImage()
    {
        EndingImage[num].SetActive(true);
        yield return new WaitForSeconds(WaitTime);
        EndingImage[num].SetActive(false);
        yield return new WaitForSeconds(1);
        num++;
        NextEndingImage();

    }
    IEnumerator WaitVideo()
    {
        yield return new WaitForSeconds(EndingVideoTime);
        GameManager.Instance.OpenScene(GameManager.SceneName.Main);
    }
}
