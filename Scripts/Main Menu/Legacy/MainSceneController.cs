using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;


public class MainSceneController : MonoBehaviour
{
    #region 변수

    [Header("가상 카메라")]
    [SerializeField] private CinemachineVirtualCamera mainMenuVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera settingMenuVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera exitMenuVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera exitMenuGoalVirtualCamera;
    

    [Header("메인 씬 Core")] 
    private MenuContext _lastMenuContext;
    [SerializeField] private MainMenuContext mainMenuContext;
    [SerializeField] private SettingOptionContext settingOptionContext;
    [SerializeField] private ExitMenuContext exitMenuContext;

    #endregion

    private void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// UI 초기화 및 이벤트 등록
    /// </summary>
    private void Initialize()
    {
        mainMenuContext.Initialize(NewStartGame, null);
        settingOptionContext.Initialize(SettingOption, SettingOptionMenuToMainMenu);
        exitMenuContext.Initialize(ExitGame, ExitMenuToMainMenu);
        exitMenuContext.InitializeExitEvent(ExitGameEvent);
    }
    
    private void ChangeVirtualCamera(Define.MainSceneCameraType mainSceneCameraType)
    {
        int camPriority;

        mainMenuVirtualCamera.Priority = (int)Define.MainSceneCameraPriority.None;
        settingMenuVirtualCamera.Priority = (int)Define.MainSceneCameraPriority.None;
        exitMenuVirtualCamera.Priority = (int)Define.MainSceneCameraPriority.None;
        exitMenuGoalVirtualCamera.Priority = (int)Define.MainSceneCameraPriority.None;
        
        switch (mainSceneCameraType)
        {
            case Define.MainSceneCameraType.Main:
                camPriority = (int)Define.MainSceneCameraPriority.Priority_1;  //  우선순위 설정
                mainMenuVirtualCamera.Priority = camPriority;
                break;
            
            case Define.MainSceneCameraType.Setting:
                camPriority = (int)Define.MainSceneCameraPriority.Priority_1; //  우선순위 설정
                settingMenuVirtualCamera.Priority = camPriority;
                break;
            
            case Define.MainSceneCameraType.Exit:
                camPriority = (int)Define.MainSceneCameraPriority.Priority_1; // 우선순위 설정
                exitMenuVirtualCamera.Priority = camPriority;
                break;
            
            case  Define.MainSceneCameraType.ExitGoal:
                camPriority = (int)Define.MainSceneCameraPriority.Priority_1; // 우선순위 설정
                exitMenuGoalVirtualCamera.Priority = camPriority;
                break;
            
            default:
                break; //TODO : 예외처리
        }
    }
    
    private void NewStartGame()
    {
        mainMenuContext.NewStartGameProcess(null);
    }

    private void SettingOption()
    {
        mainMenuContext.OffContext( () =>
        {
            settingOptionContext.OnContext(null);
            ChangeVirtualCamera(Define.MainSceneCameraType.Setting);
        });
    }

    private void ExitGame()
    {
        mainMenuContext.OffContext(() =>
        {
            exitMenuContext.OnContext(null);
            ChangeVirtualCamera(Define.MainSceneCameraType.Exit);
        });
    }

    private void SettingOptionMenuToMainMenu()
    {
        settingOptionContext.OffContext( () =>
        {
            mainMenuContext.OnContext(null);
            ChangeVirtualCamera(Define.MainSceneCameraType.Main);
        });
    }

    private void ExitMenuToMainMenu()
    {
        exitMenuContext.OffContext( () =>
        {
            mainMenuContext.OnContext(null);
            ChangeVirtualCamera(Define.MainSceneCameraType.Main);
        });
    }

    /// <summary>
    /// 종료 이벤트
    /// </summary>
    private void ExitGameEvent()
    {
        ChangeVirtualCamera(Define.MainSceneCameraType.ExitGoal);
        StartCoroutine(UnityApplicationExit(2f));
    }
    
    #region Unity 어플리케이션 종료

    
    private IEnumerator UnityApplicationExit(float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);
        
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                Debug.Log("게임 종료");
        #else
               Application.Quit(); // 어플리케이션 종료
        #endif
        
        
    }
    
    #endregion

}
