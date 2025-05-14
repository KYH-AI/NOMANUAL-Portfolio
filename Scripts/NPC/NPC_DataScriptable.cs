using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoManual.NPC
{
    [CreateAssetMenu(fileName = "[?] NPC Name",menuName = "NPC/NPC_DataScriptable")]
    public class NPC_DataScriptable : ScriptableObject
    {
        [Header("NPC KEY")] public NPC NpcKey;
        [Header("NPC 이미지")] public Sprite NPCSprite;
        [Header("NPC 이름")] public string NPCTitle;  // 로컬라이징 열거형 필요
        [Header("NPC 스토리")] // 로컬라이징 열거형 필요
        [Multiline] public string NPCDescription;
        [Header("NPC 가상 여부")] public bool IsNpcVirtual;
        [Header("NPC 생성될 프리팹")] public GameObject NPCPrefab;
        [Header("NPC 스토리 대사 데이터 리스트")] public List<NpcSubTitlesData> NpcSubtitlesTextKeys;
        [Header("NPC 평상 시 대사 데이터 리스트")] public List<NpcIdleSubTitlesData> NpcIdleSubtitlesTextKeys;

        /// <summary>
        /// 대사 및 음성
        /// </summary>
        [Serializable]
        public class NpcSubTitlesData
        {
            [Header("대사 텍스트")] public LocalizationTable.NPCTableTextKey NpcSubtitlesTextKey;
            [Header("대사 싱크 오프셋")] public float subtitlesOffset;
        }
        
        /// <summary>
        /// 평상 시 대사
        /// </summary>
        [Serializable]
        public sealed class NpcIdleSubTitlesData : NpcSubTitlesData
        {
            [Header("Idle 대사 Id")] public int idleSubtitleId;
        }
    }
}
