using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoManual.NPC
{
    public enum NPC
    {
        None = -1,
        Player = 0,
        Nurse = 1,
    }
    
    [CreateAssetMenu(fileName = "NPC DataBase",menuName = "NPC/NPC_DataBase")]
    public class NPC_DataBaseScriptable : ScriptableObject
    {
        public List<NPC_DataScriptable> npcDataBase = new List<NPC_DataScriptable>();

        /// <summary>
        /// NPC Key 기준으로 NPC 데이터 얻기
        /// </summary>
        public NPC_DataScriptable GetNpcData(NPC npc)
        {
            foreach (NPC_DataScriptable npcData in npcDataBase)
            {
                if (npcData.NpcKey == npc) return npcData;
            }
            return null;
        }
    }
}


