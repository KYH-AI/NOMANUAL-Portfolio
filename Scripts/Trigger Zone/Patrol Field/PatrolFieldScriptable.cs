using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoManual.Patrol
{

    /// <summary>
    /// Patrol Field SO
    /// </summary>
    [CreateAssetMenu(fileName = "[?] Patrol Field", menuName = "Patrol/PatrolFieldScriptable")]
    public class PatrolFieldScriptable : ScriptableObject
    {
        public int PatrolFieldID;
        public GameObject patrolFieldPrefab;
    }
}
