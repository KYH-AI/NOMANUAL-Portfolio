using System.Collections;
using System.Collections.Generic;
using HFPS.Player;
using NoManual.ANO;
using UnityEngine;

public class ANO44_ManequinnChase : ANO_Component
{
    [Header("ANO ����")]
    [SerializeField] private Collider anoStart; // ANO ���� ����
    [SerializeField] private GameObject anoObj; // ������ ����ŷ ������
    [SerializeField] private float spawnOffset = 2f; // �÷��̾� �ڿ� ����ŷ�� ������ ������ �Ÿ�
    [SerializeField] private Collider anoEnd;
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private AudioSource anoSfx0;
    [SerializeField] private AudioSource anoSfx1;

    private Transform manequinnLookPoint;
    private GameObject currentManequin; // ���� ������ ����ŷ
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main; // ���� ī�޶� ����
    }

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            // 1. player ������ gameobject anoSpawnPos ����, �÷��̾� ���� �ڿ��� ��
            Vector3 spawnPosition = PlayerController.Instance.transform.position - PlayerController.Instance.transform.forward * spawnOffset;

            // �÷��̾ �ٶ󺸴� �������� ȸ���ϰ� x���� -90���� ����
            Quaternion rotation = Quaternion.LookRotation(PlayerController.Instance.transform.forward);
            rotation *= Quaternion.Euler(-90f, 0f, 0f); // x�� -90�� ȸ��

            // ����ŷ ����
            currentManequin = Instantiate(anoObj, spawnPosition, rotation);
            // ����ŷ LookPoint
            manequinnLookPoint = currentManequin.transform.GetChild(0);
            
            Debug.Log("����ŷ�� �����Ǿ����ϴ�: " + currentManequin.name); // ����ŷ ���� �α�

            // 2. ������ anoObj�� �ٶ� ��� (�̴� Raycast), ���ŷ� ������
            StartCoroutine(CheckIfPlayerLooksAtManequin());

            anoStart.enabled = false;
        }
        
        else if (anoTriggerZone == anoEnd)
        {
            anoStart.enabled = true;
            StopAllCoroutines();
            if (currentManequin)
            {
                manequinnLookPoint = null;
                Destroy(currentManequin);
            }
        }
    }

    private IEnumerator CheckIfPlayerLooksAtManequin()
    {
        while (true)
        {
            Vector3 viewportPosition = mainCamera.WorldToViewportPoint(currentManequin.transform.position);
            if (viewportPosition.z < 0f || viewportPosition.x < 0f || viewportPosition.x > 1f || viewportPosition.y < 0f || viewportPosition.y > 1f)
            {
                Vector3 spawnPosition = PlayerController.Instance.transform.position - PlayerController.Instance.transform.forward * spawnOffset;
               
                // ȿ���� ���
                // anoSfx0.Play();
                
                // �÷��̾ �ٶ󺸴� �������� ȸ���ϰ� x���� -90���� ����
                Quaternion rotation = Quaternion.LookRotation(PlayerController.Instance.transform.forward);
                rotation *= Quaternion.Euler(-90f, 0f, 0f); // x�� -90�� ȸ��
                // ���ο� ����ŷ ���� �� ȸ��
                currentManequin.transform.position = spawnPosition;
                currentManequin.transform.rotation = rotation;
            }
            else
            {
                Vector3 camPosition = mainCamera.transform.position;

                // Raycast�� ���� �÷��̾ ����ŷ�� ���� �ִ��� Ȯ��
                if (Physics.Linecast(camPosition, manequinnLookPoint.position, out RaycastHit hit, layerMask, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.gameObject == currentManequin)
                    {
                        // ���ŷ� ������ ����
                        PlayerController.Instance.DecreaseMentality(5);
                    }
                }
            }
            
            yield return new WaitForSeconds(3f); // üũ �ֱ�
        }
    }
}
