using UnityEngine;
using NoManual.ANO;

public class ANO_Change_Effect : ANO_Component
{
    [Header("ANO 설정")]
    [SerializeField] private Collider anoStart; // 접촉할 Collider
    [SerializeField] private GameObject beforeObj; // 초기 오브젝트
    [SerializeField] private GameObject afterObj; // 변환할 오브젝트
    [SerializeField] private AudioSource anoSfx; // 재생할 소리

    
    /// <summary>
    /// anoStart가 존재하면, 콜라이더 판단 후 beforeObj가 afterObj로 변경
    /// anoStart가 없으면, 스폰되는 즉시 beforeObj가 afterObj로 변경
    /// anoSfx가 존재하면, anoSfx 재생 -> Init 가능하게 코드
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
        afterObj.SetActive(true); // afterObj를 활성화

        if (anoSfx != null)
        {
            anoSfx.Play(); // 소리 재생
        }
    }
}