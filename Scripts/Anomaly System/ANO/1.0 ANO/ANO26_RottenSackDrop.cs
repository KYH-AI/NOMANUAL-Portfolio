using UnityEngine;
using NoManual.ANO;

public class ANO26_RottenSackDrop : ANO_Component
{
    [Header("ANO ����")] 
    [SerializeField] private Collider[] anoStarts;
    [SerializeField] private GameObject[] anoObjs;
    [SerializeField] private AudioSource anoSfx;
    
    [SerializeField] private float dropForce = 10f;
    private const float dropProbability = 0.35f;   

    private int triggerCount = 0; 
    private bool hasDropped = false;

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        // Sack�� �̹� �������ٸ� �� �̻� ������ �������� ����
        if (hasDropped)
        {
            return;
        }

        // anoStarts �迭�� ��ȸ�ϸ鼭 anoTriggerZone�� �浹 ���θ� üũ
        for (int i = 0; i < anoStarts.Length; i++)
        {
            if (anoTriggerZone == anoStarts[i])
            {
                triggerCount++;  // anoStart���� �浹 Ƚ���� ����

                // �浹 Ƚ���� 4��°�� ��� ������ ������
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

    // Sack�� ����߸��� �Լ�
    private void DropSack(int index)
    {
        anoSfx.Play();
        // anoObjs[index]�� Ȱ��ȭ
        anoObjs[index].SetActive(true);
        
        Rigidbody objRigidbody = anoObjs[index].AddComponent<Rigidbody>();

        // �Ʒ� �������� ���� ����
        objRigidbody.AddForce(Vector3.down * dropForce, ForceMode.Impulse);

        // Sack�� ���������Ƿ�, �� �̻� ������ ������� �ʵ��� ����
        hasDropped = true;
    }
}
