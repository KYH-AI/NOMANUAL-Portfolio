using System;
using System.Collections;
using System.Collections.Generic;
using NoManual.Interaction;
using UnityEngine;

namespace NoManual.Interaction
{
    /// <summary>
    /// Put Task 이벤트 상호작용 컴포넌트 (Put Task와 관련된 퀘스트가 없으면 상호작용 되지 않는다)
    /// </summary>
    public abstract class PutBaseComponent : InteractionBase
    {
        [Header("상호작용 아이콘 위치 (생략 가능)")]
        [SerializeField] private Transform floatingIconRoot;

        protected HighLightIcon highLightIcon;
        
        public IPut putInterface { get; private set; }
        
        #region Put 관련 데이터 
        
        public string TaskTargetId = string.Empty;
        public IPut.PutMode CurrentPutMode = IPut.PutMode.None;
        public int[] RequestItemId { get; set; }
        // Put 성공 시 아이템 제거 유무
        protected bool RemoveRequestItem { get; } = true;
        // 1회 Put 성공 확인
        public bool PutComplete { get; protected set; } = false; 
        
        #endregion 
        
        public void Awake()
        {
            putInterface = InitializationPutInterface();
            interactionType = InteractionType.Put;
            highLightIcon = GetComponentInChildren<HighLightIcon>();
        }

        public void InitializationPutMode()
        {
            if (putInterface == null) putInterface = InitializationPutInterface();
            putInterface.InitializationPutMode();
        }

        /// <summary>
        /// Put 인터페이스 초기화
        /// </summary>
        protected abstract IPut InitializationPutInterface();
        
        public override void Interact()
        {
            if (putInterface != null)
            {
               if( putInterface.GetMode is IPut.PutMode.Put) putInterface?.PutInteraction();
            }
        }
        
        public override GameObject GetAnotherInteractionObject()
        {
            return floatingIconRoot ? floatingIconRoot.gameObject : base.GetAnotherInteractionObject();
        }
    }
}




