using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingB_Step3 : ScenarioReceiverBase
{
    [SerializeField] private Animation doors;
    
    protected override void ScenarioLogic(ScenarioTriggerSender sender)
    {
        doors.Play();
    }
}
