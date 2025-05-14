using System;
using System.Collections;
using System.Collections.Generic;
using NoManual.Task;
using NoManual.Managers;
using UnityEngine;
using UnityEngine.Playables;

namespace NoManual.Tutorial
{
   public class HandoverManager : Singleton<HandoverManager>
   {
      public delegate void StepClearEvent(HandoverStep step);
      
      public TaskHandler TaskHandler { get; private set; }
      
      [Header("PC OS")]
      public Nomanual_PC_OS_Manager pcOSManager;

      [Space(10)] 
      [Header("비디오 타임라인")]
      [SerializeField] private PlayableDirector videoTimeLinePlayer;
      
      [Header("인수인계 스텝 컴포넌트")]
      private Queue<HandoverStep> _handoverStepQueue;

      private HandoverStep _currentStep;

      private void Awake()
      {
         StartCoroutine(InitializedProcess());
      }
        
      private void Start()
      {
         // 게임 시작 Fade Out
         NoManualHotelManager.Instance.PlayerStartFade(false, 15f, true, 0.45f);
      }
      
      private IEnumerator InitializedProcess()
      {
         // NoManualHotelManager의 인스턴스를 얻기 위해 대기
         while (NoManualHotelManager.Instance == null || !NoManualHotelManager.Instance.OnInitialized)
         {
            yield return null;  // 다음 프레임까지 대기
         }
         Initialization();  // 초기화 시작
      }
      
      private void Initialization()
      {
         _handoverStepQueue = new Queue<HandoverStep>(5);
         TaskHandler = new TaskHandler();
         TaskHandler.TaskHandlerInitialization(pcOSManager.UpdateRecordItem_Handover, false);
         TaskHandler.SetNewDayStandardTask(0, false);
         
         // 인벤토리 관련 퀘스트 이벤트 등록
         NoManualHotelManager.Instance.InventoryManager.GetInventoryItemTaskHandler -= TaskHandler.GetTaskCheckHandler;
         NoManualHotelManager.Instance.InventoryManager.GetInventoryItemTaskHandler += TaskHandler.GetTaskCheckHandler;
         NoManualHotelManager.Instance.InventoryManager.PutInventoryItemTaskHandler -= TaskHandler.PutTaskCheckHandler;
         NoManualHotelManager.Instance.InventoryManager.PutInventoryItemTaskHandler += TaskHandler.PutTaskCheckHandler;

         foreach (Transform child in gameObject.transform)
         {
            child.TryGetComponent(out HandoverStep step);
            _handoverStepQueue.Enqueue(step);
            step.Init(this);
         }

         StartNextStep();
      }
      
      /// <summary>
      /// 스팁 클리어 이벤트
      /// </summary>
      public void SendClearEvent(HandoverStep step)
      {
         if (step == _currentStep)
         {
            _currentStep.enabled = false;
            StartNextStep();
         }
      }

      [ContextMenu("강제 스텝 스킵")]
      /// <summary>
      /// 다음 스텝 시작
      /// </summary>
      private void StartNextStep()
      {
         if (_handoverStepQueue.Count > 0)
         {
            // 큐에서 다음 스텝 가져오기
            _currentStep = _handoverStepQueue.Dequeue();
            _currentStep.StartStep();
         }
         else
         {
            _currentStep = null;
         }
      }


      /// <summary>
      /// Handover 비디오 타임라인 실행
      /// </summary>
      public void PlayTimeLine(PlayableAsset timelineClip, Action timeLineEndEvent)
      {
          StartCoroutine(TimeLineCoroutine(timelineClip, timeLineEndEvent));
      }
      
      /// <summary>
      /// Handover 비디오 타임라인 코루틴
      /// </summary>
      private IEnumerator TimeLineCoroutine(PlayableAsset timelineClip, Action timeLineEndEvent)
      {
         // 타임라인 타이머 0으로 설정
         videoTimeLinePlayer.time = 0f;
         // 타임라인 재생
         videoTimeLinePlayer.Play(timelineClip);

         while (videoTimeLinePlayer.state is PlayState.Playing)
         {
            yield return null;
         }
         
         // 타임라인 종료 이벤트
         timeLineEndEvent?.Invoke();
      }
      

      private void OnDestroy()
      {
         // Handover 매니저 static 리셋
         SingletonReferenceNull();
         if (_handoverStepQueue.Count > 0)
         {
            foreach (var step in _handoverStepQueue)
            {
               step.RemoveEvent();
            }
         }
      }
   }
}

