using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoManual.Inventory
{
    /// <summary>
    /// 매뉴얼 북 ScriptableObject DB
    /// </summary>
    [CreateAssetMenu(menuName = "Manual Book/ManualBookDataBase")]
    public class ManualBookDataBaseScriptable : ScriptableObject
    {
        public List<ManualBookScriptable> ManualBookDataBase = new List<ManualBookScriptable>();

        /// <summary>
        /// 아이템 고유 ID를 이용해 아이템 데이터 얻기
        /// </summary>
        public ManualBookScriptable GetItemDataToItemId(int itemId)
        {
            foreach (var manualBookScriptable in ManualBookDataBase)
            {
               if (manualBookScriptable.manualBookId == itemId) return manualBookScriptable;
            }
            return null;
        }
    }
}
