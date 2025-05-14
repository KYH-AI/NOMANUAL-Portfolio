using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HFPS.Player;
using NoManual.ANO;

public class GLMan_DropCheck : ANO_Component
{
    [Header("Head Collider ����")]
    [SerializeField] private AudioSource anoSfx; // �浹 �� ����� ȿ����

    private bool hasCollided = false; // �ߺ� ��� ���� �÷���

    private void OnCollisionEnter(Collision collision)
    {
        // Floor�� �浹�� ��츸 ó��
        if (collision.gameObject.CompareTag("Floor") && !hasCollided)
        {
            hasCollided = true; // �ߺ� ��� ����

            // ȿ���� ���
            if (anoSfx != null)
            {
                anoSfx.Play();
            }

            // �÷��̾�� ������ ����
            PlayerController.Instance.DecreaseMentality(AnoCloneData.MentalityDamage);

            // Jumpscare ����
            // ���⿡ Jumpscare ���� �ڵ� �߰�
        }
    }
}