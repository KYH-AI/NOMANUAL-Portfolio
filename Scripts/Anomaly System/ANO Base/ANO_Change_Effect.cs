using UnityEngine;
using NoManual.ANO;

public class ANO_Change_Effect : ANO_Component
{
    [Header("ANO ����")]
    [SerializeField] private Collider anoStart; // ������ Collider
    [SerializeField] private GameObject beforeObj; // �ʱ� ������Ʈ
    [SerializeField] private GameObject afterObj; // ��ȯ�� ������Ʈ
    [SerializeField] private AudioSource anoSfx; // ����� �Ҹ�

    
    /// <summary>
    /// anoStart�� �����ϸ�, �ݶ��̴� �Ǵ� �� beforeObj�� afterObj�� ����
    /// anoStart�� ������, �����Ǵ� ��� beforeObj�� afterObj�� ����
    /// anoSfx�� �����ϸ�, anoSfx ��� -> Init �����ϰ� �ڵ�
    /// </summary>

    private void Start()
    {
        anoSfx.playOnAwake = false;
        anoSfx.spatialBlend = 1.0f;
    }

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoStart != null)
        {
            if (anoTriggerZone == anoStart)
            {
                ChangeObject();
                anoStart.enabled = false;
            }
        }
    }


    private void ChangeObject()
    {
        beforeObj.SetActive(false);
        afterObj.SetActive(true); // afterObj�� Ȱ��ȭ

        if (anoSfx != null)
        {
            anoSfx.Play(); // �Ҹ� ���
        }
    }
}