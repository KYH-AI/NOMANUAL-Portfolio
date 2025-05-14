using System;
using NoManual.ANO;
using NoManual.Managers;
using UnityEngine;

public class ANO53_AngelSpawn : ANO_Component
{
    [SerializeField] private CreatureSO angelSO;
    [SerializeField] private Transform angelSpawnPos;

    private void Start()
    {
        HotelManager.Instance.ANO.CreateCreature(angelSO, angelSpawnPos);
    }
    
}
