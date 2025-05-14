using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HeadAndChestIK : MonoBehaviour
{
    [SerializeField] MultiAimConstraint multiHeadAimConstraint;
    [SerializeField] MultiAimConstraint multiChestAimConstraint;
    [SerializeField] private RigBuilder rigBuilder;
    [SerializeField] private Transform _aimTarget;
    

    /// <summary>
    /// 머리 IK 가중치
    /// </summary>
    public void SetHeadIKWeight(float weight)
    {
        multiHeadAimConstraint.weight = weight;
    }

    /// <summary>
    /// 상체 IK 가중치
    /// </summary>
    /// <param name="weight"></param>
    public void SetChestIKWeight(float weight)
    {
        multiChestAimConstraint.weight = weight;
    }

    /// <summary>
    /// 머리 & 상체 Look At 대상 (Legacy)
    /// </summary>
    public void SetAimTarget(Vector3 targetPos)
    {
        _aimTarget.position = targetPos;
    }
    
    /// <summary>
    /// 머리 & 상체 Look At 대상
    /// </summary>
    public void SetAimTarget(Transform targetTransform, float headWeight = 1.0f, float chestWeight = 1.0f)
    {
        _aimTarget = targetTransform;
        
        // MultiAimConstraint의 목표 위치 설정 (머리)
        var data = multiHeadAimConstraint.data.sourceObjects;
        if (data.Count > 0)
        {
            data.SetTransform(0, targetTransform);
        }
        else
        {
            var sourceObject = new WeightedTransform(targetTransform, headWeight);
            data.Add(sourceObject);
        }
        multiHeadAimConstraint.data.sourceObjects = data;
        
        // MultiAimConstraint의 목표 위치 설정 (상체)
        data = multiChestAimConstraint.data.sourceObjects;
        if (data.Count > 0)
        {
            data.SetTransform(0, targetTransform);
        }
        else
        {
            var sourceObject = new WeightedTransform(targetTransform, chestWeight);
            data.Add(sourceObject);
        }
        multiChestAimConstraint.data.sourceObjects = data;
        
        // rig 빌드 재시작 (비용 비쌈)
        rigBuilder.Build();
    }
}
