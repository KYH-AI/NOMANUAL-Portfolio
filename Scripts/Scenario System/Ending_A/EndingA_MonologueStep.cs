using System.Collections;
using UnityEngine;

public class EndingA_MonologueStep : ScenarioReceiverBase
{
    [SerializeField] private TextScriptArray[] TextScripts;

    protected override void ScenarioLogic(ScenarioTriggerSender sender)
    {
        StartCoroutine(PlayMonologue(TextScripts));
    }

    protected override IEnumerator PlayMonologue(TextScriptArray[] textScripts)
    {
        yield return base.PlayMonologue(textScripts);
    }
}