using System.Collections;
using System.Collections.Generic;
using NoManual.ANO;
using UnityEngine;

public class ANO_ModuleHandler : ANO_Component
{
    private ANO_BaseModule[] _totalAnoModuleList;
    private Dictionary<Collider, int> anoStartCollider = new Dictionary<Collider, int>(4);

    [Space(20)]
    [Header("====== ANO ��� ��� ����Ʈ ======")]
    public bool useANO_Collider_Object;
    [HideInInspector] public ANO_Collider_Object_Module colliderObjectModule;
    public bool useANO_Drop_Effect;
    [HideInInspector] public ANO_Drop_Effect_Module dropEffectModule;
    public bool useANO_Change_Effect;
    [HideInInspector] public ANO_Change_Effect_Module changeEffectModule;
    public bool useANO_Animation_Effect;
    [HideInInspector] public ANO_Animation_Effect_Module animationEffectModule;

    private void Awake()
    {
        // Ȱ��ȭ�� ����� ������ ����Ʈ ����
        List<ANO_BaseModule> activeModules = new List<ANO_BaseModule>();
        
        if (useANO_Collider_Object)
        {
            activeModules.Add(colliderObjectModule);
        }

        if (useANO_Drop_Effect)
        {
            activeModules.Add(dropEffectModule);
        }

        if (useANO_Change_Effect)
        {
            activeModules.Add(changeEffectModule);
        }
        _totalAnoModuleList = activeModules.ToArray();

        foreach (var anoModule in _totalAnoModuleList)
        {
            if (!anoModule.AutoInit)
            {
                anoModule.Init();
                anoModule.ANO_StartTriggerCompleteEvent -= ANO_StartTriggerCompleteCount;
                anoModule.ANO_StartTriggerCompleteEvent += ANO_StartTriggerCompleteCount;
                if (anoModule.GetAnoStartCollider)
                {
                    if (anoStartCollider.ContainsKey(anoModule.GetAnoStartCollider))
                    {
                        anoStartCollider[anoModule.GetAnoStartCollider]++;
                    }
                    else
                    {
                        anoStartCollider.Add(anoModule.GetAnoStartCollider, 1);
                    }
                }
            }
        }
    }
    
    public override void ANO_TriggerCheck(Collider anoTriggerZone)
    {
        foreach (var anoModule in _totalAnoModuleList)
        {
            anoModule.Run(anoTriggerZone);
        }
    }

    /// <summary>
    /// ANO Trigger ��Ȱ��ȭ
    /// </summary>
    private void ANO_StartTriggerCompleteCount(Collider completeAnoStartTrigger)
    {
        // completeAnoStartTrigger�� anoStartCollider�� �ִ��� Ȯ��
        if (!anoStartCollider.TryGetValue(completeAnoStartTrigger, out int count))
            return;

        // ī��Ʈ ����
        if (count > 0)
        {
            count--;
            // ī��Ʈ�� 0 ������ ��� ��Ȱ��ȭ �� ����
            if (count == 0)
            {
                completeAnoStartTrigger.enabled = false;
                anoStartCollider.Remove(completeAnoStartTrigger);
            }
            else
            {
                // ī��Ʈ�� ������Ʈ
                anoStartCollider[completeAnoStartTrigger] = count;
            }
        }
    }
}
