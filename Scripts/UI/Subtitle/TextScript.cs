using UnityEngine;

[System.Serializable]
public class TextScript
{
    [Header("대사 NPC CSV")]  public string TextKey = string.Empty;
    [Header("대사 음성 클립")] public AudioClip VoiceClip  = null;
    [Header("대사 오디오 소스 (생략가능)")] public AudioSource Voice3dAudioSource = null;
    [Header("텍스트 추가 유지 시간 (싱크)")] public float Offset = 2f;
}
