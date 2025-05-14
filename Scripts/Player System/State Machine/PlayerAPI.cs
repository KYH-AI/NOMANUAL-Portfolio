using UnityEngine;
using HFPS.Player;
using NoManual.StateMachine;

/// <summary>
/// ANO 이벤트에서 사용할 플레이어 API
/// </summary>
public static class PlayerAPI
{
    private static PlayerController _player;
    private static MouseLook _mouseLook;
    
    /// <summary>
    /// API 초기화 (건들지 말기)
    /// </summary>
    public static void Initialize(PlayerController player)
    {
        _player = player;
        _mouseLook = HFPS.Systems.ScriptManager.Instance.C<MouseLook>();
    }

#region 카메라 제어

    /// <summary>
    /// 카메라 포커스 모드
    /// </summary>
    /// <param name="target">집중할 오브젝트</param>
    public static void SetFocusCameraTarget(Transform target)
    {
        _mouseLook.LockOnTarget = target;
    }

    /// <summary>
    /// 카메라 포커스 모드 해체
    /// </summary>
    public static void ResetFocusCameraTarget()
    {
        _mouseLook.LockOnTarget = null;
    }

    /// <summary>
    /// 카메라 앵글 제한
    /// </summary>
    /// <param name="minX">왼쪽 앵글</param>
    /// <param name="maxX">오른쪽 앵글</param>
    public static void SetCameraAngelXLimit(float minX, float maxX)
    {
        _mouseLook.SetLimitMode = true;
        _mouseLook.minimumX = minX;
        _mouseLook.maximumX = maxX;
    }

    /// <summary>
    /// 카메라 앵글 제한 해체
    /// </summary>
    public static void ResetCameraAngelXLimit()
    {
        _mouseLook.SetLimitMode = false;
        _mouseLook.minimumX = -360f;
        _mouseLook.maximumX = 360f;
    }

#endregion

#region 입력 제어

    /// <summary>
    /// 키보드 입력 모두 막기 (ESC 포함)
    /// </summary>
    public static void DisablePlayerKeyBoardInput()
    {
        ThunderWire.Input.InputHandler.InputActionLockControl(false, "Pause");
        ThunderWire.Input.InputHandler.InputActionLockControl(false, "Inventory");
        HFPS.Systems.HFPS_GameManager.Instance.LockPlayerKeyBoard(false);
    }

    /// <summary>
    /// 키보드 입력 막기 해체
    /// </summary>
    public static void EnablePlayerKeyBoardInput()
    {
        ThunderWire.Input.InputHandler.InputActionLockControl(true, "Pause");
        ThunderWire.Input.InputHandler.InputActionLockControl(true, "Inventory");
        HFPS.Systems.HFPS_GameManager.Instance.LockPlayerKeyBoard(true);
    }

    /// <summary>
    /// 마우스 입력 막기
    /// </summary>
    public static void DisablePlayerMouseInput()
    {
        HFPS.Systems.HFPS_GameManager.Instance.LockPlayerMouse(false);
    }
    
    /// <summary>
    /// 마우스 입력 막기 해체
    /// </summary>
    public static void EnablePlayerMouseInput()
    {
        HFPS.Systems.HFPS_GameManager.Instance.LockPlayerMouse(true);
    }

#endregion

#region 특별 상태 확인

    /// <summary>
    /// 눈 감기 확인
    /// </summary>
    public static bool GetBlinkEyeState()
    {
        return _player.SpecialStateMachine.CurrentState is BlinkEyeState;
    }

    /// <summary>
    /// 숨 참기 확인
    /// </summary>
    public static bool GetHoldBreathState()
    {
        return _player.SpecialStateMachine.CurrentState is HoldBreathState { holdBreathStateEnum: HoldBreathState.HoldBreathStateEnum.Keep };
    }
    
#endregion

#region 일어서기, 앉기 상태 확인

    /// <summary>
    /// 일어서기 확인
    /// </summary>
    public static bool GetStandState()
    {
        return _player.characterState == PlayerController.CharacterState.Stand;
    }

    /// <summary>
    /// 앉기 확인
    /// </summary>
    public static bool GetCrouchState()
    {
          return _player.characterState == PlayerController.CharacterState.Crouch;
    }

#endregion

#region 이동 상태 확인

    /// <summary>
    /// 대기 확인
    /// </summary>
    public static bool GetMovementIdleState()
    {
        return _player.CurrentPlayerMovementState == PlayerMovementState.Idle;
    }

    /// <summary>
    /// 걷기 확인
    /// </summary>
    public static bool GetMovementWalkState()
    {
        return _player.CurrentPlayerMovementState == PlayerMovementState.Walk;
    }

    /// <summary>
    /// 달리기 확인
    /// </summary>
    public static bool GetMovementRunState()
    {
        return _player.CurrentPlayerMovementState == PlayerMovementState.Run;
    }
     
    /// <summary>
    /// 앉아서 걷기 확인
    /// </summary>
    public static bool GetMovementCrouchWalkState()
    {
        return _player.CurrentPlayerMovementState == PlayerMovementState.CrouchWalk;
    }

#endregion
 
}
