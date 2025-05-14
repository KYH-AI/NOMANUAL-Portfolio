using NoManual.Managers;
using UnityEngine;
using TMPro;

namespace NoManual.UI
{
    public class UI_CheckList : MonoBehaviour, IRequestManualBookData
    {
        [SerializeField] private GameObject taskTextPrefab;
        [SerializeField] private Transform standardTaskRoot;
        [SerializeField] private Transform bonusTaskRoot;
        [Space(5)] 
        [SerializeField] private TextMeshProUGUI currentDay;
        [SerializeField] private TextMeshProUGUI currentRound;

        [Space(5)]
        [SerializeField] private GameObject stdTaskPanel;
        [SerializeField] private GameObject bonusTaskPanel;
        [Space(5)]
        [SerializeField] private GameObject checkListEmptyText;
        [SerializeField] private GameObject checkListAllClearText;

        /// <summary>
        /// 체크리스트 초기화
        /// </summary>
        public void CheckListInitialization(CheckListUiDataMapper[] checkListUiDataMappers, bool showDayAndRound = true)
        {
            if (showDayAndRound)
            {
                currentDay.text = HotelManager.Instance.DayAndRound.CurrentDay + " Day";
                currentRound.text = ReportCutScene.ClearRound + " AM";
            }
            else
            {
                currentDay.text = string.Empty;
                currentRound.text = string.Empty;
            }
            
            checkListAllClearText.SetActive(false);
            checkListEmptyText.SetActive(checkListUiDataMappers == null);
            stdTaskPanel.SetActive(checkListUiDataMappers != null);
            bonusTaskPanel.SetActive(checkListUiDataMappers != null);
            
            if (checkListUiDataMappers == null) return;
            
            bool allClear = true;
            foreach (var checkListUiData in checkListUiDataMappers)
            {
                // 메인 퀘스트 및 서브 퀘스트 구별
                Transform targetRoot = standardTaskRoot;
                if (checkListUiData.isBonusTaskData)
                {
                    if (!checkListUiData.isClear) continue;
                    targetRoot = bonusTaskRoot;
                }

                // 퀘스트 텍스트 객체 생성 후 부모 설정
                GameObject checkListTaskObject = Instantiate(taskTextPrefab, targetRoot);
                TextMeshProUGUI checkListTaskText = checkListTaskObject.GetComponent<TextMeshProUGUI>();

                // 퀘스트 내용 등록
                checkListTaskText.text = checkListUiData.taskDescription;

                // 퀘스트 클리어 시 취소선 표시
                if (checkListUiData.isClear) checkListTaskText.fontStyle = FontStyles.Strikethrough;
                else allClear = false;
            }
            
            checkListAllClearText.SetActive(allClear);
        }


        /// <summary>
        /// 체크리스트 UI 데이터 매핑
        /// </summary>
        public class CheckListUiDataMapper
        {
            public string taskDescription { get; private set; } = string.Empty;
            public bool isBonusTaskData { get; private set; } = false;
            public bool isClear { get; private set; } = false;

            public CheckListUiDataMapper(string taskDescription, bool isBonusTaskData, bool isClear)
            {
                this.isBonusTaskData = isBonusTaskData;
                this.taskDescription = taskDescription;
                this.isClear = isClear;
            }
        }
    }
}

