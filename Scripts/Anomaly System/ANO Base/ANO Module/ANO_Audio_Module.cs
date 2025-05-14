using UnityEngine;

[System.Serializable]
public class ANO_Audio_Module 
{
    [Header("오디오 참조")]
    public AudioSource audioSource;
    
    [Header("소리")]
    [Range(0f, 1f)] public float audioVolumeSize;

    [Header("소리 범위")]
    public float minVolumeDistance;
    public float maxVolumeDistance;

    [Header("오디오 Roll off Mode")] 
    public AudioRolloffMode audioRolloffMode;

    [Header("오디오 클립 참조")] 
    public AudioClip audioClip;

    [Header("오디오 블랜드")] 
    [Range(0f, 1f)] public float audioBlend;

    [Header("오디오 실행 방식")] 
    [HideInInspector] public bool loop = false;   
    [HideInInspector] public bool oneShot  = false;   
}
