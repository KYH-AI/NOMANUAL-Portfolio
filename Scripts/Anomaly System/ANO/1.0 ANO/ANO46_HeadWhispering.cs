using System.Collections;
using System.Collections.Generic;
using NoManual.ANO;
using UnityEngine;

public class ANO46_HeadWhispering : ANO_Component
{
    [Header("ANO ����")] 
    [SerializeField] private Collider[] anoStart;
    [SerializeField] private GameObject[] anoObjCorridor1;
    [SerializeField] private GameObject[] anoObjCorridor2;
    [SerializeField] private GameObject[] anoObjCorridor3;
    [SerializeField] private GameObject[] anoObjCorridor4;

    [SerializeField] private AudioSource anoSfx;
    private bool isDropped = false;
    
    /*
     * ���� 1, ���� 2, ���� 3, ���� 4�� ������
     *
     * isDropped = false;
     * ���� 1 �ݶ��̴� ������, anoObjCorridor1[] ���̵� ���� Active, ������
     * ���� 2 �ݶ��̴� ������, anoObjCorridor2[] ���̵� ���� Active, ������
     * ���� 3 ��������. 
     * ���� 4 ��������.
     *
     * anoObjCorridorN[] Active, ������ ���� isDropped = true;
     * 
     * anoObj DropSound ������. 
     * Collider�� anoSfx�� ����. Active �� anoSfx Ȱ��ȭ
     * 
     * �÷��̾� raycast �߻�. anoObjCorridor1[], anoObjCorridor2[], anoObjCorridor3[] �߿� �ϳ��� ���� �� ���ŷ� ����
     */

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        base.ANO_TriggerCheck(anoTriggerZone);

        for (int i = 0; i < anoStart.Length; i++)
        {
            if (anoTriggerZone == anoStart[i])
            {
                // ������ anoStart �ݶ��̴��� ��� �ݵ�� �ߵ�
                if (i == anoStart.Length - 1 || Random.value <= 0.3f)
                {
                    ActivateAndDropObjects(i);
                    isDropped = true;
                }
            }
        }
    }

    private void ActivateAndDropObjects(int corridorIndex)
    {
        GameObject[] selectedCorridor = null;
        
        switch (corridorIndex)
        {
            case 0:
                selectedCorridor = anoObjCorridor1;
                break;
            case 1:
                selectedCorridor = anoObjCorridor2;
                break;
            case 2:
                selectedCorridor = anoObjCorridor3;
                break;
            case 3:
                selectedCorridor = anoObjCorridor4;
                break;
        }

        if (selectedCorridor != null)
        {
            // ��� ������Ʈ Ȱ��ȭ
            foreach (var obj in selectedCorridor)
            {
                obj.SetActive(true);
                // Rigidbody�� ������ isKinematic�� false�� �����Ͽ� �������� ��
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                }
            }

            // ����� Ȱ��ȭ
            if (anoSfx != null)
            {
                anoSfx.Play();
            }
        }
        
        // ������ ���� 
    }
}
