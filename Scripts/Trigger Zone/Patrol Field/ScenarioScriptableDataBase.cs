using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scenario Object DB
/// </summary>
[CreateAssetMenu(menuName = "Scenario/ScenarioObjectDataBase")]
public class ScenarioScriptableDataBase : ScriptableObject
{
    public ScenarioObjectDictionary scenarioObjectDB;
    
    /// <summary>
    /// 시나리오 오브젝트 가져오기
    /// </summary>
    public GameObject GetScenarioObject(int id)
    {
        return scenarioObjectDB.ContainsKey(id) ? scenarioObjectDB[id] : null;
    }
}

[Serializable]
public class ScenarioObjectDictionary : SerializableDictionary<int, GameObject>{}

