using HFPS.Player;
using UnityEngine;
using NoManual.ANO;
using NoManual.Managers;


public class ANO33_BangingDoor : ANO_Component
{

    // ¿­·ÁÀÖ´ø ¹®ÀÌ ÇÑ²¨¹ø¿¡ ´ÝÈû. 

    [Header("ANO ¼³Á¤")]
    [SerializeField] private GameObject[] anoObj;
    [SerializeField] private Collider[] anoStart;

    [SerializeField] Animator[] anoAnim;
    [SerializeField] AudioSource[] anoSfx;
    private readonly int Trigger1033 = Animator.StringToHash("Trigger1033");
    
    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart[0])
        {
            anoAnim[0].SetTrigger(Trigger1033);
            anoSfx[0].Play();
            NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(3, 0.28f);
            PlayerController.Instance.DecreaseMentality(10);

            anoStart[0].enabled = false;
            anoObj[1].SetActive(false);
        }

        if (anoTriggerZone == anoStart[1])
        {
            anoAnim[1].SetTrigger(Trigger1033);
            anoSfx[1].Play();
            NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(3, 0.28f);
            PlayerController.Instance.DecreaseMentality(10);
            
            anoStart[1].enabled = false;
            anoObj[0].SetActive(false);
        }
    }
}