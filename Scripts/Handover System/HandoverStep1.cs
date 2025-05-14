using NoManual.Interaction;
using NoManual.Managers;
using ThunderWire.Input;
using UnityEngine;
using UnityEngine.Playables;

namespace NoManual.Tutorial
{
    public class HandoverStep1 : HandoverStep
    {
        [SerializeField] private PlayableAsset handover0VideoTimeline;
        [SerializeField] private DoorComponent aDoor;
        [SerializeField] private DoorComponent bDoor;
        [SerializeField] private GameObject checkList;
        [SerializeField] private Collider collider0;
        
        private InventoryStep.InventoryTutorialState _inventoryTutorialTutorialState = InventoryStep.InventoryTutorialState.Closed;
        private int _checkListItemId;
        private bool _hasItem = false;
        
        public override void Init(HandoverManager handoverManager)
        {
            base.Init(handoverManager);
            
            // 문 초기화
            bDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            _checkListItemId = checkList.GetComponent<ItemComponent>().inventoryItem.itemId;
        }
        
        public override void StartStep()
        {
            aDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Close);
        }
        
        protected override void FirstStartTriggerStep()
        {
            // 체크리스트 오브젝트 상호작용 금지
            checkList.GetComponent<ItemComponent>().SetNoInteractive();
            
            // a 문 잠금
            aDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Lock);
            
            // 아이템 체크 확인 이벤트 등록 (이벤트 작동 시 구독해체 알아서 진행)
            NoManualHotelManager.Instance.InventoryManager.RegisterItemCheckAction(_checkListItemId, () => _hasItem = true);
            
            // TV 타임라인 실행
            HandoverManager.PlayTimeLine(handover0VideoTimeline, OnVideoEnd);
        }

        private void Update()
        {
            if(InputHandler.ReadButtonOnce(this, "Inventory"))
            {
                switch (_inventoryTutorialTutorialState)
                {
                    case InventoryStep.InventoryTutorialState.Closed:
                        _inventoryTutorialTutorialState = InventoryStep.InventoryTutorialState.Opened;
                        break;
                    case InventoryStep.InventoryTutorialState.Opened:
                        if(_hasItem) EndStep(); // 배낭에 체크리스트가 존재하면 스텝 1 종료
                        _inventoryTutorialTutorialState = InventoryStep.InventoryTutorialState.Closed;
                        break;
                }
            }
        }

        /// <summary>
        /// 체크리스트 UI를 닫을 때 진행
        /// </summary>
        protected override void EndStep()
        {
            // b 문 열기
            bDoor.DoorHandler(DoorBaseComponent.DoorStateEnum.Open);
            
            // HandoverHint_Next 표시
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(LocalizationTable.TextTable.Hint_Table, LocalizationTable.HintTableTextKey.HandoverHint_Next.ToString());
            
            _StepClearEvent?.Invoke(this);
            RemoveEvent();
        }

        /// <summary>
        /// 비디오 종료 이벤트
        /// </summary>
        private void OnVideoEnd()
        {
            // 체크리스트 오브젝트 아웃라인 활성화
            Utils.OutLineUtil.AddOutLine(checkList, QuickOutline.Mode.OutlineAll, Color.yellow, 1.5f);
            // 체크리스트 오브젝트 상호작용 활성화
            checkList.GetComponent<ItemComponent>().SetYesInteractive();
            // HandoverHint0 표시
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowLeftTopGuideText(LocalizationTable.TextTable.Hint_Table, LocalizationTable.HintTableTextKey.HandoverHint_ + "0");
        }

        /// <summary>
        /// Step1 트리거
        /// </summary>
        public override void OnTriggerEvent(Collider other)
        {
            if (other == collider0)
            {
                collider0.enabled = false;
                FirstStartTriggerStep();
            }
        }
    }
}

