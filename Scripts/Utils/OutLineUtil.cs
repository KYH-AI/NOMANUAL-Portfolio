using UnityEngine;

namespace NoManual.Utils
{
    /// <summary>
    /// 아웃라인 추가, 제거
    /// </summary>
    public  class OutLineUtil 
    {
        /// <summary>
        /// 아웃라인 추가
        /// </summary>
        public static void AddOutLine(GameObject target, QuickOutline.Mode mode, Color color, float width)
        {
           QuickOutline outline = target.AddComponent<QuickOutline>();
           outline.OutlineMode = mode;
           outline.OutlineColor = color;
           outline.OutlineWidth = width;
        }

        /// <summary>
        /// 아웃라인 제거
        /// </summary>
        public static void RemoveOutLine(GameObject target)
        {
            if (target.TryGetComponent(out QuickOutline outline))
            {
                Object.Destroy(outline);
            }
        }
    }
}


