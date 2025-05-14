using System;
using System.Collections;
using System.Collections.Generic;
using NoManual.CutScene;
using UnityEngine;
using NoManual.Managers;
using ThunderWire.Utility;
using TMPro;

namespace NoManual.Interaction
{
    public class ReportingDeskTopPC_Component : InteractionBase
    {
        // 보고 진행중 확인
        public bool IsReporting { get; private set; } = false;

        [Header("경비문")]
        [SerializeField] private DoorComponent securityDoor;
        [Header("출구문")] 
        [SerializeField] private ExitDoor exitDoor;
        [Header("전광판 라운드 텍스트")]
        [SerializeField] private TextMeshProUGUI counterClockText;
        [Header("가짜 전광판")] 
        [SerializeField] private GameObject fakeCounterClock;
        [Header("경비실 매뉴얼 책")] 
        [SerializeField] private ItemComponent mainGuidBook;
        [Header("경비실 손전등")] 
        [SerializeField] private ItemComponent flashLight;
        [Header("튜토리얼")]
        [SerializeField] private HotelLobbyTutorial tutorial;

        [Serializable]
        public struct Reporting_SFX_Position
        {
            [Header("Clock 효과음 위치")]
            public Transform tickTocSoundPos;
            [Header("보고 검사 효과음 위치")]
            public Transform counterClockSoundPos;
            [Header("PC 클릭 효과음 위치")]
            public Transform pcClickSoundPos;
        }

        [Serializable]
        public struct Reporting_SFX_List
        {
            public AudioClip tickTokSFX;
            public AudioClip PC_ClickSFX;
            public AudioClip ReportingCorrectSFX;
            public AudioClip ReportingInCorrectSFX;
            public AudioClip LastReportingInCorrectSFX;
        }

        [Header("보고 효과음 재생 위치")]
        public Reporting_SFX_Position reportingSfxPosition = new Reporting_SFX_Position();
        [Header("보고 효과음")]
        public Reporting_SFX_List reportingSfxList = new Reporting_SFX_List();
        [Header("검사 딜레이")] 
        [SerializeField] private float reportingDelay;
        
        public void Load()
        {
            if (HotelManager.Instance.TutorialClear)
            {
                fakeCounterClock.SetActive(false);
            }
            if (HotelManager.Instance.gameRound.IsGameClear)
            {
                SetExitDoorLayer((int)Utils.Layer.LayerIndex.Interact);
            }
            SetCounterClock(HotelManager.Instance.gameRound.CurrentRoundCount);
        }

        /// <summary>
        /// 경비실 상호작용 오브젝트 아웃라인 표시
        /// </summary>
        public void SetOutLine()
        {
            // 보고용 PC 아웃라인 표시
            Utils.OutLineUtil.AddOutLine(this.gameObject, QuickOutline.Mode.OutlineAll, Color.yellow, 1.5f);
            // 매뉴얼 북 아웃라인 표시
            Utils.OutLineUtil.AddOutLine(mainGuidBook.gameObject, QuickOutline.Mode.OutlineAll, Color.yellow, 1.5f);
            // 손전등 아웃라인 표시
            Utils.OutLineUtil.AddOutLine(flashLight.gameObject, QuickOutline.Mode.OutlineAll, Color.yellow, 1.5f);
        }

        /// <summary>
        /// PC 상호작용
        /// </summary>
        public void InteractPC()
        {
            // 출근 보고 진행
            if (HotelManager.Instance.gameRound.CurrentRoundCount == (HotelManager.Instance.gameRound.gameRoundSettings.minRound))
            {
                GetToWork();
            }
            // 경비실 문이 잠겨있으면 보고 진행
            else if (securityDoor.doorState == DoorComponent.DoorStateEnum.Close)
            {
                Reporting();
            }
            // 아니면 UI 호출
            else
            {
                NoManualHotelManager.Instance.UiNoManualUIManager
                    .ShowHintText("~~~" , true, 2f, 2f); //UI.TextTip.SecurityDoorNeedLock);
            }
        }
        
        /// <summary>
        /// 출근 보고
        /// </summary>
        private void GetToWork()
        {
            // 첫 라운드에서만 출근 보고가 가능
            if (HotelManager.Instance.gameRound.CurrentRoundCount != HotelManager.Instance.gameRound.gameRoundSettings.minRound) return;
            
            if (!HotelManager.Instance.TutorialClear)
            {
                // 튜토리얼 스킵
                tutorial.StopHotelLobbyTutorial(false);
            }
            
            // PC 클릭 효과음
            Utilities.PlayOneShot3D(reportingSfxPosition.pcClickSoundPos.position, OptionHandler.AudioMixerChanel.SFX, reportingSfxList.PC_ClickSFX);

            // 초기 선별 기준으로 ANO 배치
            if (HotelManager.Instance.ANO.ANO_Replace(ANO_Manager.ANO_ReplaceType.FirstRound))
            {
                // 보고 효과음 재생
                SetCounterClock(HotelManager.Instance.gameRound.CurrentRoundCount);
                counterClockText.gameObject.SetActive(true);
                Utilities.PlayOneShot3D(reportingSfxPosition.counterClockSoundPos.position, OptionHandler.AudioMixerChanel.SFX, reportingSfxList.ReportingCorrectSFX, 0.5f, 1f, 2f);
            }

            // 가짜 전광판 비활성화
            fakeCounterClock.SetActive(false);
            
            // 경비실 오브젝트 아웃라인 제거
            if ((bool)gameObject && gameObject.TryGetComponent(out QuickOutline deskTopOutline))
            {
                Destroy(deskTopOutline);
            }
            if ((bool)mainGuidBook && mainGuidBook.TryGetComponent(out QuickOutline guideBookOutline))
            {
                Destroy(guideBookOutline);
            }
            if ((bool)flashLight && flashLight.TryGetComponent(out QuickOutline flashLightOutline))
            {
                Destroy(flashLightOutline);
            }
            
        }

        /// <summary>
        /// 전광판 라운드 텍스트 설정
        /// </summary>
        private void SetCounterClock(int round)
        {
            counterClockText.text = round.ToString();
        }

        /// <summary>
        /// 출구문 상호작용 가능한 레이어로 변경
        /// </summary>
        private void SetExitDoorLayer(int layerIndex)
        {
            Utils.OutLineUtil.AddOutLine(exitDoor.gameObject, QuickOutline.Mode.OutlineAll, Color.white, 1.5f);
            exitDoor.gameObject.layer = layerIndex;
        }

        /// <summary>
        /// 보고 시스템 상호작용
        /// </summary>
        [ContextMenu("보고 하기")]
        private void Reporting()
        {
            // 보고 시작 시 문 잠금
            securityDoor.ToggleDoorLock();
            IsReporting = true;

            // PC 클릭 효과음 실행
            Utilities.PlayOneShot3D(reportingSfxPosition.pcClickSoundPos.position, OptionHandler.AudioMixerChanel.SFX, reportingSfxList.PC_ClickSFX);

            // TODO: UI 활성화 & Lock Script
            // 보고 진행
            StartCoroutine(ReportingProcess(result =>
            {
                if (result)
                {
                    // 보고 성공 처리
                    //Debug.Log("보고 성공!");
                    
                    // 전광판 효과음
                    AudioClip counterClockSFX = null;
                    // 전광판 텍스트
                    string counterText = HotelManager.Instance.gameRound.CurrentRoundCount.ToString();
                    
                    // 게임 클리어 시
                    if (HotelManager.Instance.gameRound.IsGameClear)
                    {
                        SetExitDoorLayer((int)Utils.Layer.LayerIndex.Interact);
                        counterText = "- - -";
                        counterClockSFX = reportingSfxList.LastReportingInCorrectSFX;
                    }
                    else if (HotelManager.Instance.ANO.isAllAnoClear)
                    {
                        counterClockSFX = reportingSfxList.ReportingCorrectSFX;
                    }
                    else
                    {
                        counterClockSFX = reportingSfxList.ReportingInCorrectSFX;
                    }
                    // 보고 결과 효과음
                    Utilities.PlayOneShot3D(reportingSfxPosition.counterClockSoundPos.position, OptionHandler.AudioMixerChanel.SFX,  counterClockSFX, 1f, 1f, 2f);
                    // 전광판 라운드 텍스트 변경
                    counterClockText.text = counterText;
                }
                else
                {
                    // 보고 실패 처리
                    Debug.LogError("보고 실패.");
                }

                // 보고 처리 후 문 잠금 해제
                securityDoor.ToggleDoorLock();
                
                // 아직 라운드가 남아있으면 보고 상호작용 계속 가능
                IsReporting = HotelManager.Instance.gameRound.IsGameClear;
            }));
        }

        /// <summary>
        /// 보고 코루틴
        /// </summary>
        private IEnumerator ReportingProcess(Action<bool> callback)
        {
            const int maxAttempts = 5; // 최대 시도 횟수
            int attempts = 0; // 현재 시도 횟수
            bool reportResults = false;

            while (attempts < maxAttempts && ! HotelManager.Instance.ANO.Reporting())
            {
                attempts++;
                yield return new WaitForSeconds(1); // 1초 대기
            }

            reportResults = attempts < maxAttempts;

            // 검사 효과음 실행
            Utilities.PlayOneShot3D(reportingSfxPosition.tickTocSoundPos.position, OptionHandler.AudioMixerChanel.SFX, reportingSfxList.tickTokSFX, 0.5f, 1f, 3f);
            // 검사 딜레이
            yield return new WaitForSeconds(reportingDelay);
            callback?.Invoke(reportResults);
        }

        /// <summary>
        /// (추상화) PC 상호작용
        /// </summary>
        public override void Interact()
        {
            InteractPC();
        }

        /// <summary>
        /// (추상화) PC RayCast 확인
        /// </summary>
        public override bool InteractRayCast()
        {
            return !IsReporting;
        }
    }
}


