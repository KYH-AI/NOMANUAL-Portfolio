using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachBall_DropCheck : MonoBehaviour
{
    [SerializeField] private AudioSource dropSfx;   // �浹 �� ����� ȿ����

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))  // �ٴڰ� �浹�ϸ�
        {
            if (dropSfx != null)
            {
                dropSfx.Play();  // ȿ���� ���
                Debug.Log("BeachBall�� �ٴڰ� �浹: ȿ���� �����");
            }
        }
    }
}