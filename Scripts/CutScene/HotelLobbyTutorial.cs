using System;
using NoManual.Interaction;
using NoManual.Utils;
using NoManual.Managers;
using ThunderWire.Utility;
using UnityEngine;

namespace NoManual.CutScene
{
    public class HotelLobbyTutorial : MonoBehaviour
    {
        [Header("호텔 로비 수화기 효과음 위치")] [SerializeField]
        private Transform lobbyPhoneSFXPosition;

        [Header("수화기 효과음")] 
        [SerializeField] private AudioClip lobbyPhoneSFX;
        
        [Header("노 매뉴얼 책")]
        [SerializeField] private ItemComponent noManualGuideBook;

        [Header("경비실 보고용 PC")] 
        [SerializeField] private ReportingDeskTopPC_Component reportingDeskTop;

        private AudioSource _lobbyPhoneAudioSource;
        

        /// <summary>
        /// 튜토리얼 연출 시작
        /// </summary>
        public void StartTutorial()
        {
            // 로비 수화기 연출 실행
            Invoke(nameof(PlayHotelLobbyPhoneSFX), 5f);
        }

        /// <summary>
        /// 로비 수화기 효과음 재생 및 아웃라인 활성화
        /// </summary>
        private void PlayHotelLobbyPhoneSFX()
        {
            if (!HotelManager.Instance.TutorialClear)
            {
                _lobbyPhoneAudioSource = Utilities.PlayLoopSound3D(lobbyPhoneSFXPosition.position, OptionHandler.AudioMixerChanel.SFX, lobbyPhoneSFX, 0.5f, 1f, 5f);
                OutLineUtil.AddOutLine(noManualGuideBook.gameObject, QuickOutline.Mode.OutlineVisible, Color.yellow, 1.8f);
            }
        }

        /// <summary>
        /// 튜토리얼 연출 종료
        /// </summary>
        public void StopHotelLobbyTutorial(bool outline)
        {
            StopHotelLobbyPhoneSFX();
            // 트리거 비활성화
            GetComponent<Collider>().enabled = false;
            // 보고용 PC 아웃라인 표시
            if (outline)
            {
                reportingDeskTop.SetOutLine();
            }
            HotelManager.Instance.TutorialClear = true;
        }
        

        /// <summary>
        /// 로비 수화기 효과음 종료 및 아웃라인 비활성화
        /// </summary>
        private void StopHotelLobbyPhoneSFX()
        {
            if (_lobbyPhoneAudioSource)
            {
                _lobbyPhoneAudioSource.Stop();
                Destroy(_lobbyPhoneAudioSource.gameObject);
            }
            OutLineUtil.RemoveOutLine(noManualGuideBook.gameObject);
        }

        private void OnTriggerEnter(Collider player)
        {
            if (player.CompareTag("Player"))
            {
                StopHotelLobbyTutorial(true);
            }
        }
    }
}
