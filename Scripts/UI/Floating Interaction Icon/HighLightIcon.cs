using UnityEngine;
using UnityEngine.Animations;

public class HighLightIcon : MonoBehaviour
{
    [SerializeField] private bool AwkeIsShow = false;
    [SerializeField] private LookAtConstraint lookAtConstraint;
    private bool _init = false;

    private void Start()
    {
        if (_init) return;
        if (AwkeIsShow) SetLookAtTarget();
        this.gameObject.SetActive(AwkeIsShow);
    }

    public void SetItemHighLight()
    {
        _init = true;
        SetLookAtTarget();
        this.gameObject.SetActive(true);
    }

    public void HideItemHighLight()
    {
        this.gameObject.SetActive(false);
    }

    private void SetLookAtTarget()
    {  
        if (lookAtConstraint.sourceCount > 0)  return;
        lookAtConstraint.constraintActive = false;
        ConstraintSource source = new ConstraintSource
        {
            sourceTransform = HFPS.Systems.ScriptManager.Instance.MainCamera.gameObject.transform,
            weight = 1.0f
        };
    
        lookAtConstraint.AddSource(source);
        lookAtConstraint.constraintActive = true;
    }
}
