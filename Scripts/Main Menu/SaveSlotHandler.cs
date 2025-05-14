using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace NoManual.UI
{
    public class SaveSlotHandler : MonoBehaviour
    {

        [Space(10)]
        [Header("=== 세이브 슬롯 UI ===")]
        [SerializeField] private GameObject saveSlotPanel;
        [SerializeField] private CanvasGroup saveSlotPanelMainCanvasGroup;
        [SerializeField] private CanvasGroup saveSlotPanelSubCanvasGroup;
        [SerializeField] private UI_SaveSlot[] saveSlots;
        [SerializeField] private GameObject tutorialSkipPanel;
        [SerializeField] private GameObject deleteConfirmPanel;
        [SerializeField] private GameObject continuePanel;
        [SerializeField] private GameObject newGamePanel;
        
        public event Action ButtonClickSfx;

        private readonly float _PANEL_DOTWEEN_SCALE_DURACTION = 0.1f;
        private readonly float _CANVAS_GROUP_ALPHA = 0.25f;
        
        public void InitSaveSlot(PlayerSaveData[] localSaveData)
        {
            for (int i = 0; i < saveSlots.Length; i++)
            {
                saveSlots[i].InitSaveSlotUI();
                saveSlots[i].SetSaveDataMappingUI(localSaveData[i], SlotClickButtonEvent, DeleteClickButtonEvent);
            }
        }

        /// <summary>
        /// 게임 시작 버튼 이벤트
        /// </summary>
        public void EnableSavePanel()
        {
            continuePanel.SetActive(false);
            newGamePanel.SetActive(false);
            tutorialSkipPanel.SetActive(false);
            deleteConfirmPanel.SetActive(false);

            EnableSaveSlotPanelCanvasGroup();
            
            saveSlotPanel.SetActive(true);
        }

        /// <summary>
        /// 뒤로가기 버튼 이벤트
        /// </summary>
        public void DisableSavePanel()
        {
            ButtonClickSfx?.Invoke();
            GameManager.Instance.SaveGameManager.CurrentSaveFileSlot = SaveGameManager.SaveFileSlot.None;
            saveSlotPanel.SetActive(false);
        }
        
        /// <summary>
        /// 세이브 슬롯 관련 팝업 창 열기
        /// </summary>
        private void OpenPanel(GameObject enableTarget)
        {
            ButtonClickSfx?.Invoke();
            PanelDOTween(true, enableTarget.transform);
        }


        public void ClosePanel(GameObject disableTarget)
        {
            ButtonClickSfx?.Invoke();
            PanelDOTween(false, disableTarget.transform);
        }


        /// <summary>
        /// 취소 버튼 이벤트
        /// </summary>
        public void CancelButtonEvent(GameObject disableTarget)
        {
            GameManager.Instance.SaveGameManager.CurrentSaveFileSlot = SaveGameManager.SaveFileSlot.None;
            ClosePanel(disableTarget);
        }

        /// <summary>
        /// 슬롯 정보 등록
        /// </summary>
        private void SetSelectFileSlot(SaveGameManager.SaveFileSlot saveFileSlot)
        {
            GameManager.Instance.SaveGameManager.CurrentSaveFileSlot = saveFileSlot;
        }

        
        /// <summary>
        /// 세이브 슬롯 버튼 클릭 이벤트
        /// </summary>
        private void SlotClickButtonEvent(SaveGameManager.SaveFileSlot saveFileSlot)
        {
            SetSelectFileSlot(saveFileSlot);
            var saveFileData = GameManager.Instance.SaveGameManager.GetSaveFileDataToBaseSaveSlot(GameManager.Instance.SaveGameManager.CurrentSaveFileSlot);
            
            if (saveFileData == null)
            {
                // 새 게임
                OpenPanel(newGamePanel);
            }
            else
            {
                // 게임 계속
                OpenPanel(continuePanel);
            }
        }

        /// <summary>
        /// 세이브 삭제 버튼 이벤트
        /// </summary>
        private void DeleteClickButtonEvent(SaveGameManager.SaveFileSlot saveFileSlot)
        {
            SetSelectFileSlot(saveFileSlot);
            OpenPanel(deleteConfirmPanel);
        }

        /// <summary>
        /// 세이브 파일 삭제 확인 버튼 이벤트
        /// </summary>
        public void DeleteConfirmButtonEvent()
        {
            SaveGameManager.SaveFileSlot deleteSlot = GameManager.Instance.SaveGameManager.CurrentSaveFileSlot;
            if (GameManager.Instance.SaveGameManager.DeleteSaveFile())
            {
                saveSlots[(int)deleteSlot].SetSaveDataMappingUI(GameManager.Instance.SaveGameManager.GetSaveFileDataToBaseSaveSlot(deleteSlot), SlotClickButtonEvent, null);
            }
            
        }
        
        /// <summary>
        /// 새 게임 생성 확인 버튼 이벤트
        /// </summary>
        public void NewGameConfirmButtonEvent()
        {
            OpenPanel(tutorialSkipPanel);
        }

        /// <summary>
        /// 튜토리얼 스킵 확인 버튼 이벤트
        /// </summary>
        public void SkipTutorial(bool isSkip)
        {
            var saveFileData = GameManager.Instance.SaveGameManager.GetSaveFileDataToBaseSaveSlot(GameManager.Instance.SaveGameManager.CurrentSaveFileSlot);
            if (saveFileData == null)
            {
                // 새 게임 생성 후 진행
                GameManager.Instance.SaveGameManager.CreateNewSaveFile(isSkip);
            }
            ButtonClickSfx?.Invoke();
            PlayGame(isSkip ? GameManager.SceneName.Hotel :  GameManager.SceneName.Tutorial);
        }

        /// <summary>
        /// 이어서 진행하기 확인 버튼 이벤트
        /// </summary>
        public void ContinueConfirmButtonEvent()
        {
            GameManager.Instance.SaveGameManager.LoadFile();
            SkipTutorial(GameManager.Instance.SaveGameManager.CurrentPlayerSaveData.IsTutorialClear);
        }

        /// <summary>
        /// 게임 시작
        /// </summary>
        private void PlayGame(GameManager.SceneName nextScene)
        {
            MainMenu mainMenu = GetComponentInParent<MainMenu>();
            
            // Save Slot 패널 Fade Out
            saveSlotPanelMainCanvasGroup.blocksRaycasts = false;
            saveSlotPanelMainCanvasGroup.DOFade(0f, 3f).OnComplete(() => mainMenu.PlayStartGame(nextScene));
        }


        private void DisableSaveSlotPanelCanvasGroup()
        {
            saveSlotPanelSubCanvasGroup.blocksRaycasts = false;
            saveSlotPanelSubCanvasGroup.alpha = _CANVAS_GROUP_ALPHA;
        }

        private void EnableSaveSlotPanelCanvasGroup()
        {
            saveSlotPanelSubCanvasGroup.blocksRaycasts = true;
            saveSlotPanelSubCanvasGroup.alpha = 1f;
        }

        private void PanelDOTween(bool isOpen, Transform panel)
        {
            float startValue;
            float endValue;
            TweenCallback endEvent = null;
            if (!isOpen)
            {
                startValue = 1f;
                endValue = 0f;
                endEvent += () =>  panel.gameObject.SetActive(false);
                endEvent += EnableSaveSlotPanelCanvasGroup;
            }
            else
            {
                startValue = 0f;
                endValue = 1f;
                panel.gameObject.SetActive(true);
                endEvent += () => panel.gameObject.SetActive(false);
                endEvent += () => panel.gameObject.SetActive(true);
                DisableSaveSlotPanelCanvasGroup();
            }
            panel.DOScale(endValue, _PANEL_DOTWEEN_SCALE_DURACTION).From(startValue).OnComplete(endEvent);
        }


        private void OnDestroy()
        {
            ButtonClickSfx = null;
        }
    }
}


