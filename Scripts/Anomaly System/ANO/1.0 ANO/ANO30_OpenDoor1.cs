using UnityEngine;
using NoManual.ANO;

public class ANO30_OpenDoor1 : ANO_Component
{
    [Header("ANO 설정")]
    [SerializeField] private Collider anoStart;
    [SerializeField] private AudioSource anoSfx;
    [SerializeField] private Animator anoAnim;
    
    private static readonly int Trigger1030 = Animator.StringToHash("Open");

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            anoAnim.SetTrigger(Trigger1030);
            anoSfx.Play();
            anoStart.enabled = false;
        }
        
    }
}
