using System.Collections;
using System.Collections.Generic;
using NoManual.Interaction;
using NoManual.Managers;
using UnityEngine;
using TMPro;
    

namespace NoManual.Tutorial
{
    /// <summary>
    /// 진료실 문 상호작용 
    /// </summary>
    public class TutorialStep5 : TutorialManager.TutorialNode
    {
        private TextMeshProUGUI _monitorText;
        private DoorComponent _treatmentRoomDoor;
        private AudioSource _monitorAudio;
        // 진료실 연출 1회 트리거
        private bool _readyMedicalOffice = true;

        public TutorialStep5(TutorialStep step, DoorComponent doorComponent, AudioSource monitorAudio, TextMeshProUGUI monitorText) : base(step)
        {
            this._monitorAudio = monitorAudio;
            this._monitorText = monitorText;
            this._treatmentRoomDoor = doorComponent;

            // 상호작용 예외처리
            _treatmentRoomDoor.gameObject.layer = (int)Utils.Layer.LayerIndex.Default;
            
            HintTextDelayTime = 2f;
            TutorialProcessDelayTimer = 0f;
            TutorialProcessDelayTime = HintTextDelayTime + 5f;
        }

        public override void TutorialUpdateProcess()
        {
            HandleHintTextDelay();
            if (HandleTutorialProcessDelay())
            {
                // 접수 모니터 39표시, 알람음 
                if (_readyMedicalOffice)
                {
                    _treatmentRoomDoor.gameObject.layer = (int)Utils.Layer.LayerIndex.Interact;
                    // 아웃라인 생성
                    Utils.OutLineUtil.AddOutLine(_treatmentRoomDoor.gameObject, QuickOutline.Mode.OutlineAll, Color.yellow, 2.5f);
                    _monitorText.text = "39";
                    _monitorAudio.Play();
                    _readyMedicalOffice = false;
                }
                
                // 진료실 문과 상호작용 완료 시 의사면담 진행
                if (_treatmentRoomDoor.doorState is DoorComponent.DoorStateEnum.Open)
                {
                    // 아웃라인 제거
                    Utils.OutLineUtil.RemoveOutLine(_treatmentRoomDoor.gameObject);
                    IsClear = true;
                    return;
                }
            }
        }

        public override LocalizationTable.HintTableTextKey SendHintText()
        {
            HintTextKey = LocalizationTable.HintTableTextKey.TutorialHint_Completed;
            HintFadeDuration = 5f;
            HintTextPingPong = true;
            SendHintTriggered = true; // 더 이상 가이드 텍스트 출력 X 트리거 작동
            SendHint = false; // UI에게 가이드 텍스트 출력 완료
            return HintTextKey;
        }

        public override TutorialManager.NpcTalkMapper SendTitleText()
        {
            return NpcTalkMapper;
        }
    }
}

