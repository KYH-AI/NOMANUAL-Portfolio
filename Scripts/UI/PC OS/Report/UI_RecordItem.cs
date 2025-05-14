using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NoManual.UI
{
    public class UI_RecordItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDragItem, IPointerEnterHandler, IPointerExitHandler
    {

        private RectTransform _rectTransform; // 현재 레코드 Rect 컴포넌트
        private Transform _reportMainRoot; // 리포트 UI Root
        private Transform _previousParentTransform; // 마지막 레코드 리스트 UI Root
        [SerializeField] private TextMeshProUGUI recordTitle;
        [SerializeField] private CanvasGroup recordCanvasGroup;
        [SerializeField] private Color selectColor;

        private Image _backGroundImage;
        // 배경 원본 색상
        private Color _backGroundOriginalColor;
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _backGroundImage = GetComponent<Image>();
            _backGroundOriginalColor = _backGroundImage.color;
        }

        /// <summary>
        /// 리포트 UI 최상단 Root 설정 및 레코드 이름 이름 설정
        /// </summary>
        public void InitializeRecord(string title, Transform reportMainRoot)
        {
            // 레코드 이름 설정
            recordTitle.text = title;
            // 최상위 UI Root 설정
            _reportMainRoot = reportMainRoot;
            // 현재 부모 설정
            _previousParentTransform = transform.parent;
        }

        /// <summary>
        /// 오브젝트를 끌기 시작 시 (1회 호출)
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            // 드래그 직전에 부모 소속 기록
            _previousParentTransform = transform.parent;
            
            // 드래그 중인 레코드 오브젝트가 보고서 패널에 최상단에 출력되기 위해 부모 설정
            transform.SetParent(_reportMainRoot);
            transform.SetAsLastSibling();
            
            recordCanvasGroup.alpha = 0.5f;
            recordCanvasGroup.blocksRaycasts = false;
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
        /// 오브젝트 끌기 종료 (1회 호출)
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            // 드래그 후 슬롯에 도달하지 못하는 경우 다시 원상복구
            if (transform.parent == _reportMainRoot)
            {
                // 드래그 직전에 기록한 부모 소속으로 변경
                SetParentAndPosition(_previousParentTransform);
            }
            
            recordCanvasGroup.alpha = 1f;
            recordCanvasGroup.blocksRaycasts = true;
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

        /// <summary>
        /// 마우스 포인터가 영역 내부로 들어갈 때 (1회 호출)
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            _backGroundImage.color = selectColor;
        }

        /// <summary>
        /// 마우스 포인터가 영역 내부에서 나갈 때 (1회 호출)
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            _backGroundImage.color = _backGroundOriginalColor;
        }
    }
}


