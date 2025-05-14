using System.Collections;
using System.Collections.Generic;
using NoManual.Managers;
using NoManual.UI;
using UnityEngine;
using UnityEngine.Events;

public class ExitMenuContext : MenuContext
{
    #region 변수

    [SerializeField] private UI_MonoButton exitButton;
    [SerializeField] private UI_MonoButton yesButton;
    [SerializeField] private UI_MonoButton noButton;
    [SerializeField] private Light exitMenuLight;
    [SerializeField] private CanvasGroup exitMenuCanvasGroup;
    [SerializeField] private Animator exitMenuLightAnimator;
    [SerializeField] private ExitMenuDoor doorAnimator;
    
    #endregion


    
    public override void Initialize(UnityAction onContextEvent, UnityAction offContextEvent)
    {
        OffContext(null);
        
        yesButton.UIInitialize();
        noButton.UIInitialize();
        exitButton.UIInitialize();

        // exit 버튼 이벤트 등록
        exitButton.AddButtonEvent(onContextEvent);

        // No 버튼 이벤트 등록
        noButton.AddButtonEvent(offContextEvent);
    }
    
    public void InitializeExitEvent(UnityAction exitDoorEvent)
    {
        // Yes 버튼 이벤트 등록
        yesButton.AddButtonEvent(ExitGameProcess);
        
        // 게임 종료 연출 이벤트 등록
        doorAnimator.ExitEvent += exitDoorEvent;
    }

    public override void OnContext(UnityAction buttonEvent)
    {
        /* 1. Exit UI 2.5초 Fade out
         * 2. Exit 라이트 점등
         */

        List<UnityAction> events = new List<UnityAction>()
        {
            () => exitMenuCanvasGroup.blocksRaycasts = true,
        };
        
        exitMenuLight.enabled = true; 
        exitMenuLightAnimator.CrossFade("Exit_Menu_On", 0f);
        
        DOTweenManager.FadeInCanvasGroup(exitMenuCanvasGroup, 3f, events);
    }

    public override void OffContext(UnityAction buttonEvent)
    {
       /* 1. Exit UI 1초 Fade Out
        * 2. 불 끄기
        */
       List<UnityAction> events = new List<UnityAction>()
       {
           () => exitMenuLight.enabled = false,
           buttonEvent,
       };
       
        exitMenuCanvasGroup.blocksRaycasts = false;
        exitMenuLightAnimator.CrossFade("Exit_Menu_Off", 0f);
        DOTweenManager.FadeOutCanvasGroup(exitMenuCanvasGroup, 1f, events);
    }

    private void ExitGameProcess()
    {
        DOTweenManager.FadeOutCanvasGroup(exitMenuCanvasGroup, 1f,
            new List<UnityAction> { doorAnimator.PlayAnimExitDoorOn });
    }


}
