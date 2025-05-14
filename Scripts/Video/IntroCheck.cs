using UnityEngine;
using UnityEngine.Video;

public class IntroCheck : MonoBehaviour
{
    public VideoPlayer vid;


    void Start()
    {
        vid.loopPointReached += CheckOver;     
    }

    /// <summary>
    /// 인트로 연출 후 로딩 씬 넘긴 후 메인메뉴
    /// </summary>
    void CheckOver(VideoPlayer vp)
    {
        GameManager.Instance.OpenScene(GameManager.SceneName.Main);
    }
}
