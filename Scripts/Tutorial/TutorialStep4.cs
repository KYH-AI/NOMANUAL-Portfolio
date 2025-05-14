using System.Collections;
using System.Collections.Generic;
using NoManual.Interaction;
using NoManual.Managers;
using ThunderWire.Input;
using UnityEngine;
using UnityEngine.Events;

namespace NoManual.Tutorial
{
    public class TutorialStep4 : TutorialManager.TutorialNode
    {
        private List<TutorialManager.TutorialStepProcessNode> _tutorialStepProcessNodes;
        private int _step4TutorialIndex = 0;
        private readonly int _COMPLETE_RECEIPT_ID = 4023;
        
        public TutorialStep4(TutorialStep step, TutorialManager mono, InventoryManager inventory, ItemComponent medicalRegistrationReceipt, ItemComponent pen) : base(step)
        {
            // 인벤토리 아이템 Context 버튼 비활성화
            inventory.DisableContextMenu = true;
            
            // 단계별 튜토리얼 프로세스 등록
            _tutorialStepProcessNodes = new List<TutorialManager.TutorialStepProcessNode>(3)
            {
                new InteractionMedicalReceiptStep(medicalRegistrationReceipt, inventory),
                new InteractionPenStep(pen, inventory),
                new InventoryStep(mono),
                new CombineItemStep(mono, _COMPLETE_RECEIPT_ID, inventory)
            };
            
            HintTextKey = _tutorialStepProcessNodes[0].hintTableTextKey;
            // 튜토리얼 진행 딜레이
            TutorialProcessDelayTime = _tutorialStepProcessNodes[0].TutorialProcessDelayTime;
            // 가이드 텍스트 딜레이
            HintTextDelayTime = _tutorialStepProcessNodes[0].HintTextDelayTime;

        }
        
        public override void TutorialStartProcess()
        {
            _tutorialStepProcessNodes[0].TutorialStepStartProcess();
        }

        public override void TutorialUpdateProcess()
        {
            HandleHintTextDelay();
            if (!SendSubtitleTriggered)
            {
                SendSubtitle = true;
            }

            if (HandleTutorialProcessDelay())
            {
                if (_tutorialStepProcessNodes[_step4TutorialIndex].TutorialStepUpdateProcess())
                {
                    // 힌트 텍스트 닫기
                    NoManualHotelManager.Instance.UiNoManualUIManager.HideHintText(HintFadeHideSpeed);
                        
                    _step4TutorialIndex++;
                    // 모든 Step2 튜토리얼 모두 클리어
                    if (_step4TutorialIndex >= _tutorialStepProcessNodes.Count)
                    {
                        IsClear = true;
                        return;
                    }
                            
                    // 가이드 텍스트 출력 교체
                    HintTextKey = _tutorialStepProcessNodes[_step4TutorialIndex].hintTableTextKey;
                    SendHintTriggered = false;
                    SendHint = false;

                    // 튜토리얼 진행 딜레이
                    TutorialProcessDelayTimer = 0f;
                    TutorialProcessDelayTime = _tutorialStepProcessNodes[_step4TutorialIndex].TutorialProcessDelayTime;
                            
                    // 가이드 텍스트 딜레이 
                    HintTextDelayTime = _tutorialStepProcessNodes[_step4TutorialIndex].HintTextDelayTime;
                            
                    _tutorialStepProcessNodes[_step4TutorialIndex].TutorialStepStartProcess();
                }
            }
        }

        public override LocalizationTable.HintTableTextKey SendHintText()
        {
            HintTextPingPong = false;
            SendHintTriggered = true; // 더 이상 가이드 텍스트 출력 X 트리거 작동
            SendHint = false; // UI에게 가이드 텍스트 출력 완료
            return HintTextKey;
        }

        public override TutorialManager.NpcTalkMapper SendTitleText()
        {
            return NpcTalkMapper;
        }
    }
    
    #region Step4 튜토리얼 순차적 스텝
    
    /// <summary>
    /// 접수 작성용 종이 습득
    /// </summary>
    public class InteractionMedicalReceiptStep : TutorialManager.TutorialStepProcessNode
    {
        private ItemComponent _medicalRegistrationReceipt;
        private int _itemId;
        private bool _hasItem = false;
        
        public InteractionMedicalReceiptStep(ItemComponent medicalRegistrationReceipt, InventoryManager inventory)
        {
            this._medicalRegistrationReceipt = medicalRegistrationReceipt;
            this._itemId = _medicalRegistrationReceipt.InventoryItemId;
            hintTableTextKey = LocalizationTable.HintTableTextKey.TutorialHint_MedicalRegistrationReceipt;
            HintTextDelayTime = 5f;
            TutorialProcessDelayTime = HintTextDelayTime + 1f;
            // 아이템 체크 확인 이벤트 등록 (이벤트 작동 시 구독해체 알아서 진행)
            inventory.RegisterItemCheckAction(_itemId, () => _hasItem = true);
        }

        public override void TutorialStepStartProcess()
        {
            // 접수용 종이 아웃라인 등록
            Utils.OutLineUtil.AddOutLine(_medicalRegistrationReceipt.gameObject, QuickOutline.Mode.OutlineAll, Color.yellow, 2f);
        }

        public override bool TutorialStepUpdateProcess()
        {
            return _hasItem;
        }
    }
    
    /// <summary>
    /// 바닥에 떨어진 펜 습득 (앉기 튜토리얼)
    /// </summary>
    public class InteractionPenStep : TutorialManager.TutorialStepProcessNode
    {
        private ItemComponent _pen;
        private int _itemId;
        private bool _hasItem = false;
        
        public InteractionPenStep(ItemComponent pen, InventoryManager inventory)
        {
            this._pen = pen;
            this._itemId = _pen.InventoryItemId;
            // 상호작용 레이어 해체
            _pen.gameObject.layer = (int)Utils.Layer.LayerIndex.Default;
            hintTableTextKey = LocalizationTable.HintTableTextKey.TutorialHint_ItemPickUp;
            HintTextDelayTime = 2.5f;
            TutorialProcessDelayTime = HintTextDelayTime + 1f;
            // 아이템 체크 확인 이벤트 등록 (이벤트 작동 시 구독해체 알아서 진행)
            inventory.RegisterItemCheckAction(_itemId, () => _hasItem = true);
        }

        public override void TutorialStepStartProcess()
        {
            if (_pen.TryGetComponent(out Rigidbody penRigid))
            {
                penRigid.AddForce(_pen.transform.forward * 2.5f, ForceMode.Impulse);
            }
            // 펜 아웃라인 등록
            Utils.OutLineUtil.AddOutLine(_pen.gameObject, QuickOutline.Mode.OutlineAll, Color.yellow, 2f);
            // 상호작용 레이어 등록
            _pen.gameObject.layer = (int)Utils.Layer.LayerIndex.Interact;
        }
        
        public override bool TutorialStepUpdateProcess()
        {
            return _hasItem;
        }
    }

    /// <summary>
    /// 인벤토리 열기
    /// </summary>
    public class InventoryStep : TutorialManager.TutorialStepProcessNode
    {
        private MonoBehaviour _mono;
        
        public enum InventoryTutorialState
        {
            Closed,
            Opened,
        }
        // 튜토리얼 인벤토리 On / Off 상태확인
        private InventoryTutorialState _inventoryTutorialTutorialState = InventoryTutorialState.Closed;

        public InventoryStep(TutorialManager mono)
        {
            this._mono = mono;
            hintTableTextKey = LocalizationTable.HintTableTextKey.TutorialHint_Inventory;
            HintTextDelayTime = 2f;
            TutorialProcessDelayTime = HintTextDelayTime + 1f;
        }

        public override void TutorialStepStartProcess()
        {
            // 인벤토리 입력 UnLock
            InputHandler.InputActionLockControl(true, "Inventory");
        }

        public override bool TutorialStepUpdateProcess()
        {
            // 인벤토리 입력 대기 중
            if (InputHandler.ReadButtonOnce(_mono, "Inventory"))
            {
                switch (_inventoryTutorialTutorialState)
                {
                    case InventoryTutorialState.Closed:
                        _inventoryTutorialTutorialState = InventoryTutorialState.Opened;
                        break;
                    case InventoryTutorialState.Opened:
                        return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 접수 작성용 종이 및 펜 조합
    /// </summary>
    public class CombineItemStep : TutorialManager.TutorialStepProcessNode
    {
        private UnityAction _inventoryContextMenuEnableEvent;
        private int _itemId;
        private bool _hasItem = false;
        // 튜토리얼 인벤토리 On / Off 상태확인
        private InventoryStep.InventoryTutorialState _inventoryTutorialTutorialState = InventoryStep.InventoryTutorialState.Closed;
        private MonoBehaviour _mono;
        
        public CombineItemStep(MonoBehaviour mono, int itemId, InventoryManager inventory)
        {
            this._mono = mono;
            this._itemId = itemId;
            // 상호작용 레이어 해체
            hintTableTextKey = LocalizationTable.HintTableTextKey.TutorialHint_Combine;
            HintTextDelayTime = 2.5f;
            TutorialProcessDelayTime = HintTextDelayTime + 1f;
            // 아이템 체크 확인 이벤트 등록 (이벤트 작동 시 구독해체 알아서 진행)
            inventory.RegisterItemCheckAction(_itemId, () => _hasItem = true);
            _inventoryContextMenuEnableEvent = () => { inventory.DisableContextMenu = false; };
        }

        public override void TutorialStepStartProcess()
        {
            // 인벤토리 아이템 Context 버튼 활성화
            _inventoryContextMenuEnableEvent?.Invoke();
            _inventoryContextMenuEnableEvent = null;
        }

        public override bool TutorialStepUpdateProcess()
        {
            // 인벤토리 입력 대기 중
            if (InputHandler.ReadButtonOnce(_mono, "Inventory"))
            {
                switch (_inventoryTutorialTutorialState)
                {
                    case InventoryStep.InventoryTutorialState.Closed:
                        _inventoryTutorialTutorialState = InventoryStep.InventoryTutorialState.Opened;
                        break;
                    case InventoryStep.InventoryTutorialState.Opened:
                        return _hasItem;
                }
            }
            return false;
        }
    }
    
    #endregion
}

