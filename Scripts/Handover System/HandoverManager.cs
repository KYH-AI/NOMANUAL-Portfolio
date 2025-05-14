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
      [Header("���� Ÿ�Ӷ���")]
      [SerializeField] private PlayableDirector videoTimeLinePlayer;
      
      [Header("�μ��ΰ� ���� ������Ʈ")]
      private Queue<HandoverStep> _handoverStepQueue;

      private HandoverStep _currentStep;

      private void Awake()
      {
         StartCoroutine(InitializedProcess());
      }
        
      private void Start()
      {
         // ���� ���� Fade Out
         NoManualHotelManager.Instance.PlayerStartFade(false, 15f, true, 0.45f);
      }
      
      private IEnumerator InitializedProcess()
      {
         // NoManualHotelManager�� �ν��Ͻ��� ��� ���� ���
         while (NoManualHotelManager.Instance == null || !NoManualHotelManager.Instance.OnInitialized)
         {
            yield return null;  // ���� �����ӱ��� ���
         }
         Initialization();  // �ʱ�ȭ ����
      }
      
      private void Initialization()
      {
         _handoverStepQueue = new Queue<HandoverStep>(5);
         TaskHandler = new TaskHandler();
         TaskHandler.TaskHandlerInitialization(pcOSManager.UpdateRecordItem_Handover, false);
         TaskHandler.SetNewDayStandardTask(0, false);
         
         // �κ��丮 ���� ����Ʈ �̺�Ʈ ���
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
      /// ���� Ŭ���� �̺�Ʈ
      /// </summary>
      public void SendClearEvent(HandoverStep step)
      {
         if (step == _currentStep)
         {
            _currentStep.enabled = false;
            StartNextStep();
         }
      }

      [ContextMenu("���� ���� ��ŵ")]
      /// <summary>
      /// ���� ���� ����
      /// </summary>
      private void StartNextStep()
      {
         if (_handoverStepQueue.Count > 0)
         {
            // ť���� ���� ���� ��������
            _currentStep = _handoverStepQueue.Dequeue();
            _currentStep.StartStep();
         }
         else
         {
            _currentStep = null;
         }
      }


      /// <summary>
      /// Handover ���� Ÿ�Ӷ��� ����
      /// </summary>
      public void PlayTimeLine(PlayableAsset timelineClip, Action timeLineEndEvent)
      {
          StartCoroutine(TimeLineCoroutine(timelineClip, timeLineEndEvent));
      }
      
      /// <summary>
      /// Handover ���� Ÿ�Ӷ��� �ڷ�ƾ
      /// </summary>
      private IEnumerator TimeLineCoroutine(PlayableAsset timelineClip, Action timeLineEndEvent)
      {
         // Ÿ�Ӷ��� Ÿ�̸� 0���� ����
         videoTimeLinePlayer.time = 0f;
         // Ÿ�Ӷ��� ���
         videoTimeLinePlayer.Play(timelineClip);

         while (videoTimeLinePlayer.state is PlayState.Playing)
         {
            yield return null;
         }
         
         // Ÿ�Ӷ��� ���� �̺�Ʈ
         timeLineEndEvent?.Invoke();
      }
      

      private void OnDestroy()
      {
         // Handover �Ŵ��� static ����
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

