using System.Collections;
using System.Collections.Generic;
using NoManual.ANO;
using NoManual.Managers;
using UnityEngine;

public class ANO_Drop_Effect_Module : ANO_BaseModule
{
    [Header("Drop_Effect_Module 설정")]
    [SerializeField] private GameObject[] anoObjs; // 떨어질 오브젝트 배열
    [SerializeField] private AudioClip dropSfx; // 바닥에 닿을 때 재생할 소리
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
    /// DropObject 후, 각 Obj에 ANO_DropSound 부착 이후 dropCheck Initialize
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
