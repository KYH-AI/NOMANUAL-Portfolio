using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class BT_CreatureVoiceAudio : Action
{
    [SerializeField] private SharedCreatureAudio sharedCreatureAudio;
    [SerializeField] private SfxEnum voiceSFX;
    [SerializeField] private bool isStop = false;
    [Header("크리처 추적 전용 오디오 세트")] [SerializeField] private bool isChaseAudioSet = true;
    [Header("크리처 보이스 오디오 One Shot")] [SerializeField] private bool isVoiceAudioLoop = true;

    public override void OnStart()
    {
        if (isChaseAudioSet)
        {
            if(isStop) sharedCreatureAudio.Value.StopCreatureChaseAudioSet();
            else sharedCreatureAudio.Value.PlayCreatureChaseAudioSet(voiceSFX);
        }
        else
        {
            if(isStop) sharedCreatureAudio.Value.StopCreatureVoiceAudio();
            else sharedCreatureAudio.Value.PlayCreatureVoiceAudio(voiceSFX, isVoiceAudioLoop);
        }
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}
