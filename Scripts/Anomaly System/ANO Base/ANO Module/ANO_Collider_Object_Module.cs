using System.ComponentModel;
using NoManual.ANO;
using UnityEngine;

public class ANO_Collider_Object_Module : ANO_BaseModule
{
    
    [Header("ANO ������Ʈ, �ݵ�� �ʿ�")]
    [SerializeField] private GameObject[] anoObjs;

    [Header("ANO ������Ʈ ���� �� ���� �ݶ��̴� (�ɼ�)")]
    [SerializeField] private Collider anoEnd;

    [Header("���ȭ �ɼ�")]
    [Description("������Ʈ�� Rigidbody�� �����°�?")]
    [SerializeField] private bool isRigid;

    [Description("������Ʈ�� ���� �浹�� �� ���� �Ҹ�")] 
    [SerializeField] private AudioClip dropSound;
    
    [Description("������Ʈ�� �� �������� �� ���ΰ�")]
    [SerializeField] private bool isPushed;
    [SerializeField] private float pushPower;
    
    [Description("������Ʈ�� ���� ȿ������ ���°�")]
    [SerializeField] private bool isObjSfx;
    
    [Header("ANO End �浹 �� ������Ʈ ����")]
    [SerializeField] private bool isAnoEnd_AfterObjDestroy; // anoEnd �浹 �� ������Ʈ ���� ����

    private Rigidbody[] objRigidbodies;
    private AudioSource[] objSfxArray;


    public override void Init()
    {
        base.Init();
    }
    
    private void Start()
    {
        foreach (var obj in anoObjs)
        {
            obj.SetActive(false);
        }
        
        if (anoEnd != null)
        {
            anoEnd.isTrigger = true;
            anoEnd.enabled = false;
        }

        if (isPushed)
        {
            objRigidbodies = new Rigidbody[anoObjs.Length];
            for (int i = 0; i < anoObjs.Length; i++)
            {
                objRigidbodies[i] = anoObjs[i].AddComponent<Rigidbody>();
            }
        }

        if (isObjSfx)
        {
            objSfxArray = new AudioSource[anoObjs.Length];
            for (int i = 0; i < anoObjs.Length; i++)
            {
                objSfxArray[i] = anoObjs[i].AddComponent<AudioSource>();
                if (objSfxArray[i] != null)
                {
                    objSfxArray[i].playOnAwake = false;
                }
            }
        }
    }


    public override void Run(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStartCollider)
        {
            foreach (var obj in anoObjs)
            {
                obj.SetActive(true);
            }
            Stop(false);
            
            if (isRigid) ApplyRigid();
            if (isPushed) ApplyPushForce();
            if (isObjSfx) PlayObjectSfx();
        }

        if (anoEnd != null)
        {
            if (anoTriggerZone == anoEnd)
            {
                DestroyAllObjects();
            }
            anoEnd.enabled = false;
        }
    }
    
    private void ApplyRigid()
    {
        foreach (var obj in anoObjs)
        {
            ANO_DropSound dropCheck = obj.AddComponent<ANO_DropSound>();
            dropCheck.Initialize(dropSound);
        }
    }

    private void ApplyPushForce()
    {
        if (objRigidbodies != null)
        {
            foreach (var rb in objRigidbodies)
            {
                if (rb != null)
                {
                    rb.AddForce(Vector3.forward * pushPower, ForceMode.Impulse);
                }
            }
        }
    }
    
    private void PlayObjectSfx()
    {
        if (objSfxArray != null)
        {
            foreach (var sfx in objSfxArray)
            {
                if (sfx != null) sfx.Play();
            }
        }
    }
    
    private void DestroyAllObjects()
    {
        foreach (var obj in anoObjs)
        {
            Destroy(obj);
        }
    }
}
