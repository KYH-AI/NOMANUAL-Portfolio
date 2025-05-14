using UnityEngine;

namespace NoManual.ANO
{
    
    [CreateAssetMenu(fileName = "ANO Name",menuName = "ANO/ANO_DataScriptable")]
    public class ANO_DataScriptable : ScriptableObject
    {
        [Header("ANO 고유 ID")] public int ANO_Id;
        [Space(5)]
        [Header("ANO 해결방법")] public ANOSolve ANO_SolveType = ANOSolve.None;
        [Space(5)]
        [Header("ANO 추가 연계 ID (-1 기본 값)")] public int ANO_Link_Id;
        [Space(5)]
        [Header("ANO 프리팹")] public GameObject ANO_Prefab;
        [Space(5)]
        [Header("ANO Idle 유무 확인")] public bool HaveAnoIdle = false;
        [Space(5)] 
        [Header("기본 정신력 데미지")] [Range(0, 100)] public int MentalityDamage;
    }
    
    public class ANO_CloneData
    {
        public int ANO_Id;
        public ANOSolve ANO_SolveType = ANOSolve.None;
        public int ANO_Link_Id;
        public GameObject ANO_Prefab;
        public bool HaveAnoIdle;
        public float MentalityDamage;

        public ANO_CloneData(int anoId, ANOSolve solveType, int anoLinkId, GameObject anoPrefab, bool haveAnoIdle, int dmg)
        {
            this.ANO_Id = anoId;
            this.ANO_SolveType = solveType;
            this.ANO_Link_Id = anoLinkId;
            this.ANO_Prefab = anoPrefab;
            this.HaveAnoIdle = haveAnoIdle;
            this.MentalityDamage = dmg;
        }
    }

}
