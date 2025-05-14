using System.Collections;
using System.Collections.Generic;
using HFPS.Player;
using NoManual.StateMachine;
using UnityEngine;

public class StateCameraEvent : MonoBehaviour
{
    /// <summary>
    /// 탈진 카메라 애니메이션 클립에서 호출
    /// </summary>
    public void EndCameraExhaustedEvent()
    {
        Exhausted exhaustedState = PlayerController.Instance.SpecialStateMachine.GetState(PlayerSpecialState.Exhausted) as Exhausted;
        exhaustedState?.ExitExhausted();
    }
}
