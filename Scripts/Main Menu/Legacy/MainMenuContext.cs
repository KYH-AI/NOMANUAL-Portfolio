using System.Collections.Generic;
using NoManual.Managers;
using NoManual.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuContext : MenuContext
{

    #region 변수
    
     [SerializeField] private UI_MonoButton newGameButton;
    [SerializeField] private CanvasGroup mainMenuUI;
    [SerializeField] private Image fadeImage;
    [SerializeField] private Animator mainMenuLight;
    [SerializeField] private GameObject laptopPanel;
    
    #endregion


    
    #region 사용자 정의 함수
    
    public override void Initialize(UnityAction onContextEvent, UnityAction offContextEvent)
    {
        /* 버튼 컴포넌트 초기화 */
        newGameButton.UIInitialize();

        /* 메인 버튼 이벤트 등록 */
        newGameButton.AddButtonEvent(onContextEvent);
    }
    


    public override void OnContext(UnityAction buttonEvent)
    {
        List<UnityAction> events = new List<UnityAction>()
        {
            () => laptopPanel.gameObject.SetActive(true),
            () =>  mainMenuUI.blocksRaycasts = true,
        };
        
        mainMenuLight.CrossFade("Main_Menu_On", 0f);
        DOTweenManager.FadeInCanvasGroup(mainMenuUI, 1.5f, events);
    }
    

    public override void OffContext(UnityAction buttonEvent)
    {
        List<UnityAction> events = new List<UnityAction>()
        {
            buttonEvent,
        };
        
        mainMenuUI.blocksRaycasts = false;
   
        mainMenuLight.CrossFade("Main_Menu_Off", 0f);
        laptopPanel.gameObject.SetActive(false);
        DOTweenManager.FadeOutCanvasGroup(mainMenuUI, 1f, events);
    }

    public void NewStartGameProcess(UnityAction newGameEvent)
    {
        // 연출 후 씬 전환 진행
        OffContext(() =>
        {
            DOTweenManager.FadeInImage(fadeImage, 0.1f, new List<UnityAction> { newGameEvent });
        });
   
        
         /*
         * 0. [시작] 메인 UI Fade Out  (5초) 
         * 1. 라이트 깜밖임 후 소등 (2초) (애니메이션)
         * 2. 노트북 바로 소등     (즉시)
         * 3. 페이드 아웃          (위 조건들이 모두 종료 후 3초)
         * 4. 씬 전환
         */
    }

    #endregion


}

