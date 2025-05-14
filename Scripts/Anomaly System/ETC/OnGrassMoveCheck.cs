using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGrassMoveCheck : MonoBehaviour
{
    [SerializeField] private Collider[] grassAreas;  // ���� �ܵ� ���� �ݶ��̴� �迭
    [SerializeField] private AudioClip[] grassSounds;  // ���� ����� Ŭ�� �迭
    [SerializeField] private float checkInterval = 0.5f;  // ����� ��� ����
    [SerializeField] private AudioSource audioSource;  // ����� �ҽ�
    [SerializeField] [Range(0.1f, 1.0f)] private float nextCheckTime;

    private Transform playerTransform;  // �÷��̾��� Ʈ������
    private Vector3 lastPlayerPosition;  // �÷��̾��� ������ ��ġ
    private float moveThreshold = 0.1f;  // �÷��̾ �̵��ߴٰ� ������ �ּ� �Ÿ�

    void Start()
    {
        playerTransform = HFPS.Player.PlayerController.Instance.transform;
        lastPlayerPosition = playerTransform.position;
    }

    void Update()
    {
        // ���� �ð� �������� �÷��̾��� ��ġ�� Ȯ��
        if (Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;

            // �÷��̾ ���� �ܵ� ���� �� �ϳ��� �ִ��� Ȯ��
            if (IsPlayerInAnyGrassArea())
            {
                // �÷��̾ ���������� üũ
                if (HasPlayerMoved())
                {
                    PlayRandomGrassSound();
                }
            }
        }
    }

    private bool IsPlayerInAnyGrassArea()
    {
        // ���� �ܵ� ���� �� �ϳ��� �÷��̾ �����ϰ� �ִ��� Ȯ��
        foreach (var grassArea in grassAreas)
        {
            if (grassArea.bounds.Contains(playerTransform.position))
            {
                return true;  // �÷��̾ �ܵ� ���� �� �ϳ��� ����
            }
        }
        return false;  // �÷��̾ �ܵ� ������ ����
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
