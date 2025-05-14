using System.Collections;
using NoManual.Managers;
using UnityEngine;


public class EndingA_EndStep : ScenarioReceiverBase
{
    // 1. SFX0 재생
    // 2. 3초 뒤, SFX1 재생, 카메라 시체한테 고정
    // 3. 정신력 20씩 감소 
    // 4. 밧줄 뚜두두둑 (SFX2) 소리 들림
    // 5. 시체 밧줄 Destroy, 시체한테 Rigidbody 적용
    // 6. 0.5초 뒤, 화면 암전
    // 7. Monologue 씬 진입

    [SerializeField] private bool debugMode;
    
    [SerializeField] private AudioSource endStepAudioSource0;
    [SerializeField] private AudioSource endStepAudioSource1;
    [SerializeField] private AudioSource ropeAudioSource;
    [SerializeField] private AudioSource sceneAudioSource;
    [SerializeField] private GameObject rooseveltBody;
    [SerializeField] private GameObject rooseveltRope;
    [SerializeField] private AudioClip ropeCut;
    
    protected override void ScenarioLogic(ScenarioTriggerSender sender)
    {
        endStepAudioSource0.Play();
        StartCoroutine(LookAtRoosevelt()); 
    }

    private IEnumerator LookAtRoosevelt()
    {
        endStepAudioSource1.Play();
        yield return new WaitForSeconds(2f);
        PlayerAPI.SetFocusCameraTarget(rooseveltBody.transform);
        StartCoroutine(RopeCut());

    }

    private IEnumerator RopeCut()
    {
        ropeAudioSource.Play();

        yield return new WaitForSeconds(3f);
        
        sceneAudioSource.Stop();
        ropeAudioSource.Stop();
        ropeAudioSource.clip = ropeCut;
        ropeAudioSource.Play();
        
        Destroy(rooseveltRope);
        Rigidbody rigid = rooseveltBody.AddComponent<Rigidbody>();
        rigid.useGravity = true;

        yield return new WaitForSeconds(1f);
        
        // 씬을 Monologue 로 넘김
        if(!debugMode) GameManager.Instance.SaveGameManager.CurrentPlayerSaveData.Day++;
        Monologue.EndingCredit = true;
        NoManualHotelManager.Instance.SceneMove(false, false, GameManager.SceneName.Monologue, 0f);
    }

}
