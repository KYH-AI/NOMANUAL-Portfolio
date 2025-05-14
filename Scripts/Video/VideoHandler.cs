using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[System.Serializable]
public struct VideoData
{
    // 24.04.05 오디오 소스 추가
    public VideoName videoName;
    public VideoClip videoClip;
    public AudioSource audioClip;
}
public enum VideoName { MainMenuVideo, EndVideo, EndingCreditVideo};
public class VideoHandler : MonoBehaviour
{
    public List<VideoData> VideoDatas;
    public RawImage mScreen = null;
    public float videoReady = 0.5f;

    Dictionary<string, VideoClip> VideoHashMap = new Dictionary<string, VideoClip>();
    public VideoPlayer mVideoPlayer = null;

    void Awake()
    {
        foreach(var data in VideoDatas)
        {
            VideoHashMap[data.videoName.ToString()] = data.videoClip;
        }
    }
    void Start()
    {
        mVideoPlayer = GetComponent<VideoPlayer>();
        if (mScreen != null && mVideoPlayer != null && GameManager.Instance.IsSceneCorrect(GameManager.SceneName.Main))
        {
            StartPrepareAndReturnTime(VideoName.MainMenuVideo);
        }
    }
    public double StartPrepareAndReturnTime(VideoName videoName)
    {
        if (VideoHashMap.Count == 0) return 0;
        StartCoroutine(PrepareVideo(videoName));
        return mVideoPlayer.clip.length;
    }
    public IEnumerator PrepareVideo(VideoName videoName)
    {
        // 비디오 준비
        mVideoPlayer.clip = VideoHashMap[videoName.ToString()];
        mVideoPlayer.Prepare();
        
        // 비디오가 준비되는 것을 기다림
        while (!mVideoPlayer.isPrepared)
        {
            yield return null;
        }
        
        // 비디오가 준비되면 연결된 오디오 소스를 재생
        VideoData videoData = VideoDatas.Find(data => data.videoName == videoName);
        if (videoData.audioClip != null)
        {
            videoData.audioClip.Play();
        }
        
        PlayVideo();
        // VideoPlayer의 출력 texture를 RawImage의 texture로 설정
        mScreen.texture = mVideoPlayer.texture;
    }

    
    // Controller에서 호출할 부분들 
    
    public void PlayVideo()
    {
        if (mVideoPlayer != null && mVideoPlayer.isPrepared)
        {
            // 비디오 재생
            mVideoPlayer.Play();
        }
    }

    public void StopVideo()
    {
        if (mVideoPlayer != null && mVideoPlayer.isPrepared)
        {
            // 비디오 멈춤
            mVideoPlayer.Stop();
        }
    }
    
}
