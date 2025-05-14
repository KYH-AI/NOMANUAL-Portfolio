using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RequestManualDataType
{
    None = -1,
    CheckList = 0,
    HandoverCheckList = 1,
}

namespace NoManual.Inventory
{
    /// <summary>
    /// 매뉴얼 북 ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "[?] New Manual Book", menuName = "Manual Book/ManualBookScriptable")]
    public class ManualBookScriptable : ScriptableObject
    {
        [Tooltip("ID")] public int manualBookId = -1;
        [Tooltip("매뉴얼 북에 특별한 데이터가 필요한 경우")] public RequestManualDataType requestManualDataType = RequestManualDataType.None;
        [Tooltip("매뉴얼 북 UI 프리팹")] public GameObject manualBookPrefab;
    }
}
