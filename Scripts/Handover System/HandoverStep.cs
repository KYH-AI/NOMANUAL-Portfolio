using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoManual.Tutorial
{
    public abstract class HandoverStep : MonoBehaviour
    {
        protected HandoverManager HandoverManager;
        protected HandoverManager.StepClearEvent _StepClearEvent;

        /// <summary>
        /// 스텝 초기화
        /// </summary>
        public virtual void Init(HandoverManager handoverManager)
        {
            this._StepClearEvent -= handoverManager.SendClearEvent;
            this._StepClearEvent += handoverManager.SendClearEvent;
            this.HandoverManager = handoverManager;
        }


        /// <summary>
        /// 스텝 시작
        /// </summary>
        public abstract void StartStep();
        
        /// <summary>
        /// 스텝 트리거
        /// </summary>
        protected abstract void FirstStartTriggerStep();

        /// <summary>
        /// 스텝 종료
        /// </summary>
        protected abstract void EndStep();
        
        /// <summary>
        /// 스텝 콜라이더 트리거 감지 
        /// </summary>
        public abstract void OnTriggerEvent(Collider other);

        public void RemoveEvent()
        {
            this._StepClearEvent = null;
        }
    }
}
