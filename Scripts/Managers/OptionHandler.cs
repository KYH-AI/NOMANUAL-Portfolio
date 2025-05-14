using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using HFPS.Systems;
using HFPS.Player;
using UnityEngine.Serialization;

public class OptionHandler 
{
    [Serializable]
    public class OptionData : SaveData
    {
        public float mouseSensitivity;
        public bool mouseSmooth;
        public bool windowScreenMode;
        public float masterValue;
        public float bgmValue;
        public float sfxValue;
        public int languageLocalization;
        
        public override void DefaultSettingValue()
        {
            this.mouseSensitivity = 1.5f;
            this.mouseSmooth = true;
            this.windowScreenMode = false;
            this.masterValue = 1f;
            this.bgmValue = 1f;
            this.sfxValue = 1f;
            this.languageLocalization = 0;
        }
    }
    
    public enum AudioMixerChanel
    {
        Master = 0,
        BGM = 1,
        SFX = 2
    }

    #region 옵션 값 제한 설정

    private const float _MOUSE_SENSITIVITY_MIN = 0.1f;
    private const float _MOUSE_SENSITIVITY_MAX = 3.0f;
    private const float _VOLUME_MIN = 0.1f;
    private const float _VOLUME_MAX = 1.0f;

    #endregion
    
    public OptionData CurrentOptionData { get; private set; }
    // 옵션 변경 이벤트 실행
    private List<Action> _applyOptionChangeListeners = new List<Action>();
    private const string _OPTION_FILE_NAME = "OptionSetting";
    public AudioMixer audioMixer { get; private set; }

    public OptionHandler()
    {
        // 오디오 믹서 가져오기
        audioMixer = Resources.Load<AudioMixer>("Audio/Master Audio Mixer");
        // 옵션 파일 정보 가져오기
        SavaFileDataWriter savaFileDataWriter = new SavaFileDataWriter(_OPTION_FILE_NAME);
        CurrentOptionData = savaFileDataWriter.LoadFile<OptionData>();
        // 옵션 값 예외처리
        if (!OptionDataClampCheck())
        {
            // 무결성 검사 후 이상이 있으면 미리 정해둔 값으로 저장
            savaFileDataWriter.SaveFile(CurrentOptionData);
        }
        
        // 언어 설정 
        if(GameManager.HasReference) SetLocalization();
    }

    
    /// <summary>
    /// 옵션 값 최소, 최대치 예외처리
    /// </summary>
    private bool OptionDataClampCheck()
    {
        bool integrity = true; // 무결성 여부

        if (CurrentOptionData.mouseSensitivity < _MOUSE_SENSITIVITY_MIN || CurrentOptionData.mouseSensitivity > _MOUSE_SENSITIVITY_MAX)
        {
            CurrentOptionData.mouseSensitivity = _MOUSE_SENSITIVITY_MAX / 2 ;
            integrity = false;
        }

        if (CurrentOptionData.masterValue < _VOLUME_MIN || CurrentOptionData.masterValue > _VOLUME_MAX)
        {
            CurrentOptionData.masterValue = _VOLUME_MAX;
            integrity = false;
        }

        if (CurrentOptionData.bgmValue < _VOLUME_MIN || CurrentOptionData.bgmValue > _VOLUME_MAX)
        {
            CurrentOptionData.bgmValue = _VOLUME_MAX;
            integrity = false;
        }

        if (CurrentOptionData.sfxValue < _VOLUME_MIN || CurrentOptionData.sfxValue > _VOLUME_MAX)
        {
            CurrentOptionData.sfxValue = _VOLUME_MAX;
            integrity = false;
        }

        // 언어 설정
        if (CurrentOptionData.languageLocalization <= (int)LocalizationTextManager.LanguageLocalization.KR &&
            CurrentOptionData.languageLocalization >= (int)LocalizationTextManager.LanguageLocalization.JP)
        {
            CurrentOptionData.languageLocalization = (int)LocalizationTextManager.LanguageLocalization.KR;
            integrity = false;
        }

        return integrity;
    }

    /// <summary>
    /// 옵션 변경 이벤트 등록
    /// </summary>
    public void SubscriberOptionApplyEvent(bool isMainScene)
    {
        // 이벤트 초기화
        _applyOptionChangeListeners.Clear();

        List<Action> optionEvents = new List<Action>()
        {
            () => AudioMixerVolumeChange(AudioMixerChanel.Master, CurrentOptionData.masterValue),
            () => AudioMixerVolumeChange(AudioMixerChanel.BGM, CurrentOptionData.bgmValue),
            () => AudioMixerVolumeChange(AudioMixerChanel.SFX, CurrentOptionData.sfxValue),
            WindowScreenMode,
        };

        // 마우스 설정 & 언어 설정 이벤트 등록
        if (!isMainScene)
        {
            optionEvents.Add(MouseSmoothMode);
            optionEvents.Add(SetMouseSensitivity);
        }
        else
        {
            optionEvents.Add(SetLocalization);
        }

            // 이벤트 등록
        _applyOptionChangeListeners = optionEvents;
    }

    /// <summary>
    /// 옵션 값 데이터 적용
    /// </summary>
    public void ApplyOptionData(float mouseSensitivity = 1.5f, bool mouseSmooth = true, bool windowScreenMode = false, float masterValue = 1f, float bgmValue = 1f, float sfxValue = 1f, int languageLocal = 0)
    {
        if (CurrentOptionData == null)
        {
            CurrentOptionData = new OptionData();
            CurrentOptionData.DefaultSettingValue();
        }
        else
        {
            CurrentOptionData.mouseSensitivity = mouseSensitivity;
            CurrentOptionData.mouseSmooth = mouseSmooth;
            CurrentOptionData.windowScreenMode = windowScreenMode;
            CurrentOptionData.masterValue = masterValue;
            CurrentOptionData.bgmValue = bgmValue;
            CurrentOptionData.sfxValue = sfxValue;
            CurrentOptionData.languageLocalization = languageLocal;
        }
        
        // 옵션 변경 이벤트 실행
        ApplyOptionEvent();
        
        // 옵션 값 파일 저장
        SavaFileDataWriter savaFileDataWriter = new SavaFileDataWriter(_OPTION_FILE_NAME);
        savaFileDataWriter.SaveFile(CurrentOptionData);
    }

    /// <summary>
    /// 옵션 변경 이벤트 실행
    /// </summary>
    public void ApplyOptionEvent()
    {
        // 옵션 변경 이벤트 실행
        foreach (var listener in _applyOptionChangeListeners)
        {
            listener?.Invoke();
        }
    }
    
    /// <summary>
    /// 오디오 믹서 채널 볼륨 조절
    /// </summary>
    public void AudioMixerVolumeChange(AudioMixerChanel chanel, float value)
    {
        string chanelName = chanel.ToString();
        audioMixer.SetFloat(chanelName, Mathf.Log10(value) * 20);
    }

    /// <summary>
    /// 창 모드 설정
    /// </summary>
    public void WindowScreenMode()
    {
        Screen.fullScreen = !CurrentOptionData.windowScreenMode;
    }

    /// <summary>
    /// 마우스 Smooth 모드 설정
    /// </summary>
    public void MouseSmoothMode()
    {
        ScriptManager.Instance.C<MouseLook>().smoothLook = CurrentOptionData.mouseSmooth;
    }

    /// <summary>
    /// 마우스 감소 모드 설정
    /// </summary>
    public void SetMouseSensitivity()
    {
        MouseLook mouseSetting = ScriptManager.Instance.C<MouseLook>();
        mouseSetting.sensitivityY = mouseSetting.sensitivityX = CurrentOptionData.mouseSensitivity;
    }

    /// <summary>
    /// 언어 설정
    /// </summary>
    public void SetLocalization()
    {
        LocalizationTextManager.LanguageLocalization newLanguage = (LocalizationTextManager.LanguageLocalization)CurrentOptionData.languageLocalization;
        // 언어가 변경되지 않으면 무시 (비용이 비쌈)
        if (newLanguage != GameManager.Instance.localizationTextManager.currentLanguage)
        {
            GameManager.Instance.localizationTextManager.ChangeLocale(newLanguage);
        }
    }
    
}
