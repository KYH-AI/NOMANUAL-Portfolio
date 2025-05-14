using UnityEngine;
using Michsky.DreamOS;
using NoManual.Managers;

namespace NoManual.Interaction
{
    public class PC_OS_Component : InteractionBase
    {
        [SerializeField] private WorldSpaceManager pcOsManager;

        private void Awake()
        {
            // 카메라 전환 시간
            pcOsManager.transitionTime = 1f;
            pcOsManager.onExitEnd.AddListener(ExitEndOS);
            interactionType = InteractionType.PC;
        }
        
        /// <summary>
        /// PC OS 진입
        /// </summary>
        private void EnterOS()
        {
            NoManualHotelManager.Instance.CutSceneManager.CutSceneRunning = true;
            NoManualHotelManager.Instance.DisablePlayer();
            pcOsManager.GetIn();
        }

        /// <summary>
        /// PC OS 탈출 확인
        /// </summary>
        protected virtual void ExitEndOS()
        {
            NoManualHotelManager.Instance.CutSceneManager.CutSceneRunning = false;
            // 보고 완료 후 컷신 진행
            if (HotelManager.Instance.IsPlayReportCutScene)
            {
                HotelManager.Instance.ReportCutScene.PlayReportCutSceneStartEvent();
                return;
            }
            NoManualHotelManager.Instance.EnablePlayer();
        }

        /// <summary>
        /// (추상화) PC OS 상호작용
        /// </summary>
        public override void Interact()
        {
            if(!pcOsManager.isInSystem) EnterOS();
        }

        /// <summary>
        /// (추상화) PC OS RayCast 확인
        /// </summary>
        public override bool InteractRayCast()
        {
            return !pcOsManager.isInSystem;
        }
    }
}


