using System.Collections;
using System.Collections.Generic;
using NoManual.ANO;
using UnityEngine;

public class ANO46_HeadWhispering : ANO_Component
{
    [Header("ANO 설정")] 
    [SerializeField] private Collider[] anoStart;
    [SerializeField] private GameObject[] anoObjCorridor1;
    [SerializeField] private GameObject[] anoObjCorridor2;
    [SerializeField] private GameObject[] anoObjCorridor3;
    [SerializeField] private GameObject[] anoObjCorridor4;

    [SerializeField] private AudioSource anoSfx;
    private bool isDropped = false;
    
    /*
     * 복도 1, 복도 2, 복도 3, 복도 4로 나눠짐
     *
     * isDropped = false;
     * 복도 1 콜라이더 밟으면, anoObjCorridor1[] 아이들 전부 Active, 떨어짐
     * 복도 2 콜라이더 밟으면, anoObjCorridor2[] 아이들 전부 Active, 떨어짐
     * 복도 3 마찬가지. 
     * 복도 4 마찬가지.
     *
     * anoObjCorridorN[] Active, 떨어짐 이후 isDropped = true;
     * 
     * anoObj DropSound 장착됨. 
     * Collider는 anoSfx를 가짐. Active 시 anoSfx 활성화
     * 
     * 플레이어 raycast 발사. anoObjCorridor1[], anoObjCorridor2[], anoObjCorridor3[] 중에 하나라도 닿을 시 정신력 감소
     */

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        base.ANO_TriggerCheck(anoTriggerZone);

        for (int i = 0; i < anoStart.Length; i++)
        {
            if (anoTriggerZone == anoStart[i])
            {
                // 마지막 anoStart 콜라이더일 경우 반드시 발동
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
            // 모든 오브젝트 활성화
            foreach (var obj in selectedCorridor)
            {
                obj.SetActive(true);
                // Rigidbody가 있으면 isKinematic을 false로 설정하여 떨어지게 함
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                }
            }

            // 오디오 활성화
            if (anoSfx != null)
            {
                anoSfx.Play();
            }
        }
        
        // 데미지 로직 
    }
}
