using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeckMan_RotateLock : MonoBehaviour
{

    private Vector3 moveDirection;  // NPC�� �ʱ� �̵� ����

    void Start()
    {
        // NPC�� �ʱ� �̵� ���� ���� (���� ���� ��)
        moveDirection = transform.forward;
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 90, 0);  // y���� 90���� ����
        
        transform.position += moveDirection * Time.deltaTime;
    }
}