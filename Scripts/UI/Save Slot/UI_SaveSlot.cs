using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace NoManual.UI
{
    public class UI_SaveSlot : MonoBehaviour
    {
        [Header("세이브 슬롯 위치")]
        [SerializeField] private SaveGameManager.SaveFileSlot saveFileSlot;
            
        [Space(10)]
        [Header("== UI ==")] 
        [SerializeField] private GameObject emptySlot;
        [SerializeField] private GameObject noEmptySlot;
        [SerializeField] private Image slotMapImage;
        [SerializeField] private Sprite[] slotMapImageSprites;
        [SerializeField] private TextMeshProUGUI slotDayText;
        [SerializeField] private TextMeshProUGUI slotRsCashText;
        [SerializeField] private TextMeshProUGUI slotDataTimeText;

        [Space(5)]
        [SerializeField] private Button slotDeleteButton;
        private Button _slotButton;
        
        public void InitSaveSlotUI()
        {
            _slotButton = GetComponent<Button>();
            noEmptySlot.SetActive(false);
            emptySlot.SetActive(false);
        }

        /// <summary>
        /// 세이브 데이터 UI 매핑
        /// </summary>
        public void SetSaveDataMappingUI(PlayerSaveData localSaveData, Action<SaveGameManager.SaveFileSlot> slotClickEvent, Action<SaveGameManager.SaveFileSlot> deleteClickEvent)
        {
            RemoveAllEvent();
            _slotButton.onClick.AddListener(() => slotClickEvent(saveFileSlot));
            if (localSaveData == null)
            {
                noEmptySlot.SetActive(false);
                emptySlot.SetActive(true);
            }
            else
            {
                slotDeleteButton.onClick.AddListener(() => deleteClickEvent(saveFileSlot));
              //  slotMapImage.sprite = !localSaveData.IsTutorialClear ? slotMapImageSprites[0] : slotMapImageSprites[1];
                slotDayText.text = GetPlaceHolderText(slotDayText.text, localSaveData.Day.ToString());
                slotRsCashText.text = GetPlaceHolderText(slotRsCashText.text, localSaveData.RS_Cash.ToString());
                slotDataTimeText.text = localSaveData.SaveDateTime;
                noEmptySlot.SetActive(true);
                emptySlot.SetActive(false);
            }
        }
        
        /// <summary>
        /// 텍스트 Place Holder 매핑
        /// </summary>
        private string GetPlaceHolderText(string originalText, string replacementText)
        {
            string placeholder = LocalizationVariableHelper.GetPlaceHolderString(originalText);
            if (!Utils.NoManualUtilsHelper.FindStringEmptyOrNull(placeholder))
            {
                return originalText.Replace(placeholder, replacementText);
            }
            return originalText;
        }

        private void RemoveAllEvent()
        {
            _slotButton.onClick.RemoveAllListeners();
            slotDeleteButton.onClick.RemoveAllListeners();
        }

        private void OnDestroy()
        {
            RemoveAllEvent();
        }
    }
}


