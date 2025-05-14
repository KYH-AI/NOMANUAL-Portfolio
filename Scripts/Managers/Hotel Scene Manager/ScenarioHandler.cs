using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoManual.Task;

namespace NoManual.Managers
{
    public class ScenarioHandler
    {
        // 마지막으로 클리어한 엔딩타입
        public EndingType LastClearScType { get; set; } = EndingType.None;
        // 클리어한 시나리오 단계
        public ProcessType ProcessType { get; set; } = ProcessType.None; //  5일차 종료 시 LastClearScType이 None경우 ProcessType을 강제로 Ending으로 설정
        // 시나리오 퀘스트 관련 오브젝트 생성확인
        public bool isClear = true;
        

        /// <summary>
        /// 시나리오 퀘스트 클리어 시 엔딩 조건부 확인
        /// </summary>
        public void ScenarioTaskClear(EndingType clearEndingType, ProcessType endingProcess, int debugModeScRootId = -1)
        {
#if  UNITY_EDITOR
            
            if (debugModeScRootId != -1)
            {
                NoManualHotelManager.Instance.UiNoManualUIManager.DebugTestUI.SetScText(debugModeScRootId, true);
            }
  
#endif 
            
            // B가 우선순위가 더 높기 때문에 B로 업데이트
            if (clearEndingType == EndingType.B || LastClearScType == EndingType.B)
            {
                LastClearScType = EndingType.B;
                ProcessType = endingProcess;
            }
            else if (clearEndingType == EndingType.A)
            {
                LastClearScType = EndingType.A;
                ProcessType = endingProcess;
            }
            isClear = true;
        }

        public void UpdateScenarioEndingType()
        {
            if (!isClear) LastClearScType = EndingType.None;
        }
        
        /// <summary>
        /// 시나리오 Ending Type 세이브 데이터
        /// </summary>
        public EndingType SaveScenarioEndingType()
        {
            UpdateScenarioEndingType();
            return LastClearScType;
        }

        /// <summary>
        /// 시나리오 Process Type 세이브 데이터
        /// </summary>
        public ProcessType SaveProcessType()
        {
            return ProcessType;
        }
        
    }
}


