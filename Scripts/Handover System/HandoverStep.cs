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
        /// ���� �ʱ�ȭ
        /// </summary>
        public virtual void Init(HandoverManager handoverManager)
        {
            this._StepClearEvent -= handoverManager.SendClearEvent;
            this._StepClearEvent += handoverManager.SendClearEvent;
            this.HandoverManager = handoverManager;
        }


        /// <summary>
        /// ���� ����
        /// </summary>
        public abstract void StartStep();
        
        /// <summary>
        /// ���� Ʈ����
        /// </summary>
        protected abstract void FirstStartTriggerStep();

        /// <summary>
        /// ���� ����
        /// </summary>
        protected abstract void EndStep();
        
        /// <summary>
        /// ���� �ݶ��̴� Ʈ���� ���� 
        /// </summary>
        public abstract void OnTriggerEvent(Collider other);

        public void RemoveEvent()
        {
            this._StepClearEvent = null;
        }
    }
}
