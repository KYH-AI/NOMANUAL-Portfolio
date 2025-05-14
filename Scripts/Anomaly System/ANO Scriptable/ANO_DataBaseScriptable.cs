using System.Collections.Generic;
using System.Linq;
using NoManual.Utils;
using UnityEngine;


namespace NoManual.ANO
{
    [CreateAssetMenu(fileName = "ANO DataBase",menuName = "ANO/ANO_DataBase")]
    public class ANO_DataBaseScriptable : ScriptableObject
    {
        [SerializeField] private ANO_DataScriptable[] anoScriptableList;

        public ANO_DataScriptable GetAnoData(int id)
        {
           return anoScriptableList.FirstOrDefault(anoData => anoData.ANO_Id == id);
        }

        public Dictionary<int, ANO_CloneData> GetAnoAllData()
        {
            Dictionary<int, ANO_CloneData> anoCloneData = new Dictionary<int, ANO_CloneData>();
            foreach (var anoData in anoScriptableList)
            {
                if (anoCloneData.ContainsKey(anoData.ANO_Id))
                {
                    NoManualUtilsHelper.EditorDebugLog(NoManualUtilsHelper.LogTextColor.red, "ANO Scriptable [ANO GameObject Name : "+ anoData.ANO_Prefab.gameObject.name +"] ["+ anoData.ANO_Id +"] ANO Id값 중복");
                    continue;
                }
                
                anoCloneData.Add(anoData.ANO_Id, new ANO_CloneData(
                    anoData.ANO_Id, 
                    anoData.ANO_SolveType,
                    anoData.ANO_Link_Id, 
                    anoData.ANO_Prefab, 
                    anoData.HaveAnoIdle,
                    anoData.MentalityDamage));
            }
            return anoCloneData;
        }
    }
}


