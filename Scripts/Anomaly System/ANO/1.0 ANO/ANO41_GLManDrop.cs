using HFPS.Player;
using UnityEngine;
using NoManual.ANO;
using NoManual.Managers;
using RayFire;


/// <summary>
/// 24.10.22 : RayFire ����ȭ ����, ���� ����
/// </summary>

public class ANO41_GLManDrop : ANO_Component
{
    
    [Header("ANO ����")]
    [SerializeField] private GameObject[] anoObjs; // GLMan ������Ʈ��
    [SerializeField] private Collider[] anoStarts; // anoStart Ʈ���ŵ�
    [SerializeField] private float setKinematicTime = 1f;
    [SerializeField] private AudioSource anoSfx;

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        for (int i = 0; i < anoStarts.Length; i++)
        {
            // �÷��̾ anoStarts[i]�� �����ϸ� �ش��ϴ� anoObj[i]�� Rigidbody ���� �� AddForce ����
            if (anoTriggerZone == anoStarts[i])
            {
                ActivateAnoObj(i);
            }
        }
    }

    private void ActivateAnoObj(int index)
    {
        foreach (var startCollider in anoStarts) startCollider.enabled = false;
        GameObject obj = anoObjs[index];
        obj.AddComponent<Rigidbody>();
        /*RayfireRigid rf = obj.GetComponent<RayfireRigid>();
        rf.simulationType = SimType.Dynamic;
        rf.demolitionType = DemolitionType.AwakePrefragment;
        rf.objectType = ObjectType.Mesh;
        rf.Initialize();*/

        anoObjs[index].SetActive(true);

        NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(3, 0.85f);
        PlayerController.Instance.DecreaseMentality(15);
      
    }
}