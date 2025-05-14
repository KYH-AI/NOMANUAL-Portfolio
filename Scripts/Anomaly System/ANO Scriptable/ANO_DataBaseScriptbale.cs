using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoManual.ANO
{
    [CreateAssetMenu(fileName = "ANO DataBase",menuName = "ANO/ANO_DataBase_Legacy")]
    public class ANO_DataBaseScriptbale : ScriptableObject
    {
        public List<ANO_DataScriptable_Legacy> anoDataBase = new List<ANO_DataScriptable_Legacy>();

        /// <summary>
        /// ANO id 기준으로 ANO 데이터 얻기
        /// </summary>
        public ANO_DataScriptable_Legacy GetANO_DataToId(int anoId)
        {
            foreach (var ano in anoDataBase)
            {
                if (ano.ANOId == anoId) return ano;
            }
            return null;
        }
    
    }
}


