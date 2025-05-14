using NoManual.Managers;
using UnityEngine;

namespace NoManual.Interaction
{
    /// <summary>
    /// 퇴근 문 상호작용 컴포넌트
    /// </summary>
    public class ExitDoor : DoorBaseComponent
    {
        [Header("이동할 씬 (기본 값 : Monologue)")] 
        [SerializeField] private GameManager.SceneName targetScene = GameManager.SceneName.Monologue;
        [Header("로딩 씬 필요 (기본 값 : false)")] 
        [SerializeField] private bool needLoading = false;
        [Header("세이브 파일 부분 저장 필요 (기본 값 : true)")]
        [SerializeField] private bool needSave = true;
        [Header("세이브 파일 모든 저장 필요 (기본 값 : true)")]
        [SerializeField] private bool allSaveFile = true;
        
        /// <summary>
        /// 출구문 상호작용
        /// </summary>
        protected override void DoorComponentInteract()
        {
            Utils.OutLineUtil.RemoveOutLine(this.gameObject);
            SetNoInteractive();

            if (needLoading)
            {
                // 로딩 씬이 필요한 경우, 타겟 씬 설정
                GameManager.Instance.NextScene = targetScene;
                
                // 퇴근 시 세이브 파일 저장이 필요한 경우
                if (needSave)
                {
                    // 세이브 파일 저장이 필요한 경우, 호텔 종료 및 로딩 씬 이동
                    HotelManager.Instance.ExitHotel(needSave, allSaveFile, GameManager.SceneName.Loading);
                }
                else
                {
                    // 세이브가 필요하지 않은 경우, 호텔로 바로 이동 (Handover 전용)
                    NoManualHotelManager.Instance.SceneMove(false, false, GameManager.SceneName.Loading);
                }
            }
            else
            {
                if (needSave)
                {
                    // 로딩 씬이 필요하지 않은 경우, 타겟 씬으로 바로 이동
                    HotelManager.Instance.ExitHotel(needSave, allSaveFile, targetScene);
                }
                else
                {
                    NoManualHotelManager.Instance.SceneMove(false, false, targetScene);
                }
            }
        }
    }
}
