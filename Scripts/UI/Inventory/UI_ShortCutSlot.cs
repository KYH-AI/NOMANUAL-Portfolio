using UnityEngine;
using UnityEngine.UI;

namespace NoManual.UI
{
    public class UI_ShortCutSlot : MonoBehaviour
    {
        [SerializeField] private string bindKeyId;
        [SerializeField] private Image shortCutImage;
        [SerializeField] private GameObject highLightFrame;
        [SerializeField] private Text itemAmountText;
        [SerializeField] private Image itemImage;
        
        public void Init()
        { 
            RemoveShortCutItem();
        }
        
        /// <summary>
        /// 퀵슬롯 ID 할당
        /// </summary>
        public void SetShortCutBindKeyId(string bindKey, Sprite shortCutSprite)
        {
            this.bindKeyId = bindKey;
            this.shortCutImage.sprite = shortCutSprite;
        }

        /// <summary>
        /// 퀵슬롯 ID
        /// </summary>
        public string GetSlotBindKeyId()
        {
            return bindKeyId;
        }

        /// <summary>
        /// 퀵슬롯 할당
        /// </summary>
        public void SetShortCutItem(UI_ChildInventoryPanelEmptySlot cipEmptySlot)
        {
            itemImage.sprite = cipEmptySlot.GetCipPanelPreviewSlot().GetCipPreviewSlotIcon();
            itemImage.enabled = true;
            UpdateShortCutItemAmount(cipEmptySlot.GetCipPanelPreviewSlot().ItemAmount);
        }

        /// <summary>
        /// 퀵슬롯 아이템 개수
        /// </summary>
        public void UpdateShortCutItemAmount(int amount)
        {
            itemAmountText.text = amount.ToString();
        }

        /// <summary>
        /// 퀵슬롯 초기화
        /// </summary>
        public void RemoveShortCutItem()
        {
            if(highLightFrame) highLightFrame.SetActive(false);
            itemAmountText.text = string.Empty;
            itemImage.enabled = false;
            itemImage.sprite = null;
        }

        /// <summary>
        /// 퀵슬롯 하이라이트 활성화
        /// </summary>
        public void EnableShortCutHighLight()
        {
            if(highLightFrame) highLightFrame.SetActive(true);
        }

        /// <summary>
        /// 퀵슬롯 하이라이트 바활성화
        /// </summary>
        public void DisableShortCutHighLight()
        {
            if(highLightFrame) highLightFrame.SetActive(false);
        }
    }
}


