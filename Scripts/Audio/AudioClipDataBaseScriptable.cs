using System;
using System.Collections;
using System.Collections.Generic;
using NoManual.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "AudioClipDataBase",menuName = "AudioClip/AudioClipDataBaseScriptable")]
public class AudioClipDataBaseScriptable : ScriptableObject
{
    [Header("배경음 DB"), Space(10)]
    [SerializeField] private BgmClipScriptable[] bgmAudioDataBase;
    [Header("효과음 DB"), Space(10)]
    [SerializeField] private SfxClipScriptable[] sfxAudioDataBase;
    [Header("자막음 DB"), Space(10)]
    [SerializeField] private SubtitleClipScriptable[] subtitleAudioDataBase;

    // 런타임 오디으 클립 딕셔너리
    private Dictionary<Enum, AudioClipScriptable> _audioClipDictionary;
    

    /// <summary>
    /// 오디오 클립 딕셔너리 저장
    /// </summary>
    private void OnEnable()
    {
        _audioClipDictionary = new Dictionary<Enum, AudioClipScriptable>();
        foreach (var bgmClip in bgmAudioDataBase)
        {
            _audioClipDictionary[bgmClip.bgmEnum] = bgmClip;
        }

        foreach (var sfxClip in sfxAudioDataBase)
        {
            _audioClipDictionary[sfxClip.sfxEnum] = sfxClip;
        }

        foreach (var subtitleClip in subtitleAudioDataBase)
        {
            _audioClipDictionary[subtitleClip.subtitleEnum] = subtitleClip;
        }
    }
    
    /// <summary>
    /// 오디오 클립 가져오기
    /// </summary>
    public AudioClip GetAudioClip<TEnum>(TEnum audioClipKey) where TEnum : Enum
    {
        if (_audioClipDictionary.TryGetValue(audioClipKey, out AudioClipScriptable audioData))
        {
            // 랜덤 재생 여부에 따라 적절한 클립 반환
            return audioData.IsRandomPlay ? audioData.AudioClips[Random.Range(0, audioData.AudioClips.Length)] : audioData.AudioClips[0];
        }
        ErrorCode.SendError(ErrorCode.ErrorCodeEnum.GetAudioClip);
        return null;
    }
}
