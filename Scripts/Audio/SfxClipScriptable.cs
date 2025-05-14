using UnityEngine;

public enum SfxEnum
{
    // 문
    DoorOpen,
    DoorClose,
    DoorLock,
    
    // 플레이어 
    ExhasutedBreath,

    // 크리처 추적
    StartBreath,
    ScaredBreath,
    HearthBreath,
    
    // Angel
    AngelIdle,
    AngelAngry,
    AngelChaseStart,
    AngelChase,
    
    //Cult Leader
    CultLeaderSpawn,
    CultLeaderChase,
    CultLeaderDestroy,
    
    // Elevator
    ElevatorBell,
    ElevatorDoorClose,
    ElevatorDoorOpen,
    ElevatorMove,
    ElevatorAnoWarning,
    
    PlayerDead,
}

[CreateAssetMenu(fileName = "AudioClipName",menuName = "AudioClip/SfxClipScriptable")]
public class SfxClipScriptable : AudioClipScriptable
{
    [Header("효과음 Key")]
    public SfxEnum sfxEnum;
}
