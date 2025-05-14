using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NoManual.UI
{
    public class UI_ReservationItemDragDrop : MonoBehaviour, IDragItem, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private RectTransform _rectTransform; // 현재 UI Rect 컴포넌트
        private Transform _root; // UI root Canvas
        private Transform _previousParentTransform; // 마지막 UI Root
        private CanvasGroup _canvasGroup;
        private Image _backgroundImage;
        private Color _originalColor;
        [SerializeField] private Color targetColor;
        [SerializeField] private TextMeshProUGUI customerNameText;
       
        
        public void InitReservationItemUI()
        {      
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _backgroundImage = GetComponent<Image>();
            _originalColor = _backgroundImage.color;
        }

        public void SetReservationData(string customerName, Transform root)
        {
            this.customerNameText.text = customerName;
            this._root = root;
            this._previousParentTransform = transform.parent;
        }

        public void ClearReservationData()
        {
            this.customerNameText.text = string.Empty;
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
        }

        public string GetCustomerName()
        {
            return this.customerNameText.text;
        }

        public void SetParentAndPosition(Transform parent)
        {
            transform.SetParent(parent);
            RectTransform parentRectTransform = parent as RectTransform;
            _rectTransform.position = parentRectTransform != null ? parentRectTransform.position : parent.position;
        }

        public Transform GetCurrentParent()
        {
            return _previousParentTransform;
        }
        
        private void SwapBackGroundColor(Color targetColor)
        {
            _backgroundImage.color = targetColor;
        }

        /// <summary>
        /// 오브젝트 끄는 도중 (매 프레임 호출)
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            // 현재 스크린상의 마우스 위치를 UI 위치로 설정
            _rectTransform.position = eventData.position;
        }

        /// <summary>
        /// 오브젝트를 끌기 시작 시 (1회 호출)
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            // 드래그 직전에 부모 소속 기록
            _previousParentTransform = transform.parent;
            
            // 드래그 중인 레코드 오브젝트가 보고서 패널에 최상단에 출력되기 위해 부모 설정
            transform.SetParent(_root);
            transform.SetAsLastSibling();
            
            _canvasGroup.alpha = 0.5f;
            _canvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// 오브젝트 끌기 종료 (1회 호출)
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            // 드래그 후 슬롯에 도달하지 못하는 경우 다시 원상복구
            if (transform.parent == _root)
            {
                // 드래그 직전에 기록한 부모 소속으로 변경
                SetParentAndPosition(_previousParentTransform);
            }
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SwapBackGroundColor(targetColor);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SwapBackGroundColor(_originalColor);
        }
    }
}
