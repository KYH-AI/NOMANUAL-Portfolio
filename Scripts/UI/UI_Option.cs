using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Option : MonoBehaviour
{
    [Header("창모드")]
    [SerializeField] private Toggle windowScreenMode;
    [Header("마우스 감도 (0.1 ~ 3.0)")]
    [SerializeField] private Slider mouseSensitivity;
    [Header("마우스 스무스")]
    [SerializeField] private Toggle mouseSmooth;
    [Header("마스터 볼륨 (0.1 ~ 1)")]
    [SerializeField] private Slider masterVolume;
    [Header("마스터 볼륨 (0.1 ~ 1)")]
    [SerializeField] private Slider bgmVolume;
    [Header("효과음 볼륨 (0.1 ~ 1)")] 
    [SerializeField] private Slider sfxVolume;
    [Header("언어 설정 (KR, EN, JP)")] 
    [SerializeField] private TMP_Dropdown languageLocalization;
    
    private OptionHandler _optionHandler;

    /// <summary>
    /// 옵션 UI 초기화 (Awake에서 실행 X)
    /// </summary>
    public void InitializeOptionUI()
    {
        // 개발자 모드 전용
        if (!GameManager.HasReference)
            _optionHandler = new OptionHandler();
        else
            _optionHandler = GameManager.Instance.optionHandler;
        
        this.gameObject.SetActive(false);
        
        masterVolume.onValueChanged.RemoveAllListeners();
        bgmVolume.onValueChanged.RemoveAllListeners();
        sfxVolume.onValueChanged.RemoveAllListeners();
        
        masterVolume.onValueChanged.AddListener(value =>_optionHandler.AudioMixerVolumeChange(OptionHandler.AudioMixerChanel.Master, value));
        bgmVolume.onValueChanged.AddListener(value => _optionHandler.AudioMixerVolumeChange(OptionHandler.AudioMixerChanel.BGM, value));
        sfxVolume.onValueChanged.AddListener(value => _optionHandler.AudioMixerVolumeChange(OptionHandler.AudioMixerChanel.SFX, value));
        
        // 메인 씬 확인
        bool isMainScene = mouseSensitivity && mouseSmooth ? false : true;
        
        _optionHandler.SubscriberOptionApplyEvent(isMainScene);
        _optionHandler.ApplyOptionEvent();
        UpdateOptionUI();
    }

    /// <summary>
    /// 옵션 값 데이터 변경
    /// </summary>
    public void ApplyOptionButton()
    {
        OptionHandler.OptionData optionData = _optionHandler.CurrentOptionData;

        float msSen = optionData.mouseSensitivity;
        bool msSmooth = optionData.mouseSmooth;
        int languageLocal = optionData.languageLocalization;
        
        if (mouseSensitivity && mouseSmooth)
        {
            msSen = mouseSensitivity.value;
            msSmooth = mouseSmooth.isOn;
        }

        if (languageLocalization)
        {
            languageLocal = languageLocalization.value;
        }
        
        // 옵션 값 데이터 저장
        _optionHandler.ApplyOptionData(msSen, msSmooth, 
                                                            windowScreenMode.isOn, 
                                                            masterVolume.value, bgmVolume.value, sfxVolume.value,
                                                            languageLocal);
        // 옵션 UI 변경
        UpdateOptionUI();
    }

    /// <summary>
    /// 옵션 UI 변경
    /// </summary>
    private void UpdateOptionUI()
    {
        OptionHandler.OptionData optionData = _optionHandler.CurrentOptionData;

        windowScreenMode.isOn = optionData.windowScreenMode;
        if (mouseSensitivity && mouseSmooth)
        {
            mouseSensitivity.value = optionData.mouseSensitivity;
            mouseSmooth.isOn = optionData.mouseSmooth;
        }
        
        if (languageLocalization)
        {
            languageLocalization.value = optionData.languageLocalization;
        }
        
        masterVolume.value = optionData.masterValue;
        bgmVolume.value = optionData.bgmValue;
        sfxVolume.value = optionData.sfxValue;

    }

    /// <summary>
    /// 옵션 UI를 닫으면 옵션 UI를 변경
    /// </summary>
    private void OnDisable()
    {
        UpdateOptionUI();
    }
}
