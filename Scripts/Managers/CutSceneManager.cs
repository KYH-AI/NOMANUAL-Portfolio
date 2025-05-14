using System;
using System.Collections;
using System.Collections.Generic;
using NoManual.Utils;
using HFPS.Systems;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace NoManual.Managers
{
    public class CutSceneManager : Singleton<CutSceneManager>
    {
        public enum CutSceneType
        {
            None = -1,
            Tutorial_Meet_To_The_Doctor = 0,
            Report_Production = 1,
        }

        private const string TIME_LINE_PATH = "TimeLine/";
        private PlayableDirector _playableDirector; 
        private List<CutSceneTimeLine> _cutSceneTimeLines = new List<CutSceneTimeLine>();
        private Action[] _cutSceneEndEvents;
        
        private class CutSceneTimeLine
        {
            public CutSceneType cutSceneType;
            public TimelineAsset timelineAsset;

            public CutSceneTimeLine(CutSceneType cutSceneType, TimelineAsset timelineAsset)
            {
                this.cutSceneType = cutSceneType;
                this.timelineAsset = timelineAsset;
            }
        }
        
        public bool CutSceneRunning { get; set; } = false;

        private void Awake()
        {
            _playableDirector = GetComponent<PlayableDirector>();
        }

        /// <summary>
        /// 타임라인 종료 이벤트 등록 (등록 전 다른 이벤트 기록 모두 제거)
        /// </summary>
        private void CutSceneEventHandler(Action[] cutSceneEndEvents = null)
        {
            this._cutSceneEndEvents = cutSceneEndEvents;
        }

        /// <summary>
        /// 타임라인 실행
        /// </summary>
        public void PlayTimeLine(CutSceneType timelineAsset, Action[] cutSceneEndEvents, bool needSkipButton = false)
        {
            CutSceneTimeLine timeLine = GetTimeLineAsset(timelineAsset);
            if (timeLine == null)
            {
                ErrorCode.SendError(ErrorCode.ErrorCodeEnum.GetTimeLineAsset);
                return;
            }
            
            if (CutSceneRunning)
            {
                Debug.LogWarning("타임라인 작동 중!");
                return;
            }

            CutSceneEventHandler(cutSceneEndEvents);
            StartCoroutine(TimeLineCoroutine(timeLine, needSkipButton));
        }

        /// <summary>
        /// 타임라인 실행 코루틴
        /// </summary>
        private IEnumerator TimeLineCoroutine(CutSceneTimeLine timeLine, bool needSkipButton)
        {
            // 스킵 버튼 인터페이스 참조
            ISkipUiButtonController skipUiButtonController = NoManualHotelManager.Instance.UiNoManualUIManager.uiSkip.uiSkipButton;

            if (needSkipButton) skipUiButtonController.SetActiveSkipUiButton();

            // 컷신 종료 확인
            CutSceneRunning = true;
            // 플레이어 입력 제한
            FreezePlayer(true);
            // 타임라인 타이머 0으로 설정
            SetTimelineTime(0f);
            // 타임라인 재생
            _playableDirector.Play(timeLine.timelineAsset);

            while (_playableDirector.state is PlayState.Playing)
            {
                // 타임라인 스킵 입력 받기
                if (skipUiButtonController.UpdateSkipUiButton())
                {
                    // 타임라인 즉시 종료
                    _playableDirector.Stop();
                }
                yield return null;
            }


            if(needSkipButton) skipUiButtonController.SetDisableSkipUiButton();
            // 컷신 종료 확인
            CutSceneRunning = false;
            // 플레이어 입력 제한 해체
            FreezePlayer(false);
            // 타임라인 end 이벤트 실행
            foreach (var endEvent in _cutSceneEndEvents)
            {
                endEvent?.Invoke();
            }
            _cutSceneEndEvents = null;
        }

        /// <summary>
        /// 타임라인 에셋 얻기
        /// </summary>
        private CutSceneTimeLine GetTimeLineAsset(CutSceneType timelineACutSceneType)
        {
            CutSceneTimeLine timeLineTask = null;
            // 캐싱된 타임라인 에셋 가져오기
            foreach (var timeLine in _cutSceneTimeLines)
            {
                if (timelineACutSceneType == timeLine.cutSceneType)
                {
                    timeLineTask = timeLine;
                }
            }

            // 리소스 폴더에서 타임라인 에셋 찾기
            if (timeLineTask == null)
            {
                 TimelineAsset asset = Resources.Load<TimelineAsset>(TIME_LINE_PATH + timelineACutSceneType);
                 timeLineTask = new CutSceneTimeLine(timelineACutSceneType, asset);
                 _cutSceneTimeLines.Add(timeLineTask);
            }
            return timeLineTask;
        }

        public void PauseTimeline()
        {
            _playableDirector.Pause();
        }

        public void StopTimeline()
        {
            _playableDirector.Stop();
        }

        private void SetTimelineTime(float time)
        {
            _playableDirector.time = time;
        }

        /// <summary>
        /// 플레이어 입력 막기
        /// </summary>
        private void FreezePlayer(bool state)
        {
            if (state) NoManualHotelManager.Instance.DisablePlayer();
            else NoManualHotelManager.Instance.EnablePlayer();
        }

        private void OnDisable()
        {
            CutSceneEventHandler();
        }
    }
}


