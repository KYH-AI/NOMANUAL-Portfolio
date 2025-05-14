using UnityEngine;
using NoManual.ANO;
using NoManual.Interaction;

public class ANO31_EatingMeatSound : ANO_Component
{
    [Header("ANO 설정")] 
    [SerializeField] private Collider anoStart;
    [SerializeField] private Collider anoEnd;
    [SerializeField] private GameObject anoObj;
    [SerializeField] private AudioSource anoSfx;
    [SerializeField] private DoorComponent anoDoor;
    
    
    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            anoDoor.InteractionDoor(); // 문 열기
            anoStart.enabled = false;
        }

        if (anoTriggerZone == anoEnd)
        {
            Rigidbody objRigidbody = anoObj.AddComponent<Rigidbody>();
            if (objRigidbody != null)
            {
                objRigidbody.isKinematic = false;
            }
            anoSfx.Stop();
        }
    }
}
