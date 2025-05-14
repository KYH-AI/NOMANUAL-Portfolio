using System;
using System.Collections;
using System.Collections.Generic;
using NoManual.ANO;
using NoManual.Task;
using UnityEngine;

namespace NoManual.Tutorial
{
    public class ANO_HandoverGuide : ANO_Component
    {
        public event Action<TaskHandler.TaskType, string> AnoClearEvent1;
        public event Action AnoClearEvent2;

        [SerializeField] private GameObject anoStickerPrefab;
        [SerializeField] private Rigidbody anoRigidbody;

        /// <summary>
        /// (추상화) ANO 상호작용
        /// </summary>
        public override void Interact()
        {
            GameObject sticker = Instantiate(anoStickerPrefab, interactANO_StickerPoint, Quaternion.identity);
            
            // hit.Normal 값 기준으로 스티커를 바라보게 하기
            sticker.transform.forward = interactANO_StickerNormal;
            sticker.transform.parent = this.gameObject.transform;
            
            // ANO 스티커 처리 완료
            ANO_Clear(true);
        }
        
        public override void ANO_Clear(bool isClearANO)
        {
            IsClear = isClearANO;
            AnoClearEvent1?.Invoke(TaskHandler.TaskType.Interaction,  AnoCloneData.ANO_Id.ToString());
            ANO_ClearAction();
            SetNoInteractive();
        }
        
        protected override void ANO_ClearAction()
        {
            AnoClearEvent2?.Invoke();

            AnoClearEvent1 = null;
            AnoClearEvent2 = null;
        }

        /// <summary>
        /// 이상현상 연출 실행
        /// </summary>
        public void PlayAno()
        {
            anoRigidbody.AddForce(new Vector3(5f, 0f, 0f), ForceMode.Impulse);
        }
    }
}


