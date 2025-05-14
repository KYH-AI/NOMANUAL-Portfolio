using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace NoManual.UI
{
    /// <summary>
    /// R.S 트레이더스 상품 UI
    /// </summary>
    public class UI_RS_TradersItem : MonoBehaviour
    {
        public int itemId { get; private set; }
        
        [Space(20)]
        [Header("제품 이미지")]
        [SerializeField] private Image itemImage;
        [Header("제품 이름")]
        [SerializeField] private TextMeshProUGUI itemTitle;
        [Header("제품 가격")] 
        [SerializeField] private TextMeshProUGUI itemPrice;
        [Header("제품 장바구니 담기 버튼")]
        [SerializeField] private Button addBucketListButton;
        private bool _isListenerAdded = false;
        
        /// <summary>
        /// R.S 트레이더스 제품정보 등록
        /// </summary>
        public void SetRsTradersItemInfo(int itemId, Sprite itemSprite, string itemTitle, string itemPrice, UnityAction<int> btnEvent)
        {
            this.itemId = itemId;
            this.itemImage.sprite = itemSprite;
            this.itemTitle.text = itemTitle;
            this.itemPrice.text = itemPrice;

            if (_isListenerAdded) return;
            addBucketListButton.onClick.AddListener(() => btnEvent(this.itemId));
            _isListenerAdded = true;
        }

        /// <summary>
        /// R.S 트레이더스 제품 초기화 (버튼 이벤트는 제외)
        /// </summary>
        public void ClearRsTradersItemInfo()
        {
            this.itemId = -1;
            this.itemImage.sprite = null;
            this.itemTitle.text = string.Empty;
            this.itemPrice.text = string.Empty;
        }

        private void OnDestroy()
        {
            addBucketListButton.onClick.RemoveAllListeners();
        }
    }
}

