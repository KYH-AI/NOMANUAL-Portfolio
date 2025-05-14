using HFPS.Player;
using UnityEngine;
using NoManual.ANO;
using NoManual.Managers;
using RayFire;


/// <summary>
/// 24.10.22 : RayFire 최적화 문제, 추후 적용
/// </summary>

public class ANO41_GLManDrop : ANO_Component
{
    
    [Header("ANO 설정")]
    [SerializeField] private GameObject[] anoObjs; // GLMan 오브젝트들
    [SerializeField] private Collider[] anoStarts; // anoStart 트리거들
    [SerializeField] private float setKinematicTime = 1f;
    [SerializeField] private AudioSource anoSfx;

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        for (int i = 0; i < anoStarts.Length; i++)
        {
            // 플레이어가 anoStarts[i]와 접촉하면 해당하는 anoObj[i]에 Rigidbody 부착 및 AddForce 적용
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