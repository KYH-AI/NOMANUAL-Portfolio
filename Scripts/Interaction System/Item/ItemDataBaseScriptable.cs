using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoManual.Inventory
{
    public enum InventoryItemUseType
    {
        None = -1,  
        Manual = 0,    // 매뉴얼
        Equipment = 1, // 착용 아이템
        Key = 2,       // 열쇠
        ItemPart = 3,  // 조합
        Food = 4,      // 섭취
        Bullet = 5,    // 총알
        Service = 6,   // 서비스
        Etc = 7,       // 기타
    }
    
    /// <summary>
    /// 아이템 ScriptableObject DB
    /// </summary>
    [CreateAssetMenu(menuName = "Item/ItemDataBase")]
    public class ItemDataBaseScriptable : ScriptableObject
    {
       public List<ItemScriptable> itemDataBase = new List<ItemScriptable>();

       /// <summary>
      /// 아이템 고유 ID를 이용해 아이템 데이터 얻기
      /// </summary>
      public ItemScriptable GetItemDataToItemId(int itemId)
      {
          foreach (var itemScriptable in itemDataBase)
          {
              if (itemScriptable.itemId == itemId) return itemScriptable;
          }
          
          return null;
      }
      
    }

}

