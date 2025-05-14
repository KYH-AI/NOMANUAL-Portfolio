using HFPS.Player;
using UnityEngine;
using NoManual.ANO;

public class ANO22_HotelJazzRewind : ANO_Component
{
    [Header("ANO ¼³Á¤")] 
    [SerializeField] private Collider anoStart;
    [SerializeField] private AudioSource anoSfx0;
    [SerializeField] private AudioSource anoSfx1;

    private void Start()
    {
        anoSfx0.Play();
    }

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            anoSfx1.Play();
            PlayerController.Instance.DecreaseMentality(5);
        }
    }
}
