using System.Collections;
using NoManual.Managers;
using RayFire;
using UnityEngine;

public class EndingB_Step1 : ScenarioReceiverBase
{
    [SerializeField] private RayfireRigid[] _rayfireRigids;
    [SerializeField] private AudioSource sfx1;

    protected override void ScenarioLogic(ScenarioTriggerSender sender)
    {
        NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(5, 0.1f);

        foreach (var rigid in _rayfireRigids)
        {
            rigid.Initialize();
        }
        sfx1.Play();
    }
    
}