using NoManual.ANO;
using NoManual.Managers;
using UnityEngine;

public class ANO54_CultLeaderSpawn : ANO_Component
{
    [SerializeField] private CreatureSO cultLeaderSO;
    [SerializeField] private Transform cultLeaderSpawnPos;
    [SerializeField] private Collider anoStart;
    [SerializeField] private Collider anoEndElevator;
    [SerializeField] private Collider andEndSecurityOffice;

    private bool _spawn = false;
    
    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            HotelManager.Instance.ANO.CreateCreature(cultLeaderSO, cultLeaderSpawnPos);
            _spawn = true;
        }

        if (!_spawn) return;
        if (anoTriggerZone == anoEndElevator || anoTriggerZone == andEndSecurityOffice)
        {
            HotelManager.Instance.ANO.TargetResetCreature(cultLeaderSO.CreatureId);
            _spawn = false;
        }
    }
}

