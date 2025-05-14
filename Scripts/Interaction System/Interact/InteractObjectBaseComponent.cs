using System;
using System.Collections;
using System.Collections.Generic;
using NoManual.Task;
using UnityEngine;
using UnityEngine.Events;

namespace NoManual.Interaction
{
    public abstract class InteractObjectBaseComponent : InteractionBase
    {
        public enum InteractionMode
        {
            Idle = 0,
            TaskIdle = 1,
        }

        [SerializeField] protected UnityEvent OnStartEvent;
        [SerializeField] protected UnityEvent OnEndEvent;
        
        [Space(5)]
        [Header("고유 ID")] [SerializeField] private string targetTaskId = string.Empty;
        public string GetTargetTaskId => targetTaskId;
        
        [Space(2)]
        [Header("상호작용 아이콘 출력 위치")] [SerializeField] protected Transform floatingIconRoot;
        
        private InteractionMode _currentInteractionMode = InteractionMode.Idle;
        private const TaskHandler.TaskID _TASK_ID_KEY = TaskHandler.TaskID.Interact_Object;
        private event TaskHandler.TaskEventHandler _interactEvent;
        
        [SerializeField] private HighLightIcon _highLightIcon;

        /// <summary>
        /// 오브젝트 가이드 라인 캐싱
        /// </summary>
        public void InitHighLightIcon()
        {
            _highLightIcon = GetComponentInChildren<HighLightIcon>();
        }
        

        [ContextMenu("Init")]
        /// <summary>
        /// Interact Idle Mode 오브젝트 초기화 (라운드 시작 시 호출)
        /// </summary>
        public void InitializationInteractObject()
        {
            _interactEvent = null;
            SetInteractionMode(InteractionMode.Idle);
            InitInteractObject();
        }
        

        /// <summary>
        /// Task Idle Mode (라운드 시작 시 호출)
        /// </summary>
        public void TaskIdleMode(TaskHandler.TaskEventHandler interactCallBackEvent)
        {
            _interactEvent = null;
            _interactEvent = interactCallBackEvent;  // 이벤트를 재설정
            SetInteractionMode(InteractionMode.TaskIdle);  // 모드 전환
            SetIdle_To_TaskIdle();
        }
        
        [ContextMenu("Task Idle Mode (디버깅용)")]
        public void TaskIdleMode()
        {
            SetInteractionMode(InteractionMode.TaskIdle);  // 모드 전환
            SetIdle_To_TaskIdle();
        }

        /// <summary>
        /// Idle Mode
        /// </summary>
        private void IdleMode()
        {
            _interactEvent = null;
            SetInteractionMode(InteractionMode.Idle);  // 모드 전환
            SetTaskIdle_To_Idle();
        }

        public override void Interact()
        {
            _interactEvent?.Invoke(_TASK_ID_KEY.ToString(), GetTargetTaskId);  // 이벤트가 설정된 경우 실행
            IdleMode(); // 모드 전환
        }

        protected void SetInteractionMode(InteractionMode mode)
        {
            _currentInteractionMode = mode;  // 모드 설정
            if (mode == InteractionMode.TaskIdle)
            {
                if(_highLightIcon) _highLightIcon.SetItemHighLight();
                SetYesInteractive();  // 상호작용 가능
            }
            else
            {
                if(_highLightIcon) _highLightIcon.HideItemHighLight();
                SetNoInteractive();  // 상호작용 불가
            }
        }

        /// <summary>
        /// 초기화
        /// </summary>
        protected abstract void InitInteractObject();
        /// <summary>
        /// Idle -> Task Idle
        /// </summary>
        protected abstract void SetIdle_To_TaskIdle();
        /// <summary>
        /// Task Idle -> Idle
        /// </summary>
        protected abstract void SetTaskIdle_To_Idle();

        private void OnDestroy()
        {
            _interactEvent = null;
        }
    }
}


