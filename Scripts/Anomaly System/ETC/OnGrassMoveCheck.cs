using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGrassMoveCheck : MonoBehaviour
{
    [SerializeField] private Collider[] grassAreas;  // 여러 잔디 영역 콜라이더 배열
    [SerializeField] private AudioClip[] grassSounds;  // 여러 오디오 클립 배열
    [SerializeField] private float checkInterval = 0.5f;  // 오디오 재생 간격
    [SerializeField] private AudioSource audioSource;  // 오디오 소스
    [SerializeField] [Range(0.1f, 1.0f)] private float nextCheckTime;

    private Transform playerTransform;  // 플레이어의 트랜스폼
    private Vector3 lastPlayerPosition;  // 플레이어의 마지막 위치
    private float moveThreshold = 0.1f;  // 플레이어가 이동했다고 간주할 최소 거리

    void Start()
    {
        playerTransform = HFPS.Player.PlayerController.Instance.transform;
        lastPlayerPosition = playerTransform.position;
    }

    void Update()
    {
        // 일정 시간 간격으로 플레이어의 위치를 확인
        if (Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;

            // 플레이어가 여러 잔디 영역 중 하나에 있는지 확인
            if (IsPlayerInAnyGrassArea())
            {
                // 플레이어가 움직였는지 체크
                if (HasPlayerMoved())
                {
                    PlayRandomGrassSound();
                }
            }
        }
    }

    private bool IsPlayerInAnyGrassArea()
    {
        // 여러 잔디 영역 중 하나라도 플레이어를 포함하고 있는지 확인
        foreach (var grassArea in grassAreas)
        {
            if (grassArea.bounds.Contains(playerTransform.position))
            {
                return true;  // 플레이어가 잔디 영역 중 하나에 있음
            }
        }
        return false;  // 플레이어가 잔디 영역에 없음
    }

    private bool HasPlayerMoved()
    {
        float distanceMoved = (playerTransform.position - lastPlayerPosition).sqrMagnitude;
        
        if (distanceMoved > moveThreshold * moveThreshold)
        {
            lastPlayerPosition = playerTransform.position;
            return true; 
        }
        return false;
    }

    private void PlayRandomGrassSound()
    {
        if (audioSource != null && grassSounds.Length > 0)
        {
            AudioClip randomClip = grassSounds[Random.Range(0, grassSounds.Length)];
            audioSource.PlayOneShot(randomClip);
        }
        else
        {
            Debug.LogWarning("AudioSource is not assigned or grassSounds array is empty.");
        }
    }
}
