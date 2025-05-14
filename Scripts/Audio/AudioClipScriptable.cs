using UnityEngine;

public class AudioClipScriptable : ScriptableObject
{
    [Header("오디오 클립 랜덤 사용")]
    public bool IsRandomPlay = false;
    [Header("오디오 클립")]
    public AudioClip[] AudioClips;
}
