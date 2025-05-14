using UnityEngine;
using NoManual.ANO;

public class ANO_Drop_Effect : ANO_Component
{
    [Header("ANO 설정")]
    [SerializeField] private Collider anoStart; // 접촉할 Collider
    [SerializeField] private GameObject[] anoObjs; // 떨어질 오브젝트 배열
    [SerializeField] private AudioClip dropSfx; // 바닥에 닿을 때 재생할 소리

    /// <summary>
    /// 1. Drop할 Object를 전부 비활성화
    /// </summary>
    private void Start()
    {
        foreach (GameObject obj in anoObjs)
        {
            obj.SetActive(false);
        }
    }
    

    /// <summary>
    /// 2. 플레이어 충돌 체크
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
    /// DropObject 후, 각 Obj에 ANO_DropSound 부착 이후 dropCheck Initialize
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