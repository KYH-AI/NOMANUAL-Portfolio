using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NoManual.Inventory
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Item/ItemScriptable")]
    public class ItemScriptable : ScriptableObject
    {
        [Tooltip("ID")] public int itemId = -1;
        [Tooltip("이름")] public string itemTitle = "New Item";
        [Tooltip("이미지")] public Sprite itemIcon = null;
        [Tooltip("설명")] [Multiline] public string description = "New Item Description";
        [Tooltip("사용 기준")] public InventoryItemUseType inventoryItemUseType = InventoryItemUseType.None;
        [Tooltip("버리기 프리팹")] public GameObject dropPrefab = null;

        [Serializable]
        public sealed class Toggle
        {
            [Tooltip("중첩 가능 여부")] public bool isStackable;
            [Tooltip("읽기 여부")] public bool UsageOp_Read;
            [Tooltip("장비 여부")] public bool UsageOp_Equip;
            [Tooltip("수동 퀵슬롯 사용 여부")] public bool UsageOp_BindShortcut;
            [Tooltip("자동 퀵슬롯 필요 여부")] public bool autoBindShortcut;
            [Tooltip("버리기 여부")] public bool UsageOp_Drop;
            [Tooltip("조합 여부")] public bool UsageOp_Combine;
            [Tooltip("섭취 여부")] public bool UsageOp_Eat;
        }

        [Serializable]
        public sealed class Settings
        {
            [Tooltip("장비 착용 ID (기본 값 : -1")] public int equipItemId = -1;
            [Tooltip("읽기 ID (기본 값 : -1")] public int readItemId = -1;
            [Tooltip("아이템 최대 충접 가능 계수 (무제한 : 0)")] public int maxStackAmount;
            [Tooltip("한 가방에 최대 허용 아이템 개수 (무제한 : 0)")] public int maxBagAmount;
            [Tooltip("정신력 회복 계수")] [Range(0f, 1f)] public float mentalityAmount;
            [Tooltip("스테미나 회복 계수")] [Range(0f, 1f)] public float staminaAmount;
            [Tooltip("R.S 트레이더스 판매가격 (생략가능)")] public int itemPrice;
        }

        [Serializable]
        public sealed class CombineSettings
        {
            [Tooltip("조합에 사용될 두 번째 아이템의 ID")] public int combineWithID;
            [Tooltip("두 아이템을 조합하여 얻은 결과 아이템 ID")] public int resultCombineID;
            [Tooltip("결과 아이템 개수")] public int resultCombineItemAmount;
            [Tooltip("두 아이템을 조합하여 얻은 결과를 착용아이템 선택하는 데 사용되는 아이템의 ID")] public int combineSwitcherID;
            [Tooltip("결과 아이템을 즉시 착용하기")] public bool combineShowItem;
            [Tooltip("두 아이템을 조합하면 인벤토리에 추가되는 새 아이템을 생성")] public bool combineAddItem;
            [Tooltip("현재 아이템을 이용해 조합하면 현재 아이템을 소모하지 않음")] public bool combineKeepItem;
        }

        [Serializable]
        public sealed class Sounds
        {
            [Tooltip("사용 사운드")] public AudioClip useSound;
            [Range(0, 1f)] public float useVolume = 1f;

            [Tooltip("조합 사운드")] public AudioClip combineSound;
            [Range(0, 1f)] public float combineVolume = 1f;
        }

        public Toggle itemToggle = new Toggle();
        public Settings itemSettings = new Settings();
        public Sounds itemSounds = new Sounds();
        public CombineSettings[] combineSettings;

    }
}