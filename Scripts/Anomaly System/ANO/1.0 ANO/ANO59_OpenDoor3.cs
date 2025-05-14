using System.Collections;
using NoManual.ANO;
using UnityEngine;

// ���� Neckman�� ������ ������ ����. ��� �̵��� ��������. 
public class ANO59_OpenDoor3 : ANO_Component
{
    [SerializeField] private Collider anoStart;
    [SerializeField] private Animator anoDoor; // �� �ִϸ��̼�
    [SerializeField] private AudioSource doorOpenSfx;
    [SerializeField] private Collider doorCloseCollider;
    [SerializeField] private AudioSource doorCloseSfx;
    [SerializeField] private Collider teleportCollider; // �÷��̾ ���� �� �ݶ��̴�
    [SerializeField] private Transform anoPlaceStart; // �̵��� ��ġ

    [Space(30)]
    [SerializeField] private AudioSource anoPlaceSfx1; // ���� ȿ�� 1
    [SerializeField] private AudioSource anoPlaceSfx2; // ���� ȿ�� 2
    [SerializeField] private AudioSource anoPlaceSfx3; 
    [SerializeField] private Collider deadCollider; // ���� ���� �ݶ��̴�
    
    private Transform playerPosition;

    private void Start()
    { 
        playerPosition = HFPS.Player.PlayerController.Instance.transform; // �÷��̾� ��ġ ��������
    }

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            if (anoDoor != null)
            {
                anoDoor.SetTrigger("Open");
                doorOpenSfx.Play();
                anoStart.enabled = false;
            }
        }

        if (anoTriggerZone == doorCloseCollider)
        {
            anoDoor.SetTrigger("Close");
            doorCloseSfx.Play();
            doorCloseCollider.enabled = false;

        }
        
        if (anoTriggerZone == teleportCollider)
        {
            // �÷��̾ anoPlaceStart�� �̵�
            playerPosition.position = anoPlaceStart.position;
            playerPosition.rotation = anoPlaceStart.rotation;
            
            // �Ҹ� ���
            anoPlaceSfx1.Play();
            anoPlaceSfx2.Play();
            anoPlaceSfx3.Play();
        }

        if (anoTriggerZone == deadCollider)
        {
            // ������ ���� 100
            anoPlaceSfx1.Stop();
            anoPlaceSfx2.Stop();
            anoPlaceSfx3.Stop();
            HFPS.Player.PlayerController.Instance.DecreaseMentality(100);
        }
    }
}