using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ThunderWire.Input;
using NoManual.UI;

namespace NoManual.Managers
{
    public class ItemShortCutManager : MonoBehaviour
    {
        public static readonly string[] SHORTCUT_INPUTSYSTME_VALUES = new string[3] { "UseItem1", "UseItem2", "UseItem3" };
        private readonly Dictionary<string, ShortCutData> _shortCutDataList = new Dictionary<string, ShortCutData>(3);
        
        private InventoryManager _inventoryManager;
        private UI_NoManualUIManager _uiManager;
        private HFPS.Systems.HFPS_GameManager _gameManager;
        private EquipItemSwitcherManager _equipItemSwitcher;
        private CutSceneManager _cutScene;
        
        private bool _shortCutMode = false;

        /// <summary>
        /// 퀵슬롯 초기화
        /// </summary>
        public void InitializationShortCut(InventoryManager inventoryManager, 
                                          EquipItemSwitcherManager equipItemSwitcherManager, 
                                          UI_NoManualUIManager ui,
                                          CutSceneManager cutScene)
        {
            _equipItemSwitcher = equipItemSwitcherManager;
            _inventoryManager = inventoryManager;
            _uiManager = ui;
            _cutScene = cutScene;
            _gameManager = HFPS.Systems.HFPS_GameManager.Instance;
            
        }

        
        /// <summary>
        /// 아이템 퀵슬롯 세이브 파일
        /// </summary>
        public ShortCutData[] SaveShortCutData()
        {
            return _shortCutDataList.Values.ToArray();
        }

        private void Update()
        {
            // 입력 받기 예외처리
            bool preventUse = false;

            // 게임 일시 정지 및 아이템 드래그 및 컷신 상태인 경우 무시
            if (_gameManager && _inventoryManager)
            {
                preventUse = _gameManager.isInventoryShown || _gameManager.isPaused || _cutScene.CutSceneRunning;
                _shortCutMode = _inventoryManager.IsShortCutMode;
            }

            
            // 퀵슬롯 수동 등록
            if (_shortCutMode)
            {
                string bindKey = IsSpecificActionPressed(SHORTCUT_INPUTSYSTME_VALUES);
                if (!Utils.NoManualUtilsHelper.FindStringEmptyOrNull(bindKey))
                {
                    var scData = _inventoryManager.ShortCutDataMapper;
                    scData.ShortCutKey = bindKey;
                    BindShortCut(scData.ItemId, scData.ItemType, scData.SlotId, scData.ShortCutKey);
                    _inventoryManager.ResetInventory();
                }
            }
            // 퀵슬롯 입력
            else if (!preventUse)
            {
                if (_shortCutDataList.Count > 0)
                {
                    var scDataList = _shortCutDataList.Values.ToList(); // 리스트로 변환하여 수정 가능성 방지
                    for (int i = 0; i < scDataList.Count; i++)
                    {
                        var scData = scDataList[i];
                        if (InputHandler.ReadButtonOnce(this, scData.ShortCutKey))
                        {
                            _inventoryManager.UseQuickSlotItem(scData.ItemType, scData.SlotId, scData.ShortCutKey);
                        }
                    }
                }
            }
        }

        private string IsSpecificActionPressed(params string[] shortcutValues)
        {
            return shortcutValues.SingleOrDefault(inputKey => InputHandler.ReadButtonOnce(this, inputKey));
        }

        /// <summary>
        /// 퀵슬롯 BindKey값 얻기
        /// </summary>
        public string GetBindKeyBaseInventoryItemData(int itemId, InventorySlotType slotType, int slotId)
        {
            return _shortCutDataList.Values.SingleOrDefault(scData => scData.ItemId.Equals(itemId) && scData.ItemType.Equals(slotType) && scData.SlotId.Equals(slotId))?.ShortCutKey;
        }
        
        /// <summary>
        /// 퀵슬롯 등록
        /// </summary>
        public void BindShortCut(int itemId, InventorySlotType slotType, int slotId, string bindKey)
        {
            if (_shortCutDataList.Count > 0)
            {
                // 등록된 퀵슬롯 확인
                var newScData = _shortCutDataList.Values.FirstOrDefault(scData => scData.ItemId.Equals(itemId) && scData.ItemType.Equals(slotType) && scData.SlotId.Equals(slotId));
      
                   // 기존에 있는 퀵슬롯 아이템을 교체하자
                /* 1. 새로 등록하고자는 아이템이 퀵슬롯에 등록되지 않은 경우 그냥 교체 
                 *  2. 만약 퀵슬롯에 등록된 경우 SWAP
                 */
                string tempBindKey = string.Empty;
                ShortCutData tempScData = null;

                // 변경하고자는 위치에 퀵슬롯 데이터가 있는 경우
                if (_shortCutDataList.ContainsKey(bindKey))
                {
                    // 동일한 퀵슬롯 등록 시 예외처리
                    if (_shortCutDataList[bindKey].ItemId.Equals(itemId) &&
                        _shortCutDataList[bindKey].SlotId.Equals(slotId)) return;
                    
                    if (newScData != null)
                    {
                        tempBindKey = newScData.ShortCutKey;
                        tempScData = _shortCutDataList[bindKey];
                    
                        // 덮어쓰기
                        _shortCutDataList[bindKey] = newScData;
                        newScData.ShortCutKey = bindKey;

                        // Swap
                        _shortCutDataList[tempBindKey] = tempScData;
                        tempScData.ShortCutKey = tempBindKey;
                    }
                    else
                    {
                        // 교체된 퀵슬롯 정보 
                        tempBindKey = string.Empty;
                        tempScData = new ShortCutData(_shortCutDataList[bindKey].ItemId, _shortCutDataList[bindKey].ItemType, _shortCutDataList[bindKey].SlotId, tempBindKey);

                        _shortCutDataList[bindKey].ItemId = itemId;
                        _shortCutDataList[bindKey].SlotId = slotId;
                        _shortCutDataList[bindKey].ItemType = slotType;
                    }
                }
                else
                {
                    // 기존 퀵슬롯을 빈 퀵슬롯으로 이동
                    if (newScData != null)
                    {
                        tempBindKey = newScData.ShortCutKey;
                        tempScData = _shortCutDataList[newScData.ShortCutKey];
                        _inventoryManager.RemoveShortCutItemData(tempScData.ItemId, tempScData.ItemType, tempScData.SlotId);
                        tempScData.ShortCutKey = bindKey;
                        _shortCutDataList.Add(bindKey, tempScData);
                        tempScData = null;
                    }
                    // 빈 퀵슬롯에대 새로운 퀵슬롯 데이터
                    else
                    {
                        _shortCutDataList.Add(bindKey, new ShortCutData(itemId, slotType, slotId, bindKey));
                    }
                }
                
                // Swap 퀵슬롯 UI 새로고침
                if (tempScData != null)
                {
                    UI_ChildInventoryPanelEmptySlot tempCipEmptySlot =  NoManualHotelManager.Instance.InventoryManager.GetCipEmptySlot(tempScData.ItemType, tempScData.SlotId);
                    if (tempCipEmptySlot)
                    {
                        // CIP PreView Slot에 퀵슬롯 이미지 등록
                        tempCipEmptySlot.GetCipPanelPreviewSlot().ShortCut = tempBindKey; // 숏컷 문자열 등록
                        Sprite shortCutSprite = _inventoryManager.GetShortCutSprite(tempBindKey);
                        tempCipEmptySlot.GetCipPanelPreviewSlot().SetCipPreviewShortCutIcon(shortCutSprite);
                        if(tempBindKey != string.Empty) _inventoryManager.SetShortCutItemUI(tempBindKey, tempCipEmptySlot);
                    }
                }
            }
            else
            {
                _shortCutDataList.Add(bindKey, new ShortCutData(itemId, slotType, slotId, bindKey));
            }
            
            
            UI_ChildInventoryPanelEmptySlot cipEmptySlot =  NoManualHotelManager.Instance.InventoryManager.GetCipEmptySlot(slotType, slotId);
            if (cipEmptySlot) 
            {
                // CIP PreView Slot에 퀵슬롯 이미지 등록
                cipEmptySlot.GetCipPanelPreviewSlot().ShortCut = bindKey; // 숏컷 문자열 등록
                Sprite shortCutSprite = _inventoryManager.GetShortCutSprite(bindKey);
                cipEmptySlot.GetCipPanelPreviewSlot().SetCipPreviewShortCutIcon(shortCutSprite);
                _inventoryManager.SetShortCutItemUI(bindKey, cipEmptySlot);
            }
            
        }

        /// <summary>
        /// 퀵슬롯 자동 등록
        /// </summary>
        public void AutoBindShortCut(int itemId, InventorySlotType slotType, int slotId)
        {
            if (_shortCutDataList.Count == SHORTCUT_INPUTSYSTME_VALUES.Length) return;
            string autoBindKey = string.Empty;
            // 1, 2, 3 번 퀵슬롯을 순차적으로 검사
            for (int i = 0; i < SHORTCUT_INPUTSYSTME_VALUES.Length; i++)
            {
                string key = SHORTCUT_INPUTSYSTME_VALUES[i];
                if (!_shortCutDataList.ContainsKey(key) || _shortCutDataList[key] == null)
                {
                    autoBindKey = key;
                    break;
                }
            }

            // 비어있는 퀵슬롯이 있다면 해당 키로 바인딩
            if (!Utils.NoManualUtilsHelper.FindStringEmptyOrNull(autoBindKey))
            {
                BindShortCut(itemId, slotType, slotId, autoBindKey);
            }
        }
        

        /// <summary>
        /// 퀵슬롯 데이터 지우기
        /// </summary>
        public bool RemoveBindShortCut(string bindKey)
        {
            if (_shortCutDataList.ContainsKey(bindKey))
            {
                _shortCutDataList.Remove(bindKey);
                return true;
            }
            return false;
        }

        public void EquipItemSwitch(int equipItemId)
        {
            _equipItemSwitcher.SelectSwitcherItem(equipItemId);
        }

        public void DisableEquipItem()
        {
            _equipItemSwitcher.DisableItems();
        }

        public int GetEquipItemId()
        {
            return _equipItemSwitcher.CurrentEquipItemId;
        }
    }


    [System.Serializable]
    public class ShortCutData
    {
        [Header("아이템 ID")]
        public int ItemId;
        [Header("인벤토리 아이템 타임")]
        public InventorySlotType ItemType;
        [Header("인벤토리 슬롯 위치")]
        public int SlotId;
        [Header("퀵슬롯 등록 위치 Key( UseItem1 ~ 3 )")]
        public string ShortCutKey;
        
        public ShortCutData(int itemId, InventorySlotType itemType, int slotId, string shortCutKey)
        {
            this.ItemId = itemId;
            this.ItemType = itemType;
            this.SlotId = slotId;
            this.ShortCutKey = shortCutKey;
        }
    }
}


