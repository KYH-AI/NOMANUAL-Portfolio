using System.ComponentModel;
using NoManual.ANO;
using UnityEngine;

public class ANO_Collider_Object_Module : ANO_BaseModule
{
    
    [Header("ANO 오브젝트, 반드시 필요")]
    [SerializeField] private GameObject[] anoObjs;

    [Header("ANO 오브젝트 제거 시 닿을 콜라이더 (옵션)")]
    [SerializeField] private Collider anoEnd;

    [Header("모듈화 옵션")]
    [Description("오브젝트가 Rigidbody를 가지는가?")]
    [SerializeField] private bool isRigid;

    [Description("오브젝트가 땅과 충돌할 때 나는 소리")] 
    [SerializeField] private AudioClip dropSound;
    
    [Description("오브젝트를 앞 방향으로 밀 것인가")]
    [SerializeField] private bool isPushed;
    [SerializeField] private float pushPower;
    
    [Description("오브젝트가 고유 효과음을 내는가")]
    [SerializeField] private bool isObjSfx;
    
    [Header("ANO End 충돌 시 오브젝트 삭제")]
    [SerializeField] private bool isAnoEnd_AfterObjDestroy; // anoEnd 충돌 시 오브젝트 삭제 여부

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
