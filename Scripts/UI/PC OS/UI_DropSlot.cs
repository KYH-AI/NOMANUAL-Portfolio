using UnityEngine;
using UnityEngine.EventSystems;

namespace NoManual.UI
{
    public abstract class UI_DropSlot : MonoBehaviour, IDropSlot, IDropHandler
    {
        [SerializeField] protected RectTransform targetListTransform;

        public Transform GetDropSlotTransform()
        {
            return targetListTransform;
        }
        public abstract void OnDrop(PointerEventData eventData);
    }
}

