using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NoManual.ANO;
using NoManual.Interaction;
using NoManual.Task;
using NoManual.Utils;
using NoManual.Creature;
using Random = UnityEngine.Random;

namespace NoManual.Managers
{
    public class ANO_Manager : MonoBehaviour
    {
        public bool LogMode = false;

#region Legacy
   [Header("=============== Legacy ===============")]
        [Header("ANO 원본 데이터")]
        [SerializeField] private ANO_DataBaseScriptbale anoDataBaseScriptable;
        [Header("ANO 오브젝트 부모 좌표")]
        [SerializeField] private Transform anoRootTransform;
        [Header("ANO Idle 오브젝트 부모 좌표")]
        [SerializeField] private Transform anoIdleRootTransform;
  

        [Header("디버그용")]
        [SerializeField] private ANO_ReplaceType currentType = ANO_ReplaceType.None;
        [Header("디버그용")]
        [SerializeField] private float percent = 0f;
        [Header("디버그용")] 
        [SerializeField] private List<int> activatedANO1_Id;
        [Header("디버그용")] 
        [SerializeField] private List<int> activatedANO2_Id;
        
        // ANO 초지 확인
        public bool isAllAnoClear { get; private set; } = false;
        
        // ANO 배치 완료 확인
        public bool IsSucessfullANO_Replace { get; private set; } = false;
        
        // ANO 복사 데이터
        private readonly Dictionary<int, ANO_CloneData_Legacy> _anoCloneDataBase = new Dictionary<int, ANO_CloneData_Legacy>();
        private readonly HashSet<int> _ano1List = new HashSet<int>();
        private readonly HashSet<int> _ano2List = new HashSet<int>();

        // 현재 활성화 된 ANO
        private List<ANO_Component> _currentANO_List = new List<ANO_Component>();
        public List<ANO_Component> GetCurrentANO_List => _currentANO_List;

        // 활성화 되었던 ANO 리스트들
        private HashSet<int> _activatedANO1_Id = new HashSet<int>();
        private HashSet<int> _activatedANO2_Id = new HashSet<int>();

        
        #region ANO 선별 설정

        // ANO 선별 기준
        public enum ANO_ReplaceType
        {
            None = -1,
            FirstRound = 0,
            Type1 = 1,
            Type2 = 2,
            Type3 = 3,
            Type4 = 4,
            Type5 = 5,
        }
        
        [Serializable]
        public struct ANO_ReplaceSettings
        {
            [Header("ANO 선별 기준 리스트")]
            public List<ANO_Replace> anoReplaceList;
            [Header("선별 기준3 이전 라운드")]
            public int placeType3Round;
            [Header("선별 기준4 이후 라운드")] 
            public int placeType4Round;
            [Header("선별기준 5 발동 카운터 (이전 라운드만큼 연속 ANO 배치가 없다면)")]
            public int type5LimitCount;
            [HideInInspector] public int currentType5Count;
            
            [Serializable]
            public struct ANO_Replace
            {
                [Header("선별 기준")]
                public ANO_ReplaceType anoReplaceType;
                [Header("ANO 선별 설정 리스트")]
                public List<ANO_ReplaceSettingValues> anoReplaceSettingValuesList;
                    
                [Serializable]
                public struct ANO_ReplaceSettingValues
                {
                    [Header("ANO 생성할 티어")] 
                    public ANOTier createANO_Tire;
                    [Header("ANO 생성 개수")]
                    public int createANOCount;
                    [Header("선별될 확률(주의! 최종 값이 제일 높아야 함)")]
                    public int anoReplacePercent;
                }
            }
        }

        [Header("ANO 선별 기준 설정")]
        public ANO_ReplaceSettings anoReplaceSettings = new ANO_ReplaceSettings();

        #endregion

        #region ANO Idle 오브젝트

        [Serializable]
        public struct ANO_Idle_Object
        {
            [Header("ANO Id")]
            public int ANO_Idle_Object_Id;
            [Header("평상 시 보여줄 오브젝트")]
            public GameObject ANO_Object;

            public ANO_Idle_Object(int anoId, GameObject anoIdlePrefab)
            {
                this.ANO_Idle_Object_Id = anoId;
                this.ANO_Object = anoIdlePrefab;
            }
        }

        [Header("ANO 평상 시 대기 오브젝트")] 
        private List<ANO_Idle_Object> _anoIdleObjects = new List<ANO_Idle_Object>();

        #endregion

#endregion
        
#region ANO 1.0

        #region Task 퀘스트 클리어 이벤트

        private event TaskHandler.BonusTaskEventHandler _bonusTaskHandler; 

        #endregion

        #region Task 퀘스트 생성 이벤트

        private event TaskHandler.BonusTaskEventHandler _bonusCreateTaskHandler; 

        #endregion
        

        private readonly string _ANO_CSV_PATH = "CSVData/ANO";
        
        [Space(10)]
        [Header("=================== ANO 1.0 ===================")] 
        [Header("ANO Sticker 오브젝트")]
        public GameObject anoStickerPrefab;

        [Space(10)]
        [Header("ANO DB")] 
        [SerializeField] private ANO_DataBaseScriptable anoDB;
        
        [Space(5)]
        [Header("ANO Idle Root")]
        [SerializeField] private Transform anoIdleRoot;

        [Space(5)]
        [Header("ANO Root")] 
        [SerializeField] private Transform anoRoot;
        
        [Space(5)] 
        [Header("크리처 Root")] 
        [SerializeField] private Transform creatureRoot;
        [SerializeField] private Transform[] creature1FPatrolList;

        [Space(5)] 
        [Header("왼쪽 엘리베이터")] 
        [SerializeField] private ElevatorComponent leftElevator;
        [Header("오른쪽 엘리베이터")] 
        [SerializeField] private ElevatorComponent rightElevator;

        private Dictionary<int, ANO_CloneData> _anoCloneData = new Dictionary<int, ANO_CloneData>();
        private ANO_Idle_Component[] _anoIdleComponents;

        private readonly List<ANO_Component> _enableAnoObjects = new List<ANO_Component>();
        private readonly List<ANO_Idle_Component> _disableAnoIdleObjects = new List<ANO_Idle_Component>();

            #region ANO 선별

            private Dictionary<int, int[]> _anoCsvParse; // 일차, ANO Id 배열 (CSV)
            private List<int> _defaultAnoId = new List<int>(); // 기본적으로 뽑아야할 ANO
            private readonly List<int> _usedDefaultAnoId = new List<int>(); // 전 라운드에서 사용된 ANO (뽑으면 안됨)
            private readonly List<int> _clearAnoLinkId = new List<int>(); // 클리어한 ANO Link ID (세이브 데이터)
            private Dictionary<int, int[]> _spawnAnoLinkId; // 라운드에 배치 필요한 ANO Link Id값  (불러온 세이브 데이터 기반으로 배치)
            public Dictionary<int, int[]> DebugGetCurrentSpawnAnoLink => _spawnAnoLinkId; // 디버깅용

            private const int _RND_MIN_VALUE = 5;   // 기존 0 (베타는 5)
            private const int _RND_MAX_VALUE = 7;   // 기존 4 (베타는 7)
            

            #endregion

            #region 크리처

            private Dictionary<int, CreatureCore> _creature = new Dictionary<int, CreatureCore>();

            #endregion

#endregion
        
        private void Awake()
        {
            /*
            // ANO 원본 데이터 복사
            foreach (var anoData in anoDataBaseScriptable.anoDataBase)
            {
                ANO_CloneData anoCloneData = new ANO_CloneData(anoData); 
                _anoCloneDataBase[anoData.ANOId] = anoCloneData;

                // ANO 1과 2를 구분해서 저장
                if (anoData.AnoTier is ANOTier.MA1)
                {
                    _ano1List.Add(anoData.ANOId);
                } 
                else if (anoData.AnoTier is ANOTier.MA2)
                {
                    _ano2List.Add(anoData.ANOId);
                }
                
                // Idle 프리팹이 없으면 생략
                if(!anoCloneData.ANO_IdlePrefab) continue;
                
                // ANO Idle 오브젝트 동적 생성 과 저장
                GameObject anoIdlePrefab = Instantiate(anoCloneData.ANO_IdlePrefab, anoIdleRootTransform);
                ANO_Idle_Object anoIdleObject = new ANO_Idle_Object(anoCloneData.ANOId, anoIdlePrefab);
                _anoIdleObjects.Add(anoIdleObject);
            }
            */
        }

        /// <summary>
        ///  Bonus Task 이벤트 초기화 (1회 호출)
        /// </summary>
        public void InitializationAnomalyManager(TaskHandler.BonusTaskEventHandler bonusTaskEventHandler)
        {
            _bonusTaskHandler -= bonusTaskEventHandler;
            _bonusTaskHandler += bonusTaskEventHandler;
        }

        /// <summary>
        /// ANO 데이터 초기화 (1회 호출)
        /// </summary>
        public void InitializationAnomaly(int[] anoLink, TaskHandler.BonusTaskEventHandler createTaskEventHandler)
        {
            // 테스트용
            _defaultAnoId.Clear();
            _usedDefaultAnoId.Clear();
            _clearAnoLinkId.Clear();
            _spawnAnoLinkId?.Clear();

            ReadAnoCsvFileAndParse();
            
            if (!anoDB)
            {
                ErrorCode.SendError(ErrorCode.ErrorCodeEnum.GetANO);
                return;
            }
            
            _anoCloneData = anoDB.GetAnoAllData();
            // ANO DB와 CSV 파일 무결성 검사
            foreach (var csvAnoIdArray in _anoCsvParse.Values)
            {
                foreach (var csvAnoId in csvAnoIdArray)
                {
                    if (!_anoCloneData.ContainsKey(csvAnoId))
                    {
                        NoManualUtilsHelper.EditorDebugLog(NoManualUtilsHelper.LogTextColor.red, "ANO DB에 ["+ csvAnoId +"] ID값을 가진 데이터가 없음");
                    }
                }
            }
            
            InitializationAnomalyLink(anoLink);
            
            if(anoIdleRoot)
            {
                _anoIdleComponents = anoIdleRoot.GetComponentsInChildren<ANO_Idle_Component>(true);
                foreach (var anoIdle in _anoIdleComponents) anoIdle.gameObject.SetActive(true);
                
                NoManualUtilsHelper.EditorDebugLog(NoManualUtilsHelper.LogTextColor.cyan, "ANO IDLE 초기화 완료");
            }

            if (anoRoot)
            {
                foreach (Transform ano in anoRoot) Destroy(ano.gameObject);
                NoManualUtilsHelper.EditorDebugLog(NoManualUtilsHelper.LogTextColor.cyan, "ANO 초기화 완료");
            }

            _bonusCreateTaskHandler -= createTaskEventHandler;
            _bonusCreateTaskHandler += createTaskEventHandler;

        }
        
        /// <summary>
        /// ANO CSV 파일 읽기 (1회 호출)
        /// </summary>
        private void ReadAnoCsvFileAndParse()
        {
            // ANO CSV 파싱
           int[] ParseAnoCsvFile(string[] values)
            {
                
                int[] anoCsvParseArray = new int[2];

                // Id
                anoCsvParseArray[1] = int.Parse(values[0]);
                // Day
                anoCsvParseArray[0] = int.Parse(values[1]);
                return anoCsvParseArray;
            }
            
            // 1.  ReadAnoCsvFile을 통해 CSV 파일의 각 행을 Dictionary<int, int[]>로 변환하는 함수를 정의
            CSVFileHelper<int[]> anoCsvParser = new CSVFileHelper<int[]>(ParseAnoCsvFile);
        
            // 2. ParseCSV 메서드를 호출하여 CSV 파일을 파싱
            List<int[]> parsedData = anoCsvParser.ParseCSV(_ANO_CSV_PATH);
            
            // 3. 리스트의 데이터를 하나의 Dictionary<int, int[]>로 병합
           Dictionary<int, List<int>> tempAnoCsvParse = new Dictionary<int, List<int>>();

            foreach (var anoData in parsedData)
            {
                int day = anoData[0];
                int id = anoData[1];

                if (!tempAnoCsvParse.ContainsKey(day))
                {
                    tempAnoCsvParse[day] = new List<int>();
                }
                
                // ANO ID값 중복
                if (tempAnoCsvParse[day].Contains(id))
                {
                    NoManualUtilsHelper.EditorDebugLog(NoManualUtilsHelper.LogTextColor.red, "CSV 파일 ["+ day + "] 일차 ["+ id +"] ANO Id값 중복");
                    continue;
                }
                // Id를 리스트에 추가
                tempAnoCsvParse[day].Add(id);
            }

            // List<int>를 int[]로 변환하여 최종 딕셔너리 생성
            _anoCsvParse = tempAnoCsvParse.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToArray()
            );
        }
        
        /// <summary>
        /// ANO LINK Id 라운드 분배 (1회 호출)
        /// </summary>
        private void InitializationAnomalyLink(int[] anoLinkId)
        {
            // ANO Link 할당 (1회 호출)
            SetDistributeAnoLink(anoLinkId);
        }

        /// <summary>
        /// ANO Link 세이브 데이터
        /// </summary>
        public int[] SaveAnoLink()
        {
            return _clearAnoLinkId.ToArray();
        }

        #region Bonus 퀘스트 클리어

        /// <summary>
        /// Bonus Task 퀘스트 트리거
        /// </summary>
        public void BonusTaskTrigger(TaskHandler.TaskType taskType, string taskId)
        {
            _bonusTaskHandler?.Invoke(taskType, taskId);
            
            // ANO_Link 기록
            var ano = _anoCloneData[int.Parse(taskId)];
            if (ano != null && ano.ANO_Link_Id != -1)
            {
                _clearAnoLinkId.Add(ano.ANO_Link_Id);
                NoManualUtilsHelper.EditorDebugLog( NoManualUtilsHelper.LogTextColor.green,"Link ANO 기록: " + String.Join(", ", _clearAnoLinkId));
            }
        }

        #endregion
        
        #region ANO 매니저 1.0
        
        /// <summary>
        /// ANO 라운드 초기화 (라운드 마다 호출)
        /// </summary>
        public void ResetAno()
        {
            // Idle 원상복구
            foreach (var anoIdle in _disableAnoIdleObjects)
            {
                anoIdle.gameObject.SetActive(true);
            }
            _disableAnoIdleObjects.Clear();

            // 생성된 ANO 제거
            foreach (Transform ano in anoRoot)
            {
                Destroy(ano.gameObject);
            }
            _enableAnoObjects.Clear();
            
            ResetCreature();
            
            NoManualUtilsHelper.EditorDebugLog(NoManualUtilsHelper.LogTextColor.yellow, "ANO Idle / ANO  / 크리처 리셋 완료");
        }
        
        
        private void DisableIdleAno(int idleAnoId)
        {
            // Idle ANO 비활성화
           ANO_Idle_Component anoIdle = _anoIdleComponents.FirstOrDefault(idleAno => idleAno.GetAnoIdleId == idleAnoId);
           if (anoIdle)
           {
               anoIdle.gameObject.SetActive(false);
               _disableAnoIdleObjects.Add(anoIdle); 
           }
        }
        

        [ContextMenu("ANO 배치")]
        /// <summary>
        /// ANO & ANO_Link 배치 (라운드 마다 호출)
        /// </summary>
        public void PickUpAnoHandler()
        {
            // ANO 뽑기 후 생성
            
            /* 0. 0일차는 생략 */
            
            /* 1. 1일차 경우에만 ANO를 모두 랜덤으로 뽑는다 (중복금지) */
            
            /* 2. 2일차 ~ 5일차까지 부터는 ANO_LINK가 -1값을 가진 기준으로 부터 랜덤으로 뽑는다 (중복금지) */
            
            /* 3. 1번과 2번 규칙을 이용해 각 N일차마다 1~4라운드 기준으로 ANO을 0~4개 뽑아서 배치한다 */
            
            /* 4. 만약 랜덤 뽑기에 Cycle를 모두 소모한 경우 다시 처음부터 재활용해서 뽑는다 (당연히 중복금지) */
            
            /* 5. 마지막으로 플레이어 세이브 데이터에 있는 ANO_LINK 배열을 읽어와 ANO_LINK에 있는 ID값을 이용해 현재 일차에서 1~4라운드에 랜덤으로 ANO을 배분한다.
               - (!중요) 랜덤으로 배분하는 경우 한 라운드에 너무 ANO_LINK에 있는 ID가 모두 배분되지 않도록 적절하게 조절이 필요하다
                - EX) [0,1,2,3] ANO_LINK에 있다면 적절하게 1라운드에 1개, 2라운드에 0개, 3라운드에 2개, 4라운드에 1개 이런 형태로 한번에 분배되지 않도록 해야한다
             
             */

            int currentDay = HotelManager.Instance.DayAndRound.CurrentDay;
            int currentRound = HotelManager.Instance.DayAndRound.CurrentRound;

            // log 기록용
            if (LogMode)
            {
                currentDay = testDayLog;
                currentRound = testDayRoundLog;
            }
            
            // 0 (베타는 0일차도 등장)
            //if(currentDay == 0) return;
            
            // 1
            if (currentDay == 1)
            {
                // 모든 ANO 선택
                _defaultAnoId = _anoCsvParse[1].ToList();
            }
            // 2
            else
            {
                // Link를 제외한 ANO 선택
                foreach (var anoId in _anoCsvParse[currentDay])
                {
                    if(_anoCloneData[anoId].ANO_Link_Id != -1) continue;
                    _defaultAnoId.Add(anoId);
                }
            }

            // 사용된 ANO 제거
            _defaultAnoId = _defaultAnoId.Except(_usedDefaultAnoId).ToList();

            // 3
            // 랜덤으로 ANO Default 선택
            SetSelectDefaultAno(Random.Range(_RND_MIN_VALUE, _RND_MAX_VALUE));
            
            // 4 
            // 선별된 Default 및 ANO_Link 생성
            foreach (int defaultAnoId in _defaultAnoId)
            {
                CreateAno(defaultAnoId);
            }
            
            NoManualUtilsHelper.EditorDebugLog( NoManualUtilsHelper.LogTextColor.green,"Default ANO 생성: " + String.Join(", ", _defaultAnoId));

            if (_spawnAnoLinkId == null) return;
            foreach (int anoLinkId in _spawnAnoLinkId[currentRound])
            {
                CreateAno(anoLinkId);
            }
            
            NoManualUtilsHelper.EditorDebugLog( NoManualUtilsHelper.LogTextColor.green,"Link ANO 생성: " + String.Join(", ", _spawnAnoLinkId[currentRound]));

        }

        #region ANO 선별 랜덤 로직
        
        
        /// <summary>
        /// Default ANO 선별
        /// </summary>
        private void SetSelectDefaultAno(int numToSelect)
        {
            List<int> selectedAno = new List<int>(); // 뽑힌 ANO ID들을 저장할 리스트

            // 뽑아야 할 개수가 남은 _defaultAnoId보다 많다면
            while (numToSelect > _defaultAnoId.Count)
            {
                // 남아 있는 _defaultAnoId에서 모두 선택
                selectedAno.AddRange(_defaultAnoId);
                numToSelect -= _defaultAnoId.Count;
                
                
                // _usedDefaultAnoId에서 ANO_Link ID는 제외하고 할당 (ANO_Link는 이미 다 배치됬다는 의미)
                _defaultAnoId = _usedDefaultAnoId.Where(id => anoDB.GetAnoData(id).ANO_Link_Id == -1).ToList();
                _usedDefaultAnoId.Clear(); // _usedDefaultAnoId 초기화
            }

            // 필요한 만큼 랜덤하게 뽑기 (_defaultAnoId에서 중복 없이)
            selectedAno.AddRange(DrawRandomFromDefault(numToSelect));
            _defaultAnoId.Clear();
            _defaultAnoId = selectedAno;

            // 픽업된 ANO 기록
            _usedDefaultAnoId.AddRange(selectedAno);
        }
        
        /// <summary>
        /// 랜덤으로 numToSelect개의 ANO ID를 _defaultAnoId에서 뽑는 함수 (중복 방지)
        /// </summary>
        private List<int> DrawRandomFromDefault(int numToSelect)
        {
            List<int> selected = new List<int>();

            for (int i = 0; i < numToSelect; i++)
            {
                if (_defaultAnoId.Count == 0) break;  // 남은 게 없으면 중단

                int randomIndex = Random.Range(0, _defaultAnoId.Count);
                selected.Add(_defaultAnoId[randomIndex]);  // 랜덤으로 하나 선택
                _defaultAnoId.RemoveAt(randomIndex);  // 선택된 ANO를 _defaultAnoId에서 제거
            }
            return selected;
        }
        
        /// <summary>
        /// ANO_Link_ID 배열을 1~4 라운드에 분배하는 함수 (1회 호출)
        /// </summary>
        private void SetDistributeAnoLink(int[] anoLinkId)
        {
            // 테스트용  (<- ??? 09.27)
            /*
            if (HotelManager.Instance.DayAndRound.CurrentDay <= 1) return;
            List<int> testAnoLinkId = new List<int>();
            foreach (var anoid in _anoCsvParse[HotelManager.Instance.DayAndRound.CurrentDay-1])
            {
                if (_anoCloneData[anoid].ANO_Link_Id != -1)
                {
                    testAnoLinkId.Add(_anoCloneData[anoid].ANO_Link_Id);
                }
                
            }
            anoLinkId = testAnoLinkId.ToArray();
            */

            
            
            // 분배할 값이 없으면 바로 리턴
            if ( anoLinkId == null || anoLinkId.Length == 0 ) return;
            
            // 라운드 리스트 초기화 (1~4 라운드)
            Dictionary<int, List<int>> roundsToAnoLink = new Dictionary<int, List<int>>
            {
                {1, new List<int>()},
                {2, new List<int>()},
                {3, new List<int>()},
                {4, new List<int>()},
            };
            
            System.Random random = new System.Random();

            List<int> anoLinkList = anoLinkId.ToList();
            
            // 값이 적은 경우에 대비해 랜덤하게 섞음
            anoLinkList = anoLinkList.OrderBy(x => random.Next()).ToList();

            // 라운드에 고르게 배분하는 로직
            int remainingValues = anoLinkList.Count;
            int currentRound = 1;
            bool anyValueAssigned = false; // 어떤 라운드에라도 값이 배치되었는지 확인하는 플래그

            while (remainingValues > 0 && currentRound <= 4)
            {
                // 각 라운드에 랜덤으로 0~2개의 값을 배치 (최대 2개까지 배치)
                int valuesToPlace = random.Next(0, Math.Min(3, remainingValues + 1)); // 0, 1, 또는 2개 선택
                if (valuesToPlace > 0)
                {
                    roundsToAnoLink[currentRound].AddRange(anoLinkList.Take(valuesToPlace)); // 해당 라운드에 값을 배치
                    anoLinkList = anoLinkList.Skip(valuesToPlace).ToList(); // 배치한 값 제거
                    remainingValues -= valuesToPlace; // 남은 값을 업데이트
                    anyValueAssigned = true; // 값이 배치된 라운드가 있음을 기록
                }
                currentRound++; // 다음 라운드로 넘어가기 (1~4 라운드까지 반복)
            }

            // 만약 값이 단 하나도 배치되지 않았다면, 최소 하나의 라운드에 강제로 배치
            if (anoLinkList.Count != 0 || !anyValueAssigned)
            {
                // 남은 값을 각 라운드에 순회하면서 0 또는 1개씩 배치
                while (anoLinkList.Count > 0)
                {
                    // 랜덤한 라운드를 선택 (1~4 중 하나)
                    int randomRound = random.Next(1, 5);  // 1부터 4 사이의 랜덤한 라운드 선택

                    // 라운드에 0 또는 1개 배치
                    int valuesToPlace = random.Next(0, 2);  // 0 또는 1 선택
                    if (valuesToPlace > 0 && anoLinkList.Count > 0)
                    {
                        roundsToAnoLink[randomRound].Add(anoLinkList[0]); // 한 개 배치
                        anoLinkList.RemoveAt(0);  // 배치된 값 제거
                    }
                }
            }
            // 여기서 픽업된 ANO는 기억시킬 필요가 없음 (Why? 이미 모든 라운드에 배치가 끝난상태라서)
            // List<int>를 int[]로 변환하여 최종 딕셔너리 생성
            _spawnAnoLinkId = roundsToAnoLink.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToArray()
            );
        }
        

        
        #endregion
        
        /// <summary>
        /// ANO 생성
        /// </summary>
        private void CreateAno(int id)
        {
            if (!_anoCloneData.ContainsKey(id))
            {
                ErrorCode.SendError("ANO ID : " + id, ErrorCode.ErrorCodeEnum.GetANO);
                return;
            }
            
            // ANO 프리팹 생성
            GameObject anoPrefab = _anoCloneData[id].ANO_Prefab;
            ANO_CloneData anoCloneData = _anoCloneData[id];

            if (anoCloneData.HaveAnoIdle)
            {
                DisableIdleAno(id);
            }
            
            if (anoPrefab)
            {
                ANO_Component ano = Instantiate(anoPrefab, anoRoot).GetComponent<ANO_Component>();
                ano.InitializeANO(anoCloneData);
                _enableAnoObjects.Add(ano);
                // ANO 퀘스트 생성 TODO : (Solve Type 이 None이면 퀘스트 생성 X, 아니면 생성 O)
                _bonusCreateTaskHandler?.Invoke(TaskHandler.TaskType.Interaction, id.ToString());
            }
        }
        
        #endregion

        #region 크리처

        /// <summary>
        /// 크리처 생성 (ANO에서 호출)
        /// </summary>
        public void CreateCreature(CreatureSO creature, Transform spawnPos)
        {
            if (_creature.ContainsKey(creature.CreatureId)) return;
            _creature.Add(creature.CreatureId, null);
            var instantiatedCreatureCore = Instantiate(creature.CreaturePrefab, spawnPos.position, spawnPos.rotation, creatureRoot).GetComponent<CreatureCore>();
            _creature[creature.CreatureId] = instantiatedCreatureCore;
        }

        /// <summary>
        /// 크리처 Patrol
        /// </summary>
        public Transform[] GetCreaturePatrolNode(int floor)
        {
            Transform[] nodeList = null;
            if (floor == 1) nodeList = creature1FPatrolList;
            return nodeList;
        }

        /// <summary>
        /// 크리처 리셋 (라운드 마다 호출)
        /// </summary>
        public void ResetCreature()
        {
            foreach (var creature in _creature.Values)
            {
                creature.SendCreatureDestroyEvent();
            }
            _creature.Clear();
        }

        
        /// <summary>
        /// 특정 크리쳐 소멸
        /// </summary>
        public void TargetResetCreature(int creatureId)
        {
            if (_creature.ContainsKey(creatureId))
            {
                _creature[creatureId].SendCreatureDestroyEvent();
                _creature.Remove(creatureId);
            }
            else
            {
                CreatureCore target = null;
                // 모든 자식 객체에서 CreatureCore 컴포넌트를 가져옴
                CreatureCore[] creatureCores = creatureRoot.GetComponentsInChildren<CreatureCore>();
                foreach (var creatureCore in creatureCores)
                {
                    if (creatureCore.GetCreatureID == creatureId)
                    {
                        target = creatureCore;
                        break;
                    }
                }
                if(target) target.SendCreatureDestroyEvent();
            }
        }

        #endregion

        #region 엘레베이터

        /// <summary>
        /// 엘베 비활성화
        /// </summary>
        public void DisableElevatorInteraction(bool isLeft)
        {
            if (isLeft)
            {
                leftElevator.SetNoInteractElevatorButton();
            }
            else
            {
                rightElevator.SetNoInteractElevatorButton();
            }
        }

        /// <summary>
        /// 엘베 활성화
        /// </summary>
        public void EnableElevatorInteraction(bool isLeft)
        {
            if (isLeft)
            {
                leftElevator.SetYesInteractElevatorButton();
            }
            else
            {
                rightElevator.SetYesInteractElevatorButton();
            }
        }

        #endregion
        
        #region ANO CSV Log 남기기

            private int testDayLog = 0;
            private int testDayRoundLog = 1;
            
            [ContextMenu("ANO Log 생성")]
            /// <summary>
            /// 테스트용 Log 남기기
            /// </summary>
            public void ANO_LOG_TEST()
            {
                for (int i = 0; i < 5; i++)
                {
                    testDayLog = 0;

                    List<ANO_LOG_DATA> logDataList = new List<ANO_LOG_DATA>();
                    int[] anoLinkId = null;
                    _spawnAnoLinkId = null;

                    while (testDayLog <= 5)
                    {
                        _defaultAnoId.Clear();
                        _usedDefaultAnoId.Clear();
                        testDayRoundLog = 1;
                        
                        InitializationAnomalyLink(anoLinkId);

                        while (testDayRoundLog <= 4)
                        {
                            PickUpAnoHandler();

                            foreach (var id in _defaultAnoId)
                            {
                                logDataList.Add(new ANO_LOG_DATA(id, testDayLog, testDayRoundLog, anoDB.GetAnoData(id).ANO_Link_Id));
                            }

                            if (_spawnAnoLinkId != null)
                            {
    
                                foreach (var linkId in _spawnAnoLinkId[testDayRoundLog])
                                {
                                    logDataList.Add(new ANO_LOG_DATA(linkId, testDayLog, testDayRoundLog, anoDB.GetAnoData(linkId).ANO_Link_Id));
                                }
                            }
                        
                            testDayRoundLog++;
                        }

                        // link_id가 -1이 아닌 값들만 추출하여 int 배열로 변환
                        anoLinkId = logDataList.Where(linkData => linkData.day == testDayLog && linkData.link_id != -1)
                            .Select(linkData => linkData.link_id)
                            .ToArray();
                        testDayLog++;
                    }

                    CSVLogger csv = new CSVLogger();
                    csv.LogAnoData(logDataList);
                }
            }

            public class ANO_LOG_DATA
            {
                public int id;
                public int day;
                public int round;
                public int link_id;

                public ANO_LOG_DATA(int id, int day, int round, int linkID)
                {
                    this.id = id;
                    this.day = day;
                    this.round = round;
                    this.link_id = linkID;
                }
            }

         #endregion

        
        /* ============== ANO Demo (Legacy) ============== */

        #region ANO 에디터 테스트 동적 생성

        [ContextMenu("ANO 1라운드 배치")]
        public void Test_ANO_Place()
        {
            ANO_Replace(ANO_ReplaceType.FirstRound);
        }
        
        [ContextMenu("ANO 보고 후 배치")]
        public void Test_ANO_Reporting()
        {
            ANO_Replace();
        }
        

        [ContextMenu("ANO0_NeckMan ANO 생성")]
        public void TestCreatANO_0()
        {
            AddNewANO(0);
        }
        
        [ContextMenu("ANO1_DisappearGL_MAN ANO 생성")]
        public void TestCreatANO_1()
        {
            AddNewANO(1);
        }
        
        [ContextMenu("ANO2_Prayer ANO 생성")]
        public void TestCreatANO_2()
        {
            AddNewANO(2);
        }
        
        [ContextMenu("ANO3_Bleeding Road ANO 생성")]
        public void TestCreatANO_3()
        {
            AddNewANO(3);
        } 
        
        [ContextMenu("ANO4_DropPainting ANO 생성")]
        public void TestCreatANO_4()
        {
            AddNewANO(4);
        } 
        
        [ContextMenu("ANO5_Necromancy Practice ANO 생성")]
        public void TestCreatANO_5()
        {
            AddNewANO(5);
        } 
        
        [ContextMenu("ANO6_Dead Cats ANO 생성")]
        public void TestCreatANO_6()
        {
            AddNewANO(6);
        }
        
        [ContextMenu("ANO7_Bleeding Water Tap ANO 생성")]
        public void TestCreatANO_7()
        {
            AddNewANO(7);
        } 
        
        [ContextMenu("ANO8_Room Open ANO 생성")]
        public void TestCreatANO_8()
        {
            AddNewANO(8);
        } 
        
        [ContextMenu("ANO9_Statue Drop ANO 생성")]
        public void TestCreatANO_9()
        {
            AddNewANO(9);
        }
        
        [ContextMenu("ANO10_Infinite Documents ANO 생성")]
        public void TestCreatANO_10()
        {
            AddNewANO(10);
        }
        
        [ContextMenu("ANO11_Punched 105 Room 생성")]
        public void TestCreatANO_11()
        {
            AddNewANO(11);
        }
        
        [ContextMenu("ANO12_Meeting Room TV Noise 생성")]
        public void TestCreatANO_12()
        {
            AddNewANO(12);
        }
        
        [ContextMenu("ANO13_Elevator Sound 생성")]
        public void TestCreatANO_13()
        {
            AddNewANO(13);
        }
        
        [ContextMenu("ANO14_Some One Watching You 생성")]
        public void TestCreatANO_14()
        {
            AddNewANO(14);
        }
        
                
        [ContextMenu("ANO15_Same Painting 생성")]
        public void TestCreatANO_15()
        {
            AddNewANO(15);
            IsSucessfullANO_Replace = true;
        }
        
        [ContextMenu("ANO16_104 Semi Open 생성")]
        public void TestCreatANO_16()
        {
            AddNewANO(16);
        }
        
        [ContextMenu("ANO17_No Entry Signs 생성")]
        public void TestCreatANO_17()
        {
            AddNewANO(17);
        }
        
        [ContextMenu("ANO18_Rotten Sack 생성")]
        public void TestCreatANO_18()
        {
            AddNewANO(18);
        }
        
        [ContextMenu("ANO19_Fake Sign 생성")]
        public void TestCreatANO_19()
        {
            AddNewANO(19);
        }
        
        [ContextMenu("ANO20_Trace Blood 생성")]
        public void TestCreatANO_20()
        {
            AddNewANO(20);   
        }

        [ContextMenu("ANO21 생성")]
        public void TestCreatANO_21()
        {
            AddNewANO(21);
        }
        
        [ContextMenu("ANO22 생성")]
        public void TestCreatANO_22()
        {
            AddNewANO(22);
        }
        
        [ContextMenu("ANO23 생성")]
        public void TestCreatANO_23()
        {
            AddNewANO(23);
        }
        
        [ContextMenu("ANO24 생성")]
        public void TestCreatANO_24()
        {
            AddNewANO(24);
        }

        #endregion

        #region ANO CRUD

        /// <summary>
        /// ANO 프리팹 생성
        /// </summary>
        public void AddNewANO(int ANOid)
        {
            ANO_CloneData_Legacy anoCloneDataLegacy = GetANO_CloneDataToId(ANOid);
            
            if(anoCloneDataLegacy == null) return;

            // ANO 프리팹 생성
            ANO_Component anoComponent = Instantiate(anoCloneDataLegacy.ANOPrefab, anoRootTransform).GetComponent<ANO_Component>();
            ANORatingType ratingType = CalculationANORating(anoCloneDataLegacy.anoRatingSettings);
            
            // 해당 ANO에 대한 ANO Idle 오브젝트 찾기
            int anoIdleObjectId = GetANO_IdleObjectId(anoCloneDataLegacy.ANOId);
                
            // ANO 데이터 초기화
            anoComponent.InitializeANO(anoCloneDataLegacy, ratingType, anoIdleObjectId);
            
            _currentANO_List.Add(anoComponent);
        }

        /// <summary>
        /// 배치된 ANO 오브젝트 모두 제거
        /// </summary>
        private void Current_ANO_ObjectAllDelete()
        {
            // ANO 배치 하기 전 지난 라운드에 배치된 ANO 오브젝트 모두 제거
            if (_currentANO_List.Count > 0)
            {
                /*
                 *  foreach 루프를 사용하기 때문에
                 *   foreach 루프는 반복 중에 컬렉션을 수정할 수 없음.
                 *   이런 경우에는 대신에 일반적인 for 루프를 사용
                 */
                for (int i = _currentANO_List.Count - 1; i >= 0; i--)
                {
                    DeleteANO(_currentANO_List[i]);
                }
            }
        }
        
        /// <summary>
        /// ANO Id값을 이용해 ANO 복사 데이터 얻기
        /// </summary>
        private ANO_CloneData_Legacy GetANO_CloneDataToId(int ANOid)
        {
            if (_anoCloneDataBase.ContainsKey(ANOid))
            {
                return _anoCloneDataBase[ANOid];
            }
            
            ErrorCode.SendError(ErrorCode.ErrorCodeEnum.GetANO);
            return null;
        }
        
        /// <summary>
        /// 랜덤 ANO ID 가져오기 [중복 허용 X]
        /// </summary>
        /// <param name="targetTier">가져올 ANO 티어</param>
        /// <param name="anoCount">총 가져올 ANO 개수</param>
        private int[] GetRandom_ANO_ID(ANOTier targetTier, int anoCount)
        {
            int selectCount = 0;
            // 배열 -1로 초기화
            // int[] selectedAnoId = Enumerable.Repeat(-1, anoCount).ToArray();
            // 선택 가능한 리스트
            List<int> selectableList = new List<int>();
            // 선정해야할 ANO 티어 리스트
            HashSet<int> targetANO_list = targetTier is ANOTier.MA1 ? _ano1List : _ano2List;
            // 게임에서 선정되었던 ANO 리스트
            HashSet<int> activedANO_List = targetTier is ANOTier.MA1 ? _activatedANO1_Id : _activatedANO2_Id;
            //디버그용
            List<int> testDeubug_List = targetTier is ANOTier.MA1 ? activatedANO1_Id : activatedANO2_Id;
            // 디버그용
            string anoTier = targetTier is ANOTier.MA1 ? "MA1" : "MA2";
            
            // ANO가 모두 선정된 경우 다시 초기화
            if (activedANO_List.Count == targetANO_list.Count)
            {
                Debug.Log(anoTier + " 초기화");
                
                activedANO_List.Clear();
                testDeubug_List.Clear();
            }

            // 랜덤으로 선정해야할 ANO 리스트 생성
            foreach (int targetANOid in targetANO_list)
            {
                // 해당 ANO는 선택된 기록이 없음
                if (!activedANO_List.Contains(targetANOid))
                {
                    // 랜덤으로 선택 가능한 ANO 리스트에 할당
                    selectableList.Add(targetANOid);
                }
            }

            // 랜덤 Seed 할당
            System.Random randomSeed = new System.Random();
            HashSet<int> pickedId = new HashSet<int>();
            do
            {
                /* #region Demo V1.0 ANO 선정 기준
               
                // 랜덤으로 ANO ID 선별
                int selectId = Random.Range(0, _anoCloneDataBase.Count);
                // 선택한 ID 기준으로 ANO 티어 확인
                ANOTier anoTier = GetANO_CloneDataToId(selectId).AnoTier;
                // ANO 티어확인
                if (anoTier == targetTier)
                {
                    // 중복 ID 찾기
                    bool isDuplicate = NoManual.Utils.Utils.FindDuplicate<int>(selectId, selectedAnoId);
                    if (!isDuplicate)
                    {
                        // 선별된 ANO Id 저장
                        selectedAnoId[selectCount] = selectId;
                        selectCount++;
                    }
                }
               
                #endregion  */

                // ANO가 모두 선정된 경우 다시 초기화
                if (activedANO_List.Count == targetANO_list.Count)
                {
                    Debug.Log(anoTier + " 초기화");
                    activedANO_List.Clear();
                    // 랜덤으로 선택 가능한 ANO 리스트들을 모두 할당
                    selectableList = targetANO_list.ToList();
                    testDeubug_List.Clear();
                }
                
                // selectableList에서 랜덤으로 하나의 요소를 선택
                int selectId = selectableList.OrderBy(x => randomSeed.Next()).FirstOrDefault();
                
                
                // 중복 Id 찾기
                if (pickedId.Contains(selectId))
                {
                    continue;
                }
                
                // 선별된 ANO Id 저장
                pickedId.Add(selectId);
                // 게임에서 선정된 ANO 리스트로 할당
                activedANO_List.Add(selectId);
                testDeubug_List.Add(selectId);
                testDeubug_List.Sort();
                selectCount++;

            } while (selectCount < anoCount);

            return pickedId.Count == 0 ? new int[] { -1 } : pickedId.ToArray();
        }

        /// <summary>
        /// ANO 오브젝트 비활성화
        /// </summary>
        public void DisableANO(ANO_Component anoComponent)
        {
            if(anoComponent != null) anoComponent.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// ANO 오브젝트 삭제
        /// </summary>
        public void DeleteANO(ANO_Component anoComponent)
        {
            if (anoComponent != null)
            {
                for (int i = 0; i < _currentANO_List.Count; i++)
                {
                    if (_currentANO_List[i] == anoComponent)
                    {
                        Destroy(anoComponent.gameObject);
                        _currentANO_List.RemoveAt(i);
                        // 삭제 후 루프 종료
                        break; 
                    }
                }
            }
        }

        #endregion

        #region ANO Idle Object

        /// <summary>
        /// ANO Idle 오브젝트를 Id 값으로 얻기 
        /// </summary>
        private GameObject GetANO_IdleObjectToId(int anoIdleObjectId)
        {
            foreach (var anoIdleObject in _anoIdleObjects)
            {
                if (anoIdleObject.ANO_Idle_Object_Id == anoIdleObjectId)
                {
                    return anoIdleObject.ANO_Object;
                }
            }

            ErrorCode.SendError(ErrorCode.ErrorCodeEnum.GetANOIdleObject);
            return null;
        }
        
        
        /// <summary>
        /// ANO Idle 오브젝트의 Id 값 얻기 (-1 경우 해당 Idle 오브젝트는 없음)
        /// </summary>
        private int GetANO_IdleObjectId(int anoIdleObjectId)
        {
            foreach (var anoIdleObject in _anoIdleObjects)
            {
                if (anoIdleObject.ANO_Idle_Object_Id == anoIdleObjectId)
                {
                    return anoIdleObject.ANO_Idle_Object_Id;
                }
            }
            return -1;
        }
        
        /// <summary>
        /// ANO Idle 오브젝트 활성화
        /// </summary>
        public void Enable_ANO_IdleObject(int anoIdleObjectId)
        {
            GameObject anoIdleObject = GetANO_IdleObjectToId(anoIdleObjectId);
            if (!anoIdleObject || anoIdleObject.activeSelf) return;
            
            anoIdleObject.SetActive(true);
        }
        
        /// <summary>
        /// ANO Idle 오브젝트 비활성화
        /// </summary>
        public void Disable_ANO_IdleObject(int anoIdleObjectId)
        {
            GameObject anoIdleObject = GetANO_IdleObjectToId(anoIdleObjectId);
            if (!anoIdleObject || !anoIdleObject.activeSelf) return;
            
            anoIdleObject.SetActive(false);
        }

        /// <summary>
        /// ANO Idle 오브젝트 모두 활성화
        /// </summary>
        private void EnableAll_ANO_IdleObject()
        {
            foreach (var anoIdle in _anoIdleObjects)
            {
                Enable_ANO_IdleObject(anoIdle.ANO_Idle_Object_Id);
            }
        }
        
        /// <summary>
        /// ANO Idle 오브젝트 모두 비활성화
        /// </summary>
        private void DisableAll_ANO_IdleObject()
        {
            foreach (var anoIdle in _anoIdleObjects)
            {
                Disable_ANO_IdleObject(anoIdle.ANO_Idle_Object_Id);
            }
        }

        #endregion
    
        #region ANO 레이팅 계산

        private ANORatingType CalculationANORating(ANO_DataScriptable_Legacy.ANORatingSettings anoRatingData)
        {
            ANORatingType resultANORatingType = ANORatingType.None;
            int totalRatingValue = 0;

            totalRatingValue += CalculateMentalityDamagePoint(anoRatingData.mentalityDamage);
            totalRatingValue += anoRatingData.AnoDeteriorationSettings.deterioration ? 1 : 0;
            totalRatingValue += (int)anoRatingData.AnoManagement;
            totalRatingValue += (int)anoRatingData.AnoEventRange;
            totalRatingValue += (int)anoRatingData.AnoVisvility;
            totalRatingValue += anoRatingData.AnoDeteriorationSettings.IsLink ? 1 : 0;

            if (totalRatingValue <= (int)ANORatingType.SafeANO)
                resultANORatingType = ANORatingType.SafeANO;
            else if (totalRatingValue <= (int)ANORatingType.DangerANO)
                resultANORatingType = ANORatingType.DangerANO;
            else if (totalRatingValue <= (int)ANORatingType.VeryDangerousANO)
                resultANORatingType = ANORatingType.VeryDangerousANO;
            
            
            if(resultANORatingType is ANORatingType.None) ErrorCode.SendError(ErrorCode.ErrorCodeEnum.ANORatingType);
            
            return resultANORatingType;
        }
        
        /// <summary>
        /// ANO 정신력 피해 계산
        /// </summary>
        private int CalculateMentalityDamagePoint(int value)
        {
            // 등급을 결정할 범위와 각 등급에 해당하는 값
            int[] thresholds = { 0, 26, 51, 66 };
            int[] grades = { 0, 1, 2, 3 };

            // 주어진 값이 어느 범위에 속하는지 확인하고 해당하는 등급을 반환
            for (int i = thresholds.Length - 1; i >= 0; i--)
            {
                if (value >= thresholds[i])
                {
                    return grades[i];
                }
            }

            // 범위에 해당하는 등급이 없는 경우 0 반환
            return 0;
        }

        #endregion

        #region ANO 보고 & 배치
        

        /// <summary>
        /// ANO 보고 시스템
        /// </summary>
        public bool Reporting()
        {
            return ANO_Replace();
        }
        
        /// <summary>
        /// ANO 조치 확인 후 배치
        /// </summary>
        /// <param name="customReplaceType">강제로 선별 기준 설정</param>
        public bool ANO_Replace(ANO_ReplaceType customReplaceType = ANO_ReplaceType.None)
        {
            bool isAll_AnoClear = false;
            // ANO 선별 여부
            IsSucessfullANO_Replace = false;
            // 선별기준 선정
            ANO_ReplaceType replaceType;

            // 마지막 라운드 경우
            if (HotelManager.Instance.gameRound.IsLastRound())
            {
                // ANO가 모두 해결되었는지 확인
                if (ANO_ClearCheck(_currentANO_List))
                {
                    // 전 라운에 배치된 ANO 오브젝트 모두 제거
                    Current_ANO_ObjectAllDelete();
                    EnableAll_ANO_IdleObject();
                    return HotelManager.Instance.gameRound.IsGameClear = true;
                }
            }
            
            // 현재 라운드 증가
            HotelManager.Instance.gameRound.IncreaseRoundCount();
            
            if (customReplaceType == ANO_ReplaceType.None)
            {
                // 선별기준 선정
                replaceType = ANO_ReplaceTypeCheck(_currentANO_List, out isAll_AnoClear);
            }
            else
            {
                // 사용자 지정 선별
                replaceType = customReplaceType;
            }

            // 에디터 확인용
            currentType = replaceType;
            
            // 선별 기준 선정 실패 시 예외처리
            if (replaceType == ANO_ReplaceType.None)
            {
                return IsSucessfullANO_Replace = false;
            }
            
            
            // 최종 선정된 ANO Id 값
            int[] selectANO_IdList = ANO_ReplaceSelection(replaceType);

            // 선별 실패 시 예외처리
            if (selectANO_IdList == null)
            {
                return IsSucessfullANO_Replace = false;
            }

            // 전 라운에 배치된 ANO 오브젝트 모두 제거
            Current_ANO_ObjectAllDelete();
            EnableAll_ANO_IdleObject();
            
            // ANO 배치 선별에 성공  배치할 ANO가 있다는 의미 (-1인 경우 배치할 ANO가 없다는 의미)
            if (selectANO_IdList[0] != -1)
            {
                // 최종 ANO Id 값을 이용해 ANO 프리팹 생성
                foreach (var anoId in selectANO_IdList)
                {
                    ANO_CloneData_Legacy anoCloneDataLegacy = GetANO_CloneDataToId(anoId);
                    AddNewANO(anoCloneDataLegacy.ANOId);
                }
            }
            // ANO 배치 선별에 성공, 하지만 배치할 ANO가 없음
            else if(selectANO_IdList[0] == -1)
            {
                // 선별기준 5 카운터 +1 증가
                anoReplaceSettings.currentType5Count = Mathf.Clamp(anoReplaceSettings.currentType5Count + 1, 0, anoReplaceSettings.type5LimitCount);
            }

            return IsSucessfullANO_Replace = true;
        }
        

        /// <summary>
        /// ANO 선별기준 설정 (기존에 배치된 ANO 조치 검사진행)
        /// </summary>
        private ANO_ReplaceType ANO_ReplaceTypeCheck(List<ANO_Component> anoList, out bool isAll_AnoClear)
        {
            ANOTier clearAnoTier = ANOTier.None;
            isAll_AnoClear = ANO_ClearCheck(anoList, out clearAnoTier);

            // 선별기준 5에 도달했으면 선별기준 5로 다른 선별기준을 무시하고 5로 진행
            if (anoReplaceSettings.currentType5Count >= anoReplaceSettings.type5LimitCount)
            {
                anoReplaceSettings.currentType5Count = 0;
                return ANO_ReplaceType.Type5;
            }

            // ANO 모두 조치 완료
            if (isAll_AnoClear)
            {
                // MA2 클리어 시
                if (clearAnoTier == ANOTier.MA2)
                {
                    return ANO_ReplaceType.Type1;
                }
                // 조치 가능한 ANO를 모두 완료 시 
                else
                {
                    // MA1 모두 조치 그리고 3라운드 이하
                    if (HotelManager.Instance.gameRound.CurrentRoundCount <= anoReplaceSettings.placeType3Round)
                    {
                        return ANO_ReplaceType.Type3;
                    }
                    // MA1 모두 조치 그리고 4라운드 이상
                    else if (HotelManager.Instance.gameRound.CurrentRoundCount >= anoReplaceSettings.placeType4Round)
                    {
                        return ANO_ReplaceType.Type4;
                    }
                }
            }
            // MA1 미조치 후 보고했을 시
            else
            {
                return ANO_ReplaceType.Type2;
            }

            ErrorCode.SendError(ErrorCode.ErrorCodeEnum.ANOReplaceType);
            return ANO_ReplaceType.None;
        }


        /// <summary>
        /// ANO 조치 확인
        /// </summary>
        private bool ANO_ClearCheck(List<ANO_Component> anoList, out ANOTier clearAnoTier)
        {
            bool isAll_AnoClear = true;
            clearAnoTier = ANOTier.None;
            
            foreach (var ano in anoList)
            {
                // 현재 ANO 티어 판별
                clearAnoTier = ano.AnoCloneDataLegacy.AnoTier;

                // 조치 가능한 ANO인지 판단
                if (ano.AnoCloneDataLegacy.AnoSolve == ANOSolve.SimpleSolve)
                {
                    // 조치 가능한 ANO가 있지만 조치를 하지 않은 경우 실패로 판정
                    if (!ano.Reporting_ANO_Clear())
                    {
                        isAll_AnoClear = false;
                        // 현재라운드를 1라운드로 변경
                        HotelManager.Instance.gameRound.SetRoundCount(1);
                        break;
                    }
                }
                /*
                else if (clearAnoTier == ANOTier.MA2)
                {
                    // 접촉 ANO가 있지만 조치를 하지 않은 경우 실패로 판정
                    if (!ano.Reporting_ANO_Clear())
                    {
                        isAll_AnoClear = false;
                        NoManualGameManager.Instance.DecreaseRoundCount(1);
                        break;
                    }
                }
                */
            }

            return isAllAnoClear = isAll_AnoClear;
        }
        
        /// <summary>
        /// ANO 조치 확인 [마지막 라운드 판별]
        /// </summary>
        private bool ANO_ClearCheck(List<ANO_Component> anoList)
        {
            ANOTier dummyClearAnoTier; // 이 변수는 실제 사용되지 않지만, 오버로딩된 다른 함수를 호출하기 위해 더미 변수
            return ANO_ClearCheck(anoList, out dummyClearAnoTier);
        }

        /// <summary>
        /// 확률을 계산해 선별된 ANO ID 얻기
        /// </summary>
        private int[] ANO_ReplaceSelection(ANO_ReplaceType replaceType)
        {
            // ANO_ReplaceType 기준으로 확인
            foreach (var anoReplace in anoReplaceSettings.anoReplaceList)
            {
                if (anoReplace.anoReplaceType == replaceType)
                {
                    float maxPercent = 0f;
                    float percent = 0f;
                    
                    foreach (var anoReplaceSettingValue in anoReplace.anoReplaceSettingValuesList)
                    {
                        maxPercent = anoReplaceSettingValue.anoReplacePercent / 100f;
                    }
                         //   Debug.Log("현재 라운드의 최고 확률은 " + maxPercent);

                    percent = Random.Range(0.0f, maxPercent);
                    
                         //   Debug.Log("현재 컴퓨터가 뽑은 확률 " + percent);
                    
                    this.percent = percent;
                    
                    foreach (var anoReplaceSettingValue in anoReplace.anoReplaceSettingValuesList)
                    {
                        /* 최대확률이 고정일 때 Exception
                        if (cumulativeProbability > 1.0f)
                        {
                            Debug.LogWarning("누적 확률 중첩 : " + cumulativeProbability);
                            cumulativeProbability = 0.9f;
                            // ErrorCode.SendError(ErrorCode.ErrorCodeEnum.ANOReplacePercent);
                            // return null;
                        }
                        */
                        
                        // 선정한 확률에 맞으면 ID 값 얻기
                        if (anoReplaceSettingValue.anoReplacePercent/100f >= percent)      
                        {
                            // 배치할 ANO가 없으면 생략
                            if (anoReplaceSettingValue.createANOCount <= 0)
                            {
                                // -1 배열을 전달
                                return new int[] { -1 };
                            }
                            return GetRandom_ANO_ID(anoReplaceSettingValue.createANO_Tire, anoReplaceSettingValue.createANOCount);
                        }
                    }
                }
            }
            
            ErrorCode.SendError(ErrorCode.ErrorCodeEnum.ANOReplacePercent);
            return null;
        }
        
        #endregion

    
    }

    /// <summary>
    /// ANO 복사 데이터
    /// </summary>
    public sealed class ANO_CloneData_Legacy
    {
        public int ANOId;
        public TaskHandler.TaskType TaskType;
        public Sprite ANOSprite;
        public string ANOTitle;
        public string ANODescription;
        public ANOTier AnoTier;
        public ANOType AnoType;
        public ANOSolve AnoSolve;
        public GameObject ANOPrefab;
        public GameObject ANO_IdlePrefab;
        
        public ANO_DataScriptable_Legacy.ANORatingSettings anoRatingSettings;
        public ANO_DataScriptable_Legacy.ANORatingSettings.ANODeteriorationSettings anoDeteriorationSettings;

        public ANO_CloneData_Legacy(ANO_DataScriptable_Legacy anoDataScriptableLegacy)
        {
            ANOId = anoDataScriptableLegacy.ANOId;
            TaskType = anoDataScriptableLegacy.TaskType;
            ANOSprite = anoDataScriptableLegacy.ANOSprite;
            ANOTitle = anoDataScriptableLegacy.ANOTitle;
            ANODescription = anoDataScriptableLegacy.ANODescription;
            AnoTier = anoDataScriptableLegacy.AnoTier;
            AnoType = anoDataScriptableLegacy.AnoType;
            AnoSolve = anoDataScriptableLegacy.AnoSolve;
            ANOPrefab = anoDataScriptableLegacy.ANOPrefab;
            ANO_IdlePrefab = anoDataScriptableLegacy.ANO_IdlePrefab;

            anoRatingSettings = anoDataScriptableLegacy.AnoRatingSettings;
            anoDeteriorationSettings = anoDataScriptableLegacy.AnoRatingSettings.AnoDeteriorationSettings;
        }
    } 
}




