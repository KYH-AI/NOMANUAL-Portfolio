using System.Collections;
using UnityEngine;
using NoManual.ANO;

public class ANO40_RollingTowardsToMe : ANO_Component
{
    [Header("ANO ¼³Á¤")]
    [SerializeField] private Collider[] anoStarts; 
    [SerializeField] private GameObject[] anoObjs;
    [SerializeField] private AudioSource[] anoSfxs;

    [SerializeField] private float forceAmount;

    private void Awake()
    {
        foreach (var ball in anoObjs) ball.SetActive(false);
    }
    
    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStarts[0])
        {
            Rigidbody rb = anoObjs[0].AddComponent<Rigidbody>();
            if (rb != null)
            {
                anoObjs[0].SetActive(true);
                rb.AddForce(Vector3.forward * forceAmount, ForceMode.Impulse);
                anoStarts[1].enabled = false;
            }
        }
        
        else if (anoTriggerZone == anoStarts[1])
        {
            Rigidbody rb = anoObjs[1].AddComponent<Rigidbody>();
            if (rb != null)
            {
                anoObjs[1].SetActive(true);
                rb.AddForce(Vector3.back * forceAmount, ForceMode.Impulse);
                anoStarts[0].enabled = false;
            }
        }
    }
}