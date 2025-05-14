using System.Collections.Generic;
using UnityEngine;
using NoManual.Managers;

namespace NoManual.NPC
{
    public class NpcManager : MonoBehaviour
    {
        [SerializeField] private NPC_DataBaseScriptable npcDataBaseScriptable;

        // NPC 생성 오브젝트 루트
        [SerializeField] private Transform _npcSpawnRoot;
        // 실존 NPC 정보
        private List<NPC_Reality> _npcRealities = new List<NPC_Reality>();
        // 가상 NPC 정보
        private List<NPC_Virtual> _npcVirtual = new List<NPC_Virtual>();
        

        /// <summary>
        /// NPC 프리팹 생성
        /// </summary>
        public void CreateNpcPrefab(NPC npc, Transform root = null)
        {
            Transform npcSpawnRoot = !root ? _npcSpawnRoot : root;
            
            NPC_DataScriptable npcOriginalData = npcDataBaseScriptable.GetNpcData(npc);
            NPC_Component npcComponent = Instantiate(npcOriginalData.NPCPrefab, npcSpawnRoot).GetComponent<NPC_Component>();

            if (npcComponent == null)
            {
                // TODO : NPC 프리팹 생성 실패 오류
                return;
            }

            npcComponent.InitializeNPC(new NPC_CloneData(npcOriginalData));
            AddNpcComponentData(npcComponent);
        }

        /// <summary>
        /// NPC 프리팹 제거
        /// </summary>
        public void DeleteNpcPrefab(NPC_Component npcComponent)
        {
            
        }

        /// <summary>
        /// NPC 데이터 저장
        /// </summary>
        private void AddNpcComponentData(NPC_Component npcComponent)
        {
            if (npcComponent is NPC_Virtual npcVirtual)
            {
                _npcVirtual.Add(npcVirtual);
            }
            else if (npcComponent is NPC_Reality npcReality)
            {
                _npcRealities.Add(npcReality);
            }
            else
            {
                // TODO : NPC 프리팹 정보 저장 실패 오류
            }
        }

        /// <summary>
        /// NPC 데이터 삭제
        /// </summary>
        private void RemoveNpcComponentData(NPC_Component npcComponent)
        {
            
        }

        /// <summary>
        /// 배치된 NPC 대사 진행
        /// </summary>
        public void PlayNpcTalk(NPC_Component npcComponent, LocalizationTable.NPCTableTextKey textKey, bool isStoryTalk = false)
        {
            NPC_DataScriptable.NpcSubTitlesData subTitleData;
            
            // 스토리 대사
            if (isStoryTalk)
            {
                subTitleData = GetNpcStorySubtitleKey(npcComponent, textKey);
            }
            // 평상 시 대사
            else
            {
                int idleSubTitleId = npcComponent.GetIdleTalkId();
                subTitleData = GetNpcIdleSubtitleKey(npcComponent, idleSubTitleId);
            }

            if (subTitleData == null)
            {
                // TODO : 오류 출력
                return;
            }
            
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowSubtitlesText(subTitleData);
        }
        
        /// <summary>
        /// 배치된 NPC 대사 진행
        /// </summary>
        public void PlayNpcTalk(NPC npcKey, LocalizationTable.NPCTableTextKey textKey, bool isVirtual, bool isStoryTalk)
        {
            NPC_Component npcComponent;
            
            // NPC_Component를 통해 가상 또는 실존 NPC를 가져옴
            if (isVirtual)
            {
                npcComponent = GetNpcComponent(npcKey, _npcRealities);
            }
            else
            {
                npcComponent = GetNpcComponent(npcKey, _npcVirtual);
            }

            if (!npcComponent)
            {
                // TODO : 에러 출력
                return;
            }
            PlayNpcTalk(npcComponent, textKey, isStoryTalk);
        }

        /// <summary>
        /// 배치된 NPC 컴포넌트 정보 가져오기
        /// </summary>
        private NPC_Component GetNpcComponent<T>(NPC npcKey, List<T> npcList) where T : NPC_Component
        {
            foreach (T npcComponent in npcList)
            {
                if (npcComponent.NpcKey == npcKey)
                {
                    return npcComponent;
                }
            }
            return null;
        }

        /// <summary>
        /// 배치된 NPC 컴포넌트 정보 가져오기
        /// </summary>
        public NPC_Component GetNpcComponent(NPC npc)
        {
            NPC_DataScriptable npcDataScriptable = npcDataBaseScriptable.GetNpcData(npc);
            NPC_Component npcComponent;
            
            // NPC_Component를 통해 가상 또는 실존 NPC를 가져옴
            if (!npcDataScriptable.IsNpcVirtual)
            {
                npcComponent = GetNpcComponent(npc, _npcRealities);
            }
            else
            {
                npcComponent = GetNpcComponent(npc, _npcVirtual);
            }
            
            return npcComponent;
        }

        #region NPC 대사 정보 얻기

        /// <summary>
        /// NPC 키값을 이용해 스토리 대사 정보 가져오기
        /// </summary>
        public NPC_DataScriptable.NpcSubTitlesData GetNpcStorySubtitleKey(NPC npcKey, LocalizationTable.NPCTableTextKey subtitleKey)
        {
            NPC_DataScriptable npcOriginalData = npcDataBaseScriptable.GetNpcData(npcKey);
            return GetNpcStorySubtitleKey(npcOriginalData.NpcSubtitlesTextKeys, subtitleKey);
        }
        
        public NPC_DataScriptable.NpcSubTitlesData GetNpcStorySubtitleKey(NPC npcKey, string subtitleKey)
        {
            NPC_DataScriptable npcOriginalData = npcDataBaseScriptable.GetNpcData(npcKey);
            return GetNpcStorySubtitleKey(npcOriginalData.NpcSubtitlesTextKeys, subtitleKey);
        }

        /// <summary>
        /// NPC 컴포넌트을 이용해 스토리 대사 정보 가져오기
        /// </summary>
        private NPC_DataScriptable.NpcSubTitlesData GetNpcStorySubtitleKey(NPC_Component npcComponent, LocalizationTable.NPCTableTextKey subtitleKey)
        {
            return GetNpcStorySubtitleKey(npcComponent.NpcCloneData.NpcSubtitlesTextKeys, subtitleKey);
        }
        
        /// <summary>
        /// NPC 스토리 대사 정보 가져오기
        /// </summary>
        private NPC_DataScriptable.NpcSubTitlesData GetNpcStorySubtitleKey(List<NPC_DataScriptable.NpcSubTitlesData> subTitleList,  LocalizationTable.NPCTableTextKey subtitleKey)
        {
            foreach (NPC_DataScriptable.NpcSubTitlesData subtitleData in subTitleList)
            {
                if (subtitleData.NpcSubtitlesTextKey == subtitleKey)
                {
                    return subtitleData;
                }
            }
            return null;
        }
        
        private NPC_DataScriptable.NpcSubTitlesData GetNpcStorySubtitleKey(List<NPC_DataScriptable.NpcSubTitlesData> subTitleList,  string subtitleKey)
        {
            foreach (NPC_DataScriptable.NpcSubTitlesData subtitleData in subTitleList)
            {
                if (subtitleData.NpcSubtitlesTextKey.ToString() == subtitleKey)
                {
                    return subtitleData;
                }
            }
            return null;
        }
        
        
        /// <summary>
        /// NPC 키값을 이용해 평상 시 대사 정보 가져오기
        /// </summary>
        private NPC_DataScriptable.NpcIdleSubTitlesData GetNpcIdleSubtitleKey(NPC npcKey, int idleSubtitleId)
        {
            NPC_DataScriptable npcOriginalData = npcDataBaseScriptable.GetNpcData(npcKey);
            return GetNpcIdleSubtitleKey(npcOriginalData.NpcIdleSubtitlesTextKeys, idleSubtitleId);
        }

        /// <summary>
        /// NPC 컴포넌트을 이용해 평상 시 대사 정보 가져오기
        /// </summary>
        private NPC_DataScriptable.NpcIdleSubTitlesData GetNpcIdleSubtitleKey(NPC_Component npcComponent, int idleSubtitleId)
        {
            return GetNpcIdleSubtitleKey(npcComponent.NpcCloneData.NpcIdleSubTitlesKeys, idleSubtitleId);
        }
        
        /// <summary>
        /// NPC 평상 시 대사 정보 가져오기
        /// </summary>
        private NPC_DataScriptable.NpcIdleSubTitlesData GetNpcIdleSubtitleKey(List<NPC_DataScriptable.NpcIdleSubTitlesData> subTitleList, int idleSubtitleId)
        {
            foreach (NPC_DataScriptable.NpcIdleSubTitlesData subtitleData in subTitleList)
            {
                if (subtitleData.idleSubtitleId == idleSubtitleId)
                {
                    return subtitleData;
                }
            }
            return null;
        }
        

        #endregion
        
    }

    /// <summary>
    /// NPC 데이터 복사(매핑)
    /// </summary>
    public sealed class NPC_CloneData
    {
        public NPC NpcKey;
        public Sprite NpcSprite;
        public string NpcTitle;
        public string NpcDescription;
        public bool IsNpcVirtual;
        public GameObject NpcPrefab;
        public List<NPC_DataScriptable.NpcSubTitlesData> NpcSubtitlesTextKeys;
        public List<NPC_DataScriptable.NpcIdleSubTitlesData> NpcIdleSubTitlesKeys;
 
        public NPC_CloneData(NPC_DataScriptable npcDataScriptable)
        {
            this.NpcKey = npcDataScriptable.NpcKey;
            this.NpcSprite = npcDataScriptable.NPCSprite;
            this.NpcTitle = npcDataScriptable.NPCTitle;
            this.NpcDescription = npcDataScriptable.NPCDescription;
            this.IsNpcVirtual = npcDataScriptable.IsNpcVirtual;
            this.NpcPrefab = npcDataScriptable.NPCPrefab;
            this.NpcSubtitlesTextKeys = npcDataScriptable.NpcSubtitlesTextKeys;
            this.NpcIdleSubTitlesKeys = npcDataScriptable.NpcIdleSubtitlesTextKeys;
        }
    }
}

