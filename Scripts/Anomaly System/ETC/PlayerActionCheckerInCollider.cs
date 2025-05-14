using System.Collections;
using System.Collections.Generic;
using HFPS.Player;
using NoManual.StateMachine;
using UnityEngine;

public class PlayerActionCheckerInCollider : MonoBehaviour
{
    private PlayerController _player;
    private bool _isPlayerInside = false;
    private bool _isHoldingBreath = false;
    private bool _isBlinking = false;

    void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        if (_player == null)
        {
            Debug.LogError("PlayerController를 찾을 수 없습니다!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInside = true;
            Debug.Log("플레이어가 콜라이더 안에 들어옴.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInside = false;
            _isHoldingBreath = false;
            _isBlinking = false;
            Debug.Log("플레이어가 콜라이더 밖으로 나감.");
        }
    }

    void Update()
    {
        if (_isPlayerInside)
        {
            CheckPlayerActions();
        }
    }

    private void CheckPlayerActions()
    {
        _isHoldingBreath = IsPlayerHoldingBreath();
        _isBlinking = IsPlayerBlinking();
    }

    private bool IsPlayerHoldingBreath()
    {
        return _player != null && _player.SpecialStateMachine.CurrentState is HoldBreathState { holdBreathStateEnum: HoldBreathState.HoldBreathStateEnum.Keep};
    }

    private bool IsPlayerBlinking()
    {
        return _player != null && _player.SpecialStateMachine.CurrentState is BlinkEyeState;
    }

    // 숨참기 상태를 외부에서 접근할 수 있도록 프로퍼티 제공
    public bool IsHoldingBreath => _isHoldingBreath;

    // 눈감기 상태를 외부에서 접근할 수 있도록 프로퍼티 제공
    public bool IsBlinking => _isBlinking;
}