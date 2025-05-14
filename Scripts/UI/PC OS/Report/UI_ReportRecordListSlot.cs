using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NoManual.UI
{
    public class UI_ReportRecordListSlot : MonoBehaviour, IDropHandler, IDropSlot, IPointerEnterHandler, IPointerExitHandler
    {
        // 레코드 List 패널
        [SerializeField] private RectTransform targetListTransform;
        public UnityEvent<IDragItem, IDropSlot> recordOnDropEvent;

        [SerializeField] private Image backGround;
        [SerializeField] private Color backGroundOriginalColor;
        [SerializeField] private Color backGroundEnterColor;
   
        
        /// <summary>
        /// 현재 슬롯 영역 내부에서 드롭을 했을 때 (1회 호출)
        /// </summary>
        public void OnDrop(PointerEventData eventData)
        { 
            GameObject pointerDrag = eventData.pointerDrag;
    
            // IDragItem 인터페이스를 가진 오브젝트만 드래그 Drop 허용
            if (pointerDrag != null && pointerDrag.TryGetComponent(out IDragItem dragItem))
            {
                dragItem.SetParentAndPosition(targetListTransform);
                recordOnDropEvent?.Invoke(dragItem, this);
            }
        }

        public Transform GetDropSlotTransform()
        {
            return targetListTransform;
        }

        /// <summary>
        /// 레코드 오브젝트 모두 제거
        /// </summary>
        public void AllRemoveRecordItem()
        {
            foreach (Transform record in targetListTransform)
            {
                Destroy(record.gameObject);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            backGround.color = backGroundEnterColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            backGround.color = backGroundOriginalColor;
        }
    }
}
