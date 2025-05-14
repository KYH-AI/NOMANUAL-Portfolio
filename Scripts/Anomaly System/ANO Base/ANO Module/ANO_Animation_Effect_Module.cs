using System.Collections;
using System.Collections.Generic;
using NoManual.ANO;
using UnityEngine;

public class ANO_Animation_Effect_Module : ANO_BaseModule
{
    [Header("ANO �ִϸ��̼�")] [SerializeField] private List<Animator> anoAnims; // Animator ����Ʈ�� ����
    [Header("ANO End")] [SerializeField] private Collider anoEnd;
    [Header("ANO SFX")] [SerializeField] private AudioSource[] anoSfx; // AudioSource �迭�� ����

    [Header("ANO End �ɼ�")]
    [SerializeField] private bool isAnoEnd_AfterObjDestroy; // anoEnd �� ������Ʈ ���� ����
    [SerializeField] private bool isAnoEnd_AnimTrigger; // anoEnd �� Trigger2 ���� ����
    
    
    private readonly int trigger1 = Animator.StringToHash("Trigger1"); // Trigger1 �ִϸ��̼� Ʈ����
    private readonly int trigger2 = Animator.StringToHash("Trigger2"); // Trigger2 �ִϸ��̼� Ʈ����

    public override void Init()
    {
        base.Init();
        if (anoEnd != null)
        {
            anoEnd.enabled = false; // �ʱ⿣ ��Ȱ��ȭ
        }
    }

    public override void Run(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStartCollider)
        {
            ActivateAnoStart();
        }
        else if (anoTriggerZone == anoEnd)
        {
            ActivateAnoEnd();
        }
    }

    private void ActivateAnoStart()
    {
        AnimTrigger(trigger1); // Trigger1 �ִϸ��̼� ����
        PlayAllSfx(); // ��� SFX ���
        if (anoEnd != null)
        {
            anoEnd.enabled = true; // anoEnd Ȱ��ȭ
        }
        JumpScare();
        GiveDamage();
        Stop(true);
    }

    private void ActivateAnoEnd()
    {
        if (isAnoEnd_AnimTrigger)
        {
            AnimTrigger(trigger2); // Trigger2 �ִϸ��̼� ����
        }

        if (isAnoEnd_AfterObjDestroy)
        {
            DestroyObjects();
        }
        
        anoEnd.enabled = false; // anoEnd ��Ȱ��ȭ
    }

    private void AnimTrigger(int animId)
    {
        foreach (var anim in anoAnims)
        {
            anim.SetTrigger(animId);
        }
    }
    
    // ��� AudioSource ���
    private void PlayAllSfx()
    {
        foreach (var sfx in anoSfx)
        {
            sfx.Play();
        }
    }

    // ��� ������Ʈ ����
    private void DestroyObjects()
    {
        foreach (var obj in anoAnims)
        {
            Destroy(obj.gameObject);
        }
    }
}
