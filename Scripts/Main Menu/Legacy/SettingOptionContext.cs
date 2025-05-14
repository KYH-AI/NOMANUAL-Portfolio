using System.Collections;
using System.Collections.Generic;
using NoManual.Managers;
using NoManual.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingOptionContext : MenuContext
{
    #region 변수
    
    [SerializeField] private UI_MonoButton settingOptionButton;
    [SerializeField] private UI_MonoButton settingVideoButton;
    [SerializeField] private UI_MonoButton returnMainMenuButton;
    
    [SerializeField] private CanvasGroup settingUI;
    
    [SerializeField] private GameObject soundSettingUI;
    [SerializeField] private GameObject videoSettingUI;

    [SerializeField] private TMP_Dropdown screenResolutionDropdown;
    [SerializeField] private Slider soundSlider;
    
    [SerializeField] private Animator settingOptionLightAnimator; 
    [SerializeField] private Animator settingOptionFlashLightAnimator;
    [SerializeField] private Light settingOptionLight;
    [SerializeField] private Light settingOptionFlashLight_01;
    [SerializeField] private Light settingOptionFlashLight_02;

    [Header("해상도 옵션 값")]
    private List<Resolution> _resolutions;
    
    #endregion

    #region 프로퍼티

    private int _ResolutionIndex
    {
        get => PlayerPrefs.GetInt("ResolutionIndex", 0); // 아무 값도 없으면 0을 가져옴
        set => PlayerPrefs.SetInt("ResolutionIndex", value);
    }

    #endregion
    
    #region 사용자 정의 함수



    public override void Initialize(UnityAction onContextEvent, UnityAction offContextEvent)
    {
        // 해상도 초기화 (1 프레임 딜레이)
        Invoke(nameof(SetResolution), 0.01f);

        // 드롭다운 이벤트 등록
        screenResolutionDropdown.onValueChanged.AddListener(ScreenResolutionDropDownChanged);
        
        // 오브젝트 초기화
        OffContext(null);
        
        /* 버튼 컴포넌트 초기화 */
        settingOptionButton.UIInitialize();
        settingVideoButton.UIInitialize();
        returnMainMenuButton.UIInitialize();

        /* 세팅 버튼 이벤트 등록 */
        settingOptionButton.AddButtonEvent(onContextEvent);
        
        /* 비디오 세팅 버튼 이벤트 등록 */
        settingVideoButton.AddButtonEvent(OnVideoSetting);
        
        /* 돌아가기 버튼 이벤트 등록 */
        returnMainMenuButton.AddButtonEvent(offContextEvent);
    }
    


    public override void OnContext(UnityAction buttonEvent)
    {
        /*  옵션 연출 시작 */
        
        List<UnityAction> events = new List<UnityAction>()
        {
           () => settingUI.blocksRaycasts = true,
          // () => settingOptionLight.enabled = true,
          // () => settingOptionFlashLight.enabled = true,
          // () => settingOptionLightAnimator.CrossFade("Setting_Menu_On", 0f),
          // () => settingOptionFlashLightAnimator.CrossFade("Setting_Menu_On", 0f),
        };
        
        soundSettingUI.SetActive(true);
        videoSettingUI.SetActive(false);
        
        settingOptionLight.enabled = true;
        settingOptionFlashLight_01.enabled = true;
        settingOptionFlashLight_02.enabled = true;
        
        settingOptionLightAnimator.CrossFade("Setting_Menu_On", 0f);
        settingOptionFlashLightAnimator.CrossFade("Setting_Menu_On", 0f);
        
        DOTweenManager.FadeInCanvasGroup(settingUI, 3f, events);
    }

    public override void OffContext(UnityAction buttonEvent)
    {
      /*  옵션 연출 종료 */
      
      List<UnityAction> events = new List<UnityAction>()
      {
          () => soundSettingUI.SetActive(false),
          () => videoSettingUI.SetActive(false),
          () => settingOptionLight.enabled = false,
          () => settingOptionFlashLight_01.enabled = false,
          () => settingOptionFlashLight_02.enabled = false,
          buttonEvent,
      };

      settingUI.blocksRaycasts = false;

      settingOptionLightAnimator.CrossFade("Setting_Menu_Off" ,0f);
      settingOptionFlashLightAnimator.CrossFade("Setting_Menu_Off", 0f);
      DOTweenManager.FadeOutCanvasGroup(settingUI, 1f, events);
    }

    private void OnVideoSetting()
    {
        /* 1. 사운드 옵션 UI 끄기 
         * 2. 비디오 옵션 UI 켜기
         */
        
        soundSettingUI.SetActive(false);
        videoSettingUI.SetActive(true);
    }

    private void OffVideoSetting()
    {
        /* 1. 비디오 옵션 UI 끄기 
         * 2. 사운드 옵션UI 켜기
         */
        
        soundSettingUI.SetActive(true);
        videoSettingUI.SetActive(false);
    }
    
    private void SetResolution()
    {
        _resolutions = new List<Resolution>(Screen.resolutions);
        _resolutions.Reverse();

        // 해상도 값 얻기
        if (_resolutions.Count > 0)
        {
            List<Resolution> tempResolutions = new List<Resolution>();
            int curWidth = _resolutions[0].width;
            int curHeight = _resolutions[0].height;
            
            tempResolutions.Add(_resolutions[0]);
            foreach (var resolution in _resolutions)
            {
                if (curWidth != resolution.width || curHeight != resolution.height)
                {
                    tempResolutions.Add(resolution);
                    curWidth = resolution.width;
                    curHeight = resolution.height;
                }
            }
            _resolutions = tempResolutions;
        }
        
        // 해상도 드롭다운 입력
        List<string> options = new List<string>();
        foreach (var resolution in _resolutions)
        {
            string option = $"{resolution.width} x {resolution.height}";
            options.Add(option);
        }
        
        screenResolutionDropdown.ClearOptions();
        screenResolutionDropdown.AddOptions(options);

        screenResolutionDropdown.value = _ResolutionIndex;
        screenResolutionDropdown.RefreshShownValue(); // 해상도 변경 값 새로고침
    }

    private void ScreenResolutionDropDownChanged(int index)
    {
        _ResolutionIndex = index;
        Resolution resolution = _resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    
    #endregion
}
