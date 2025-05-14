using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeckMan_RotateLock : MonoBehaviour
{

    private Vector3 moveDirection;  // NPC의 초기 이동 방향

    void Start()
    {
        // NPC의 초기 이동 방향 저장 (게임 시작 시)
        moveDirection = transform.forward;
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 90, 0);  // y축을 90도로 고정
        
        transform.position += moveDirection * Time.deltaTime;
    }
}