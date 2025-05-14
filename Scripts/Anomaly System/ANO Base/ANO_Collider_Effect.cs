using System.ComponentModel;
using HFPS.Player;
using UnityEngine;
using NoManual.ANO;
using NoManual.Managers;

public class ANO_Collider_Object : ANO_Component
{
    [Header("ANO 오브젝트, 반드시 필요")]
    [SerializeField] private GameObject[] anoObjs;

    [Header("플레이어가 닿으면 ANO 활성화, 반드시 필요")]
    [SerializeField] private Collider anoStart;

    [Header("ANO 오브젝트 제거 시 닿을 콜라이더 (옵션)")]
    [SerializeField] private Collider anoEnd;
    [SerializeField] private AudioSource anoEndSfx;

    [Header("ano Start에 충돌 시 재생될 오디오")]
    [SerializeField] private AudioSource[] anoSfx;

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
    
    [Description("점프스케어를 가지는가")]
    [SerializeField] private bool isJumpscare;
    [SerializeField, Range(2, 4)] private int jumpscareLevel;
    [SerializeField, Range(0f, 5f)] private int jumpscareDelay;
    
    [Description("데미지가 있는가?")]
    [SerializeField] private bool isDamage;
    [SerializeField] private int damageValue = 0;

    [Header("ANO End 충돌 시 오브젝트 삭제")]
    [SerializeField] private bool isAnoEnd_AfterObjDestroy; // anoEnd 충돌 시 오브젝트 삭제 여부

    [Header("스티커 조치 시 효과음 삭제 여부 (아직 미구현)")] 
    [SerializeField] private bool isTurnOff;
    

    private Rigidbody[] objRigidbodies;
    private AudioSource[] objSfxArray;

    private void Start()
    {
        foreach (var obj in anoObjs)
        {
            obj.SetActive(false);
        }

        if (anoSfx != null)
        {
            foreach (var sfx in anoSfx)
            {
                SfxSettings(sfx);   
            }
        }
        
        anoStart.isTrigger = true;

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

    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        if (anoTriggerZone == anoStart)
        {
            ActivateObjects();
            anoStart.enabled = false;
        }
        else if (anoTriggerZone == anoEnd)
        {
            if (isAnoEnd_AfterObjDestroy)
            {
                DestroyAllObjects();
            }
            anoEnd.enabled = false;
        }
    }

    private void ActivateObjects()
    {
        foreach (var obj in anoObjs)
        {
            obj.SetActive(true);
        }

        if (anoEnd != null)
        {
            anoEnd.enabled = true;
        }
        

        PlayAnoSfx();
        
        if (isRigid) ApplyRigid();
        if (isPushed) ApplyPushForce();
        if (isObjSfx) PlayObjectSfx();
        if (isJumpscare) TriggerJumpscare();
        if (isDamage) ApplyDamage();
    }

    private void DestroyAllObjects()
    {
        foreach (var obj in anoObjs)
        {
            Destroy(obj);
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

    private void PlayAnoSfx()
    {
        if (anoSfx != null)
        {
            foreach (var sfx in anoSfx)
            {
                sfx.Play();       
            }
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

    private void TriggerJumpscare()
    {
        NoManualHotelManager.Instance.JumpScareManager.PlayJumpScareEffectToJumpScareId(jumpscareLevel, jumpscareDelay);
    }

    private void ApplyDamage()
    {
        PlayerController.Instance.DecreaseMentality(damageValue);
    }

    private void SfxSettings(AudioSource sfx)
    {
        sfx.playOnAwake = false;
        sfx.loop = false;            // 구분
        sfx.spatialBlend = 1.0f;
        sfx.rolloffMode = AudioRolloffMode.Linear;
        sfx.minDistance = 1;
        sfx.maxDistance = 10;
    }
}
