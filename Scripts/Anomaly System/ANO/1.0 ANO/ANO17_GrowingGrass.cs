using Magio;
using UnityEngine;
using NoManual.ANO;

public class ANO17_GrowingGrass : ANO_Component
{
    [SerializeField] private Collider anoStart;
    [SerializeField] private GameObject tree;
    [SerializeField] private GameObject grassArea;
    [SerializeField] private AudioSource anoSfx;

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            anoSfx.Play();
            anoStart.enabled = false;
            grassArea.SetActive(true);
            tree.SetActive(true);
        }
    }
}
