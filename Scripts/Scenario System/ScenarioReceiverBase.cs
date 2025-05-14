using System;
using System.Collections;
using NoManual.Managers;
using UnityEngine;

public abstract class ScenarioReceiverBase : MonoBehaviour
{
    private Collider _collider;

    /// <summary>
    /// 충돌 이벤트 처리
    /// </summary>
    /// <param name="sender">ScenarioTriggerSender 객체</param>
    public void OnScenarioTrigger(ScenarioTriggerSender sender)
    {
        ScenarioLogic(sender); 
    }

    /// <summary>
    /// 각 Receiver가 구현해야 할 로직
    /// </summary>
    /// <param name="sender"></param>
    protected abstract void ScenarioLogic(ScenarioTriggerSender sender);
    
    /// <summary>
    /// 플레이어가 sender를 벗어났을 때 처리할 동작
    /// </summary>
    /// <param name="sender">ScenarioTriggerSender 객체</param>
    public virtual void OnScenarioExit(ScenarioTriggerSender sender)
    {
        // 기본 동작: 아무것도 하지 않음
    }

    public void Update()
    {
        
    }

    /// <summary>
    /// 텍스트와 음성을 재생하는 기본 동작
    /// </summary>
    /// <param name="textScripts"></param>
    protected virtual IEnumerator PlayMonologue(TextScriptArray[] textScripts)
    {
        foreach (var script in textScripts)
        {
            // 텍스트와 음성 출력
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowSubtitlesText(script);

            // 음성 길이에 JumpOffset 추가 후 대기
            yield return new WaitForSeconds(script.TextScriptList[0].VoiceClip.length + script.JumpOffset);
        }
    }
}
