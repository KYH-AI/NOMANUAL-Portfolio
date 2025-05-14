using UnityEngine;
using NoManual.ANO;

public class ANO26_RottenSackDrop : ANO_Component
{
    [Header("ANO 설정")] 
    [SerializeField] private Collider[] anoStarts;
    [SerializeField] private GameObject[] anoObjs;
    [SerializeField] private AudioSource anoSfx;
    
    [SerializeField] private float dropForce = 10f;
    private const float dropProbability = 0.35f;   

    private int triggerCount = 0; 
    private bool hasDropped = false;

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        // Sack이 이미 떨어졌다면 더 이상 로직을 실행하지 않음
        if (hasDropped)
        {
            return;
        }

        // anoStarts 배열을 순회하면서 anoTriggerZone과 충돌 여부를 체크
        for (int i = 0; i < anoStarts.Length; i++)
        {
            if (anoTriggerZone == anoStarts[i])
            {
                triggerCount++;  // anoStart와의 충돌 횟수를 증가

                // 충돌 횟수가 4번째일 경우 무조건 떨어짐
                if (triggerCount == 4)
                {
                    DropSack(i);
                }
                else
                {
                    if (Random.value <= dropProbability)
                    {
                        DropSack(i);
                    }
                }
            }
        }
    }

    // Sack을 떨어뜨리는 함수
    private void DropSack(int index)
    {
        anoSfx.Play();
        // anoObjs[index]를 활성화
        anoObjs[index].SetActive(true);
        
        Rigidbody objRigidbody = anoObjs[index].AddComponent<Rigidbody>();

        // 아래 방향으로 힘을 가함
        objRigidbody.AddForce(Vector3.down * dropForce, ForceMode.Impulse);

        // Sack이 떨어졌으므로, 더 이상 로직이 실행되지 않도록 설정
        hasDropped = true;
    }
}
