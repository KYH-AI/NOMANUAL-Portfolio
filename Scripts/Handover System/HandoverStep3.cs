using NoManual.ANO;
using NoManual.Interaction;
using NoManual.Managers;
using NoManual.Task;
using UnityEngine;
using UnityEngine.Playables;

namespace NoManual.Tutorial
{
    public class HandoverStep3 : HandoverStep
    {
        [Header("Handover Ÿ�Ӷ���")] 
        [SerializeField] private PlayableAsset handover2VideoTimeline;

        [Space(10)] 
        [Header("Jazz �����")] 
        [SerializeField] private AudioSource jazzAudio;
        
        [Space(10)]
        [Header("�� ������Ʈ")]
        [SerializeField] private DoorComponent fDoor;
        [SerializeField] private DoorComponent gDoor;
        [SerializeField] private DoorComponent hDoor;

        [Space(10)] 
        [Header("�ݶ��̴� ������Ʈ")]
        [SerializeField] private Collider collider4;
        [SerializeField] private Collider collider5;
        [SerializeField] private Collider collider6;

        [Space(10)]
        [Header("ANO ������Ʈ")]
        [SerializeField] private ANO_HandoverGuide anoComponent;
 
        public override void Init(HandoverManager handoverManager)
        {
            base.Init(handoverManager);

            // �� �ʱ�ȭ
            fDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            gDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            hDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
        }
        
        public override void StartStep()
        {
            // �̻����� �ʱ�ȭ
            anoComponent.InitializeANO(new ANO_CloneData(9999, ANOSolve.SimpleSolve, -1, null, false, 0));
            anoComponent.AnoClearEvent1 -= HandoverManager.TaskHandler.BonusTaskCheckHandler;
            anoComponent.AnoClearEvent1 += HandoverManager.TaskHandler.BonusTaskCheckHandler;
            anoComponent.AnoClearEvent2 -= EndStep;
            anoComponent.AnoClearEvent2 += EndStep;
            
            HandoverManager.TaskHandler.SetRunTimeBonusTaskData(TaskHandler.TaskType.Interaction, anoComponent.AnoCloneData.ANO_Id.ToString());
            fDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Close);
        }
        
        protected override void FirstStartTriggerStep()
        {
            fDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            HandoverManager.PlayTimeLine(handover2VideoTimeline, OnVideoEnd);
        }

        private void Collider5Trigger()
        {
            jazzAudio.Stop();
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(LocalizationTable.TextTable.Hint_Table, LocalizationTable.HintTableTextKey.HandoverHint_ + "8");
        }

        private void Collider6Trigger()
        {
            anoComponent.PlayAno();
        }
        
        /// <summary>
        /// handover ���� ���� �̺�Ʈ
        /// </summary>
        private void OnVideoEnd()
        {
            // g �� ����
            gDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Open);
            jazzAudio.Play();
        }
        
        protected override void EndStep()
        {
            hDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Open);
            
            // HandoverHint_Next ǥ��
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(
                LocalizationTable.TextTable.Hint_Table,
                LocalizationTable.HintTableTextKey.HandoverHint_Next.ToString());

            _StepClearEvent?.Invoke(this);
            RemoveEvent();
        }

        public override void OnTriggerEvent(Collider other)
        {
            if (other == collider4)
            {
                collider4.enabled = false;
                FirstStartTriggerStep();
            }
            else if (other == collider5)
            {
                collider5.enabled = false;
                Collider5Trigger();
            }
            else if (other == collider6)
            {
                collider6.enabled = false;
                Collider6Trigger();
            }
        }
    }
}
