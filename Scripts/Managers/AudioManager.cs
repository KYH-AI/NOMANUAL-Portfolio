using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NoManual.Managers;
using NoManual.Utils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;


public class AudioManager : MonoBehaviour
{
   
    /// <summary>
    /// SFX 실행할 오디오
    /// </summary>
    public enum SFX_Audio_List
    {
        Player = 0,
        Etc = 1,
        Subtitles = 2,
    }

    [SerializeField] private AudioClipDataBaseScriptable audioClipDataBase;
    [SerializeField] private AudioSource bgmPlayer = null;
    [SerializeField] private AudioSource sfxEtc = null;
    [SerializeField] private AudioSource sfxSubtitle = null;
    [SerializeField] private AudioSource sfxPlayer = null;
   
    [Space(10)]
    [Header("추적 BGM 오디오")] [SerializeField] private AudioSource chaseBgmAudio;
    [Range(0f, 1f)] [SerializeField] private float chaseBgmAudioVolume;
    [Header("추적 SFX 오디오1")][SerializeField] private AudioSource chaseSfxChannel1Audio;
    [Range(0f, 1f)] [SerializeField] private float chaseSfxChannel1AudioVolume;
    [Header("추적 SFX 오디오2")][SerializeField] private AudioSource chaseSfxChannel2Audio;
    [Range(0f, 1f)] [SerializeField] private float chaseSfxChannel2AudioVolume;
    [Header("Fade Out 시간")] [SerializeField] private float fadeOutDuration;
    
    private List<bool> _chaseAudioState = new List<bool>(2); 
    private readonly List<Tween> _chaseAudioFadeOut = new List<Tween>();
    

    public AudioMixer audioMixer { get; set; }

    public void SetSfxAudioVolume(SFX_Audio_List targetAudio, float volume)
    {
        AudioSource audioSource;
        switch (targetAudio)
        {
            case SFX_Audio_List.Player:
                audioSource = sfxPlayer;
                break;
            default: 
                audioSource = sfxEtc;
                break;
        }

        if (audioSource) audioSource.volume = volume;
    }
    
    /// <summary>
    /// BGM 오디오 정지
    /// </summary>
    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    /// <summary>
    /// SFX 오디오 실행
    /// </summary>
    public void PlaySFX(SFX_Audio_List targetAudio, SfxEnum p_sfxName, bool oneShot)
    {
        AudioClip sfx = GetAudioClip(p_sfxName);
        if (!sfx)
        {
            ErrorCode.SendError(ErrorCode.ErrorCodeEnum.GetAudioClip);
            return;
        }
        PlaySFX(targetAudio, oneShot, sfx);
    }

    public void PlaySFX(SFX_Audio_List targetAudio, bool oneShot, AudioClip audioClip)
    {
        AudioSource audioSource;
        switch (targetAudio)
        {
            case SFX_Audio_List.Player:
                audioSource = sfxPlayer;
                break;
            default: 
                audioSource = sfxEtc;
                break;
        }

        audioSource.clip = audioClip;
        if (oneShot)
        {
            audioSource.PlayOneShot(audioClip);
        }
        else
        {
            audioSource.Play();
        }
    }
    
    /// <summary>
    /// 자막 SFX 오디오 실행
    /// </summary>
    public void PlaySubtitleSFX(AudioClip subtitleClip, bool oneShot)
    {
        sfxSubtitle.clip = subtitleClip;
        if (oneShot)
        {
            sfxSubtitle.PlayOneShot(subtitleClip);
        }
        else
        {
            sfxSubtitle.Play();
        }
    }

    /// <summary>
    /// 자막 SFX 3D 오디오 실행
    /// </summary>
    public void PlaySubtitle3dSFX(AudioSource audio3D, AudioClip subtitleClip)
    {
        audio3D.PlayOneShot(subtitleClip);
    }

    /// <summary>
    /// BGM 오디오 Fade In, Out
    /// </summary>
    public void PlayBGM_Fade(bool isFadeIn, float targetValue)
    {
        DOTweenManager.FadeAudioSource(isFadeIn, bgmPlayer, 5f, targetValue, null);
    }
    
    /// <summary>
    /// 특정 BGM 오디오 Fade In, Out
    /// </summary>
    public void PlayBGM_Fade(AudioSource targetAudio, bool isFadeIn, float targetValue, float duration)
    {
        DOTweenManager.FadeAudioSource(isFadeIn, targetAudio, duration, targetValue, null);
        targetAudio.Play();
    }

    #region 크리처 추적 오디오

    /// <summary>
    /// 추적 오디오 Fade Out 확인
    /// </summary>
    private bool IsPlayingChaseAudio()
    {
        return _chaseAudioFadeOut.Count != 0;
    }

    /// <summary>
    /// 추적 오디오 실행
    /// </summary>
    public void PlayChaseAudio()
    {
        if (IsPlayingChaseAudio())
        {
             // 진행 중인 트윈 중지 및 리스트 초기화
            foreach (var audioFade in _chaseAudioFadeOut) audioFade.Kill();
            _chaseAudioFadeOut.Clear();
            
            DOTweenManager.FadeAudioSource(true, chaseBgmAudio, 1f, chaseBgmAudioVolume, null);
            DOTweenManager.FadeAudioSource(true, chaseSfxChannel1Audio, 1f, chaseSfxChannel1AudioVolume, null);
            DOTweenManager.FadeAudioSource(true, chaseSfxChannel2Audio, 1f, chaseSfxChannel2AudioVolume, null);
            
            Debug.Log("추적 사운드 중지 후 재생");
        }
        else if(_chaseAudioState.Count == 0)
        {
            SettingAudioSource(chaseBgmAudio, GetAudioClip(BgmEnum.ChaseStart), chaseBgmAudioVolume, true);
            chaseBgmAudio.Play();
            
            SettingAudioSource(chaseSfxChannel1Audio, GetAudioClip(SfxEnum.ScaredBreath), chaseSfxChannel1AudioVolume, true);
            chaseSfxChannel1Audio.Play();
            
            SettingAudioSource(chaseSfxChannel2Audio, GetAudioClip(SfxEnum.HearthBreath), chaseSfxChannel2AudioVolume, true);
            chaseSfxChannel2Audio.Play();
            
            Debug.Log("추적 사운드 재생");
        }
        chaseSfxChannel1Audio.PlayOneShot(GetAudioClip(SfxEnum.StartBreath));
        _chaseAudioState.Add(true);
    }

    /// <summary>
    /// 추적 오디오 중지
    /// </summary>
    public void StopChaseAudio()
    {
        if (_chaseAudioState.Count > 0)
        {
            _chaseAudioState.RemoveAt(_chaseAudioState.Count - 1);
            // 추적 사운드가 아직 남아있는 경우 추적 효과음이 필요하다고 판단
            if (_chaseAudioState.Count >= 1) return;
        }

        if (IsPlayingChaseAudio())
        {
            // 진행 중인 트윈 중지 및 리스트 초기화
            foreach (var audioFade in _chaseAudioFadeOut) audioFade.Kill();
            _chaseAudioFadeOut.Clear(); 
        }

        // BGM 페이드 아웃
        var bgmFade = DOTweenManager.FadeAudioSource(false, 
            chaseBgmAudio, fadeOutDuration, 
            chaseBgmAudio.volume, 
            0f, 
            new List<UnityAction> { chaseBgmAudio.Stop });
        bgmFade.OnComplete(() => _chaseAudioFadeOut.Remove(bgmFade)); // 완료 후 리스트에서 제거
        _chaseAudioFadeOut.Add(bgmFade);
    
        // SFX 채널 1 페이드 아웃
        var sfx1Fade = DOTweenManager.FadeAudioSource(false, 
            chaseSfxChannel1Audio, 
            fadeOutDuration, 
            chaseSfxChannel1Audio.volume, 
            0f, 
            new List<UnityAction> { chaseSfxChannel1Audio.Stop });
        sfx1Fade.OnComplete(() => _chaseAudioFadeOut.Remove(sfx1Fade)); // 완료 후 리스트에서 제거
        _chaseAudioFadeOut.Add(sfx1Fade);
    
        // SFX 채널 2 페이드 아웃
        var sfx2Fade = DOTweenManager.FadeAudioSource(false, 
            chaseSfxChannel2Audio, 
            fadeOutDuration, 
            chaseSfxChannel2Audio.volume, 
            0f, 
            new List<UnityAction> { chaseSfxChannel2Audio.Stop });
        sfx2Fade.OnComplete(() => _chaseAudioFadeOut.Remove(sfx2Fade)); // 완료 후 리스트에서 제거
        _chaseAudioFadeOut.Add(sfx2Fade);
    }

    /// <summary>
    /// 추적 오디오 즉시 종료
    /// </summary>
    public void StopChaseAudioNotFade()
    {
        // 진행 중인 트윈 중지 및 리스트 초기화
        foreach (var audioFade in _chaseAudioFadeOut) audioFade.Kill();
        _chaseAudioFadeOut.Clear(); 
        
        chaseBgmAudio.Stop();
        chaseSfxChannel1Audio.Stop();
        chaseSfxChannel2Audio.Stop();
    }
    

    #endregion

    /// <summary>
    /// 오디오 소스 컴포넌트 세팅
    /// </summary>
    private void SettingAudioSource(AudioSource audioSource, AudioClip audioClip, float volume, bool loop)
    {
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = loop;
    }

    #region 오디오 클립 얻기

    
    /// <summary>
    /// 자막음 가져오기
    /// </summary>
    public AudioClip GetAudioClip(LocalizationTable.NPCTableTextKey audioClipKey)
    {
        return GetAudioClipInternal(audioClipKey);
    }

    /// <summary>
    /// 효과음 가져오기
    /// </summary>
    public AudioClip GetAudioClip(SfxEnum audioClipKey)
    {
        return GetAudioClipInternal(audioClipKey);
    }

    /// <summary>
    /// 배경음 가져오기
    /// </summary>
    public AudioClip GetAudioClip(BgmEnum audioClipKey)
    {
        return GetAudioClipInternal(audioClipKey);
    }
    
    private AudioClip GetAudioClipInternal<TEnum>(TEnum audioClipKey) where TEnum : Enum
    {
        return audioClipDataBase.GetAudioClip(audioClipKey);
    }

    #endregion

    /// <summary>
    /// 오디오 믹서 그룹 가져오기
    /// </summary>
    public AudioMixerGroup GetAudioMixerGroup(OptionHandler.AudioMixerChanel audioMixerChanel)
    {
        return audioMixer.FindMatchingGroups(audioMixerChanel.ToString())[0];
    }
}