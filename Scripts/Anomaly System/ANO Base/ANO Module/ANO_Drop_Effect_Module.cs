using System.Collections;
using System.Collections.Generic;
using NoManual.ANO;
using NoManual.Managers;
using UnityEngine;

public class ANO_Drop_Effect_Module : ANO_BaseModule
{
    [Header("Drop_Effect_Module ����")]
    [SerializeField] private GameObject[] anoObjs; // ������ ������Ʈ �迭
    [SerializeField] private AudioClip dropSfx; // �ٴڿ� ���� �� ����� �Ҹ�
    [SerializeField] private bool isPush;
    [SerializeField] private float pushPower;
    
    public override void Init()
    {
        base.Init();
        foreach (GameObject obj in anoObjs)
        {
            obj.SetActive(false);
        }
    }

    public override void Run(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStartCollider)
        {
            if(UseAudio) audioModule.audioSource.Play();
            if(UseJumpScare) NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(jumpScareModule.jumpScareLevel, jumpScareModule.jumpScareDelay);
            Stop(false);
            DropObjects();
        }
    }
    
    /// <summary>
    /// DropObject ��, �� Obj�� ANO_DropSound ���� ���� dropCheck Initialize
    /// </summary>
    private void DropObjects()
    {
        foreach (GameObject obj in anoObjs)
        {
            obj.SetActive(true);
            Rigidbody rb = obj.AddComponent<Rigidbody>();
            IsPush(rb);
            rb.isKinematic = false;
            
            rb.AddForce(Vector3.down * 10);
            
          ANO_DropSound dropCheck = obj.AddComponent<ANO_DropSound>();
         dropCheck.Initialize(dropSfx);
        }
     
    }

    private void IsPush(Rigidbody rb)
    {
        rb.AddForce(Vector3.forward * pushPower, ForceMode.Impulse);
    }
}
