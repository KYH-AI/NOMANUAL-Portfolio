using UnityEngine;
using NoManual.ANO;

public class ANO_Drop_Effect : ANO_Component
{
    [Header("ANO ����")]
    [SerializeField] private Collider anoStart; // ������ Collider
    [SerializeField] private GameObject[] anoObjs; // ������ ������Ʈ �迭
    [SerializeField] private AudioClip dropSfx; // �ٴڿ� ���� �� ����� �Ҹ�

    /// <summary>
    /// 1. Drop�� Object�� ���� ��Ȱ��ȭ
    /// </summary>
    private void Start()
    {
        foreach (GameObject obj in anoObjs)
        {
            obj.SetActive(false);
        }
    }
    

    /// <summary>
    /// 2. �÷��̾� �浹 üũ
    /// </summary>
    /// <param name="anoTriggerZone"></param>
    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            DropObjects();
            anoStart.enabled = false;
        }
    }

    
    /// <summary>
    /// DropObject ��, �� Obj�� ANO_DropSound ���� ���� dropCheck Initialize
    /// </summary>
    private void DropObjects()
    {
        foreach (GameObject obj in anoObjs)
        {
            obj.SetActive(true);
            
            ANO_DropSound dropCheck = obj.AddComponent<ANO_DropSound>();
            dropCheck.Initialize(dropSfx);
        }
    }
}