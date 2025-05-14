using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NoManual.UI
{
    public class UI_ReservationItemSlot : UI_DropSlot
    {
        /// <summary>
        /// 현재 슬롯 영역 내부에서 드롭을 했을 때 (1회 호출)
        /// </summary>
        public override void OnDrop(PointerEventData eventData)
        {
            GameObject pointerDrag = eventData.pointerDrag;
    
            // IDragItem 인터페이스를 가진 오브젝트만 드래그 Drop 허용
            if (pointerDrag != null && pointerDrag.TryGetComponent(out IDragItem dragItem))
            {
                dragItem.SetParentAndPosition(targetListTransform);
            }
        }
    }
}


