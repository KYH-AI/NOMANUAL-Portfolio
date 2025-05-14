using System.Collections;
using System.Collections.Generic;
using NoManual.Managers;
using UnityEngine;

namespace NoManual.NPC
{
    /// <summary>
    /// NPC 컴포넌트
    /// </summary>
    public class NPC_Component : MonoBehaviour
    {
        [Header("NPC Key(Id)")]
        public NPC NpcKey;
        public NPC_CloneData NpcCloneData { get; private set; }

        /// <summary>
        /// NPC 데이터 초기화
        /// </summary>
        public virtual void InitializeNPC(NPC_CloneData npcData)
        {
            this.NpcCloneData = npcData;
        }

        /// <summary>
        /// 평상 시 대화 Id 값 전달
        /// </summary>
        public int GetIdleTalkId()
        {
            return Random.Range(0, NpcCloneData.NpcIdleSubTitlesKeys.Count);
        }
    }
}


