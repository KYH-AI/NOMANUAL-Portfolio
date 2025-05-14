using UnityEngine;
using NoManual.Managers;
using System.Collections;


    /// <summary>
    /// 
    /// 1. receiver 에 플레이어 충돌 시
    ///   -> phoneScenario의 Layer가 Interact로 변경
    ///   -> phoneAudioSource에 phoneSound[0] 삽입 후 재생
    ///   -> blockArea 활성화 
    ///   
    /// 2. 플레이어가 phoneScenario와 Interact 했을 시
    ///   -> Roosevelt_Call_0 ~ 7 재생 (가능하면 phoneAudioSource에서)
    ///   (RootA_5 근무 완료, 근무 속성이 Interact이기 때문에)
    ///  
    /// 3. Roosevelt_Call_7 재생 종료 
    ///   -> phoneAudioSource에 phoneSound[1] 삽입 후 재생
    ///   -> blockArea 비활성화
    /// 
    /// </summary>


public class RootA_5 : ScenarioReceiverBase
{
    [Header("설정")]
    [SerializeField] private Collider blockColl; // 길막기 용 콜라이더
    [SerializeField] private AudioSource phoneAudioSource; // 오디오 재생 소스
    [SerializeField] private AudioClip[] phoneSound; // 전화 관련 오디오 (0: 벨소리, 1: 끝 소리)
    [SerializeField] private TextScriptArray[] textScripts; // 자막 스크립트 배열

    private int callCount = 0;
    private bool isInteracting = false;

        // RootA_5만의 고유 로직
    protected override void ScenarioLogic(ScenarioTriggerSender sender)
    {

    // 1. receiver에 플레이어가 닿으면, scenarioPhone의  
    // 3. Roosevelt_Call7의 재생이 종료될 시, endSound 재생 및 scenarioPhone의 Layer가 Default로 바뀜. 

        phoneAudioSource.clip = phoneSound[0];
        phoneAudioSource.Play();
    }

    public void StartRooseveltCall()
    { 
        phoneAudioSource.Stop();
        StartCoroutine(PlayRooseveltCalls()); 

    }

    private IEnumerator PlayRooseveltCalls()
    {
        blockColl.enabled = true;
        yield return new WaitForSeconds(3f);

        for (int i = 0; i < textScripts.Length; i++)
        {
            var script = textScripts[i];

            // 자막 출력 및 음성 재생
            NoManualHotelManager.Instance.UiNoManualUIManager.ShowSubtitlesText(script);

            // 현재 음성 재생이 끝날 때까지 대기
            if (script.TextScriptList.Length > 0 && script.TextScriptList[0].VoiceClip != null)
            {
                float waitTime = script.TextScriptList[0].VoiceClip.length + script.JumpOffset;
                yield return new WaitForSeconds(waitTime);
            }
        }
        // 이슈 : 음성 다 끝나고 나서 이걸 실행시켜야 함
        RooseveltCallEnd();
    }

    private void RooseveltCallEnd()
    {
        // 끝나는 소리 재생
        phoneAudioSource.clip = phoneSound[1];
        phoneAudioSource.Play();

        // 길막기 비활성화
        blockColl.enabled = false;

        isInteracting = false;
    }

}
