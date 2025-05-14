using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadAndChestIKHandler
{
    private RigMaster _rigMaster;
    private HeadAndChestIK _headAndChestIK;

    public HeadAndChestIKHandler(RigMaster rigMaster, HeadAndChestIK headAndChestIK, Transform targetTransform)
    {
        this._rigMaster = rigMaster;
        this._headAndChestIK = headAndChestIK;
        
        SetRigWeight(1f);
        SetAimTarget(targetTransform, 1.0f, 0.3f);
    }


    public void SetRigWeight(float weight)
    {
        _rigMaster.SetRigWeight(weight);
    }

    public void SetHeadIKWeight(float weight)
    {
        _headAndChestIK.SetHeadIKWeight(weight);   
    }

    public void SetChestIKWeight(float weight)
    {
        _headAndChestIK.SetChestIKWeight(weight);
    }

    public void SetAimTarget(Transform targetTransform, float headTargetWeight, float chestTargetWeight)
    {
        if (!targetTransform) return;
        _headAndChestIK.SetAimTarget(targetTransform, headTargetWeight, chestTargetWeight);
    }
}
