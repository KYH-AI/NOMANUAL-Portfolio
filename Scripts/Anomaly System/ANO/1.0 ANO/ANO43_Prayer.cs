using System.Collections;
using HFPS.Player;
using NoManual.ANO;
using NoManual.StateMachine;
using UnityEngine;

public class ANO43_Prayer : ANO_Component
{
    [Header("ANO 설정")]
    [SerializeField] private GameObject prayer;
    [SerializeField] private Collider anoStart;
    [SerializeField] private Collider[] anoEnds;
    [SerializeField] private AudioSource anoSfx1;
    [SerializeField] private AudioSource anoBGM;

    private Coroutine damageCoroutine;
    private bool isInAno = false;

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            isInAno = true;
            anoSfx1.Play();
            anoBGM.Play();
            damageCoroutine = StartCoroutine(DamageOverTime());
        }
        
        if (anoTriggerZone == anoEnds[0] || anoTriggerZone == anoEnds[1])
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
          //  PlayerAPI.ResetFocusCameraTarget();
            isInAno = false;
            
            anoSfx1.Stop();
            anoBGM.Stop();
        }
    }

    private IEnumerator DamageOverTime()
    {
        while (isInAno)
        {
           // PlayerAPI.SetFocusCameraTarget(prayer.transform);
            PlayerController.Instance.DecreaseMentality(2);
            if (isInAno == false)
            {
                StopCoroutine(DamageOverTime());
            }
            yield return new WaitForSeconds(2f);
        }
    }
}