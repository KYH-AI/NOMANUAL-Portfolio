using System.Collections;
using System.Collections.Generic;
using NoManual.Creature;
using UnityEngine;

[CreateAssetMenu(fileName = "CreatureAnimatorStateSO",menuName = "Creature/CreatureSO")]
public class CreatureSO : ScriptableObject
{
    [Header("크리처 ID")] public int CreatureId;
    [Header("크리처 프리팹")] public GameObject CreaturePrefab;

    [Space(10)] [Header("======== 시야 ========")]
   
    [Header("일반 시야각")] public float DefaultSearchAngleRange;
    [Header("추적 시야각")] public float ChaseSearchAngleRange;
    [Header("일반 감지거리")] public float DefaultSearchDistanceRange;
    [Header("추적 감지거리")] public float ChaseSearchDistanceRange;

    [Space(10)] [Header("======== 공격 ========")]

    [Header("일반 공격거리")]  public float DefaultAttackRange;
    [Header("즉사 공격거리")]  public float KillAttackRange;
    [Header("일반 공격력")] public int DefaultAttackValue;
    [Header("공격 간격")] public float attackDelay;

    [Space(10)] [Header("======== 이동 ========")]
    
    [Header("순찰 이동속도")]  public float PatrolSpeed;
    [Header("추적 이동속도")]  public float ChaseSpeed;

    [Space(10)] [Header("======== 회전 ========")] 

    [Header("회전속도 (기본 = 8)")] public float RotationSpeed = 8f;
    
    [Space(10)] [Header("======== 문 ========")] 

    [Header("문 개방시간 (기본 = 5)")] public float DoorOpenDelay = 5f;

    [Space(10)] [Header("======== 애니메이터 SO ========")]
    
    [Header("크리처 애니메이터 SO")] public CreatureAnimatorStateSO CreatureAnimatorStateSo;
}
