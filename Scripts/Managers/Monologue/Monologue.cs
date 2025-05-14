using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NoManual.NPC;
using NoManual.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace NoManual.Managers
{
    public class Monologue : MonoBehaviour
    {
        private readonly string _MONOLOUGE_CSV_PATH = "CSVData/MonologueCSV";
        [SerializeField] private bool debugMode = false;
        [SerializeField] private int debugClearDay = 0;
        [SerializeField] private EndingType debugEndingType = EndingType.None;
        [SerializeField] private ProcessType debugProcessType = ProcessType.Start;
        
        [Space(10)]
        [SerializeField] private AudioClipDataBaseScriptable subtitleAudioDB;
        [SerializeField] private NPC_DataScriptable playerSubtitleData;
        [SerializeField] private UI_SkipButton skipButton;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private AudioSource monologueBgmAudio;
        [SerializeField] private AudioSource monologueSubtitleAudio;

        public static bool EndingCredit = false;
        private bool _loadingScene = false;
        private bool _skip = false;
        private Coroutine _monologueCoroutine;
        private Tween _bgmAudioFade;
        private readonly Dictionary<EndingType, Dictionary<int, MonologueData>> _monologueList = new Dictionary<EndingType, Dictionary<int, MonologueData>>();
        private GameManager.SceneName _nextScene;
        
        private void Awake()
        {
            ReadCsvMonologueFile();
        }

        public void Start()
        {
            StopMonologue();
            PlayMonologue();
        }


       
        private void Update()
        {
            if (skipButton.UpdateSkipUiButton() && !_skip)
            {
                _skip = true;
                StopMonologue(true);
            }
        }

        /// <summary>
        /// 독백 CSV 파일 읽기
        /// </summary>
        private void ReadCsvMonologueFile()
        {
            string[] ParseMonologueCsvFile(string[] values)
            {
                string[] parseData = new string[values.Length];
                for (int i = 0; i < values.Length; i++) parseData[i] = values[i];
                return parseData;
            }
            
            var csvHelper = new CSVFileHelper<string[]>(ParseMonologueCsvFile);
            List<string[]> parsedData = csvHelper.ParseCSV(_MONOLOUGE_CSV_PATH);
            
                       
            List<string> textId = new List<string>();
            EndingType endingType = EndingType.None;
            EndingType lastEndingType = EndingType.None;
            int lastDay = -1;
            foreach (var data in parsedData)
            {

                // 1. Day 파싱
                if (!NoManualUtilsHelper.FindStringEmptyOrNull(data[1]))
                { 
                    // 2. EndingType 파싱
                    if (!NoManualUtilsHelper.FindStringEmptyOrNull(data[0]))
                    {
                        if (!Enum.TryParse(data[0], true, out endingType))
                        {
                            NoManualUtilsHelper.EditorDebugLog(NoManualUtilsHelper.LogTextColor.red ,$"독백 CSV EndingType 형변환 오류 : {data[0]} !");
                            continue;
                        }
                        if (!_monologueList.ContainsKey(endingType))
                        {
                            if (lastEndingType != endingType && lastDay != -1)
                            {
                                _monologueList[lastEndingType][lastDay] = new MonologueData(textId.ToArray());
                            }
                            _monologueList[endingType] = new Dictionary<int, MonologueData>();
                            lastEndingType = endingType;
                        }
                    }
                    
                    if (!int.TryParse(data[1], out int newDay))
                    {
                        NoManualUtilsHelper.EditorDebugLog(NoManualUtilsHelper.LogTextColor.red ,$"독백 CSV Day 형변환 오류 : {data[1]} !");
                        continue;
                    }

                    if (lastDay != -1 && lastDay != newDay)
                    {
                        _monologueList[lastEndingType][lastDay] = new MonologueData(textId.ToArray());
                        textId.Clear();
                    }
                    lastDay = newDay; 
                }
                textId.Add(data[2]);
            }
            
            // 마지막 Day 저장
            if (lastDay != -1)  _monologueList[lastEndingType][lastDay] = new MonologueData(textId.ToArray());
     
           
            // Debug.Log로 최종 정리된 데이터 출력
            /*
            foreach (var entry in _monologueList)
            {
                EndingType type = entry.Key;
                foreach (var dayEntry in entry.Value)
                {
                    int day = dayEntry.Key;
                    MonologueData monologueData = dayEntry.Value;
                    Debug.Log($"EndingType: {type}, Day: {day}, TextIds: {string.Join(", ", monologueData.TextId)}, AudioIds: {string.Join(", ", monologueData.AudioId)}");
                }
            }
            */
        }


        /// <summary>
        /// 독백 시작
        /// </summary>
        private void PlayMonologue()
        {
            BgmAudioFade(true);
            SaveGameManager saveGM = GameManager.Instance.SaveGameManager;
            MonologueData monologueData;
            
            if (debugMode)
            {
                monologueData = _monologueList[debugEndingType][debugClearDay];
            }
            else
            {
                monologueData = _monologueList[saveGM.CurrentPlayerSaveData.EndingType][saveGM.CurrentPlayerSaveData.Day];
            }

            if (monologueData == null)
            {
                NoManualUtilsHelper.EditorDebugLog(NoManualUtilsHelper.LogTextColor.red, 
                    $"독백 오류 : {saveGM.CurrentPlayerSaveData.EndingType}의 {saveGM.CurrentPlayerSaveData.Day} 독백 데이터가 존재하지 않습니다!");
                return;
            }

#if UNITY_EDITOR
                if(debugMode) SetNextScene(debugEndingType, debugProcessType);
#else                
                debugMode = false;
#endif
               if(!debugMode) SetNextScene(saveGM.CurrentPlayerSaveData.EndingType, saveGM.CurrentPlayerSaveData.EndingProcess);
      

            subtitleText.gameObject.SetActive(true);
            skipButton.gameObject.SetActive(true);
            monologueBgmAudio.loop = true;
            monologueBgmAudio.Play();
            monologueSubtitleAudio.loop = false;
            _monologueCoroutine = StartCoroutine(MonologueProcess(monologueData));
        }

        /// <summary>
        /// 독백 코루틴
        /// </summary>
        private IEnumerator MonologueProcess(MonologueData monologueData)
        {
            GameManager gm = GameManager.Instance;

            yield return new WaitForSeconds(1f);
            
            for (int i = 0; i < monologueData.TextId.Length; i++)
            {
               string subtitle = gm.localizationTextManager.GetText(LocalizationTable.TextTable.Monologue_Table, monologueData.TextId[i]);
               AudioClip audioClip = null;
               float syncTimer = 0f;
               foreach (var subTitleData in playerSubtitleData.NpcSubtitlesTextKeys)
               {
                   if (subTitleData.NpcSubtitlesTextKey.ToString().Equals(monologueData.TextId[i]))
                   {
                       audioClip = subtitleAudioDB.GetAudioClip(subTitleData.NpcSubtitlesTextKey);
                       syncTimer += subTitleData.subtitlesOffset + audioClip.length;
                   }
                 
               }
               // 자막
               subtitleText.text = subtitle;
               // 음성
               if (audioClip)
               {
                   monologueSubtitleAudio.clip = audioClip;
                   monologueSubtitleAudio.Play();
               }

               yield return new WaitForSeconds(syncTimer);

               subtitleText.text = string.Empty;
               monologueSubtitleAudio.Stop();
            }
    
            StopMonologue(true);
        }

        /// <summary>
        /// 독백 종료
        /// </summary>
        private void StopMonologue(bool isSceneLoad = false)
        {
            if (_monologueCoroutine != null)
            {
                StopCoroutine(_monologueCoroutine);
                _monologueCoroutine = null;
            }
            
            monologueSubtitleAudio.Stop();
            subtitleText.text = string.Empty;
            subtitleText.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(false);
            BgmAudioFade(false, isSceneLoad ? new List<UnityAction> { NextMoveScene } : null);
        }

        /// <summary>
        /// 씬 할당
        /// </summary>
        private void SetNextScene(EndingType endingType, ProcessType isEnding)
        {
            // 엔딩 크레딧 이동
            if (EndingCredit)
            {
                _nextScene = GameManager.SceneName.Ending_Credit;
                EndingCredit = false;
                return;
            }
            
            if (isEnding == ProcessType.End)
            {
                if (endingType == EndingType.A)
                {
                    _nextScene = GameManager.SceneName.Ending_A;
                }
                else if (endingType == EndingType.B)
                {
                    _nextScene = GameManager.SceneName.Ending_B;
                }
                else
                {
                    _nextScene = GameManager.SceneName.Ending_None;
                }

                _loadingScene = true;
            }
            else
            {
                _nextScene = GameManager.SceneName.Adjustment;
            }
        }
        
        /// <summary>
        /// 씬 전환
        /// </summary>
        private void NextMoveScene()
        {
            
            #if UNITY_EDITOR
                if (debugMode)
                {
                    NoManualUtilsHelper.EditorDebugLog(NoManualUtilsHelper.LogTextColor.cyan, $"{_nextScene} 씬으로 이동!");
                    return;
                }
            #endif

            if (_loadingScene)
            {
                GameManager.Instance.NextScene = _nextScene;
                GameManager.Instance.OpenScene(GameManager.SceneName.Loading);
            }
            else
            {
                GameManager.Instance.OpenScene(_nextScene);
            }
            
        }


        /// <summary>
        /// 독백 BGM 오디오 페이드
        /// </summary>
        private void BgmAudioFade(bool isFadeIn, List<UnityAction> sceneLoad = null)
        {
            if (_bgmAudioFade != null && _bgmAudioFade.IsActive())
            {
                _bgmAudioFade.Kill();  
                _bgmAudioFade = null; 
            }
            _bgmAudioFade = DOTweenManager.FadeAudioSource(isFadeIn, monologueBgmAudio, 3f, isFadeIn ? 0.55f : 0f, sceneLoad);
        }
    }
}


