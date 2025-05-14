using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace NoManual.UI
{
    public class UI_RS_BucketList : MonoBehaviour
    {
        public int itemId { get; private set; }
        
        [Space(20)]
        [Header("장비구니 제품 제거 버튼")]
        [SerializeField] private Button deleteButton;
        [Header("제품 수량 증가 버튼")]
        [SerializeField] private Button itemStackAddButton;
        [Header("제품 수량 감소 버튼")]
        [SerializeField] private Button itemStackSubButton;
        [Header("제품 이미지")]
        [SerializeField] private Image itemImage;
        [Header("제품 이름")]
        [SerializeField] private TextMeshProUGUI itemTitle;
        [Header("제품 단가")] 
        [SerializeField] private TextMeshProUGUI itemUnitPrice;
        [Header("제품 수량")]
        [SerializeField] private TextMeshProUGUI itemStack;
        [Header("제품 총가격")]
        [SerializeField] private TextMeshProUGUI itemTotalPrice;
        private bool _isListenerAdded = false;
        private RS_TradersItemCloneData _bucketItemData;
      
        /// <summary>
        /// R.S 장바구니 제품정보 등록
        /// </summary>
        public void SetRsBucketListItemInfo(RS_TradersItemCloneData bucketItemData, int itemId, string itemTitle, int itemPrice, Sprite itemSprite, int itemStack,
                                                    UnityAction<UI_RS_BucketList, RS_TradersItemCloneData> deleteEvent, 
                                                    UnityAction<UI_RS_BucketList, RS_TradersItemCloneData> stackAddEvent, 
                                                    UnityAction<UI_RS_BucketList, RS_TradersItemCloneData> stackSubEvent)
        {
            this._bucketItemData = bucketItemData;
            this.itemId = itemId;
            this.itemTitle.text = itemTitle;
            this.itemUnitPrice.text = $"{itemPrice}$";
            this.itemImage.sprite = itemSprite;
            this.itemStack.text = itemStack.ToString();

            if (_isListenerAdded) return;
            deleteButton.onClick.AddListener(() => deleteEvent(this, _bucketItemData));
            itemStackAddButton.onClick.AddListener(() => stackAddEvent(this, _bucketItemData));
            itemStackSubButton.onClick.AddListener(() => stackSubEvent(this, _bucketItemData));
            _isListenerAdded = true;
        }

        /// <summary>
        /// R.S 장바구니 제품정보 초기화
        /// </summary>
        public void ClearRsBucketListItemInfo()
        {
            this.itemId = -1;
            this.itemTitle.text = string.Empty;
            this.itemUnitPrice.text = string.Empty;
            this.itemImage.sprite = null;
            this.itemStack.text = string.Empty;
            this._bucketItemData = null;
        }

        /// <summary>
        /// 장바구니 제품 총 수량 및 가격 업데이트
        /// </summary>
        public void UpdateItemStackAndPriceText(string itemStack, string totalPrice)
        {
            this.itemStack.text = itemStack;
            itemTotalPrice.text = $"{totalPrice}$";
        }
        
        private void OnDestroy()
        {
            deleteButton.onClick.RemoveAllListeners();
            itemStackAddButton.onClick.RemoveAllListeners();
            itemStackSubButton.onClick.RemoveAllListeners();
        }

    }
}


