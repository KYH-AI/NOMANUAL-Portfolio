using System;
using System.Collections;
using System.Collections.Generic;
using NoManual.Task;
using UnityEngine;
using UnityEngine.Serialization;

namespace NoManual.ANO
{

    #region ANO Enum
    
    public enum ANOType
    {
        None = -1,
        PassiveANO = 0,
        ActiveANO = 1,
        NpcANO = 2,
        EndingANO = 3,
    }

    public enum ANOTier
    {
        None = -1,
        MA1 = 0,
        MA2 = 1,
        UA = 2,
    }

    // ANO 조치방법
    public enum ANOSolve
    {
        None = -1,
        SimpleSolve = 0,
    }

    public enum ANOEventRange
    {
        None = -1,
        Little = 0,
        Regular = 1,
        Extensive = 2,
    }

    public enum ANOVisvility
    {
        None = -1,
        EasyToSee = 0,
        NormalToSee = 1,
        HardToSee = 2,
    }

    public enum ANOManagement
    {
        None = -1,
        Simple = 0,
        Normal = 1,
        Hard = 2,
        Impossible = 3,
    }

    public enum ANORatingType
    {
        None = -1,
        SafeANO = 3,
        DangerANO = 8,
        VeryDangerousANO = 12,
    }
    
    #endregion
    
    [CreateAssetMenu(fileName = "[?] ANO Name",menuName = "ANO/ANO_DataScriptable_Legacy")]
    public class ANO_DataScriptable_Legacy : ScriptableObject
    {
        [Header("ANO ID")] public int ANOId;
        [Header("ANO Task Type")] public TaskHandler.TaskType TaskType;
        [Header("ANO 이미지")] public Sprite ANOSprite;
        [Header("ANO 이름")] public string ANOTitle;
        [Header("ANO 스토리")] 
        [Multiline] public string ANODescription;
        [Header("ANO 티어")] public ANOTier AnoTier;
        [Header("ANO 종류")] public ANOType AnoType;
        [Header("ANO 해결 방법")] public ANOSolve AnoSolve;
        [Header("ANO 생성될 프리팹")] public GameObject ANOPrefab;
        [Header("ANO Idle 프리팹 (생략 가능)")] public GameObject ANO_IdlePrefab;
        
        [Serializable]
        public sealed class ANORatingSettings
        {
            [Header("정신력 최대 데미지")] 
            [Range(0, 100)] public int mentalityDamage;
            [Header("조치 난이도")] public ANOManagement AnoManagement;
            [Header("스케일")] public ANOEventRange AnoEventRange;
            [Header("가시성")] public ANOVisvility AnoVisvility;
            
            [Serializable]
            public sealed class ANODeteriorationSettings
            {
                [Header("악화 유무")] public bool deterioration;
                [Header("악화 시 생성 유무")] public bool IsLink;
                [Header("악화 시 생성 될 ANO ID")] public int ANOLink;
                [Header("악화 타이머")]
                [Range(0, 100f)] public float deteriorationTimer;
            }

            [Header("ANO 악화 설정 (사용 X)")]
            public ANODeteriorationSettings AnoDeteriorationSettings = new ANODeteriorationSettings();
        }

        [Header("ANO 레이팅 설정")]
        public ANORatingSettings AnoRatingSettings = new ANORatingSettings();
    }
}


