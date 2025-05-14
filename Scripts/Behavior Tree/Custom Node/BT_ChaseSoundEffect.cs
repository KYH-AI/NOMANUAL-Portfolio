using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class BT_ChaseSoundEffect : Action
{
    [SerializeField] private bool isChase = false;
    
    public override void OnStart()
    {
        if(isChase)  NoManual.Managers.NoManualHotelManager.Instance.AudioManager.PlayChaseAudio();
        else NoManual.Managers.NoManualHotelManager.Instance.AudioManager.StopChaseAudio();
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}
