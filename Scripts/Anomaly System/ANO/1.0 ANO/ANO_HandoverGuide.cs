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
        /// (�߻�ȭ) ANO ��ȣ�ۿ�
        /// </summary>
        public override void Interact()
        {
            GameObject sticker = Instantiate(anoStickerPrefab, interactANO_StickerPoint, Quaternion.identity);
            
            // hit.Normal �� �������� ��ƼĿ�� �ٶ󺸰� �ϱ�
            sticker.transform.forward = interactANO_StickerNormal;
            sticker.transform.parent = this.gameObject.transform;
            
            // ANO ��ƼĿ ó�� �Ϸ�
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
        /// �̻����� ���� ����
        /// </summary>
        public void PlayAno()
        {
            anoRigidbody.AddForce(new Vector3(5f, 0f, 0f), ForceMode.Impulse);
        }
    }
}


