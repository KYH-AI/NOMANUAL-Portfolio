using System.Collections;
using NoManual.Managers;
using UnityEngine;


public class EndingA_EndStep : ScenarioReceiverBase
{
    // 1. SFX0 ���
    // 2. 3�� ��, SFX1 ���, ī�޶� ��ü���� ����
    // 3. ���ŷ� 20�� ���� 
    // 4. ���� �ѵεε� (SFX2) �Ҹ� �鸲
    // 5. ��ü ���� Destroy, ��ü���� Rigidbody ����
    // 6. 0.5�� ��, ȭ�� ����
    // 7. Monologue �� ����

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
        
        // ���� Monologue �� �ѱ�
        if(!debugMode) GameManager.Instance.SaveGameManager.CurrentPlayerSaveData.Day++;
        Monologue.EndingCredit = true;
        NoManualHotelManager.Instance.SceneMove(false, false, GameManager.SceneName.Monologue, 0f);
    }

}
