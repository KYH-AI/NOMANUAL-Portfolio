using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ANO_LookAtConstraint_Component : MonoBehaviour
{
    private LookAtConstraint _lookAtConstraint;

    private void Awake()
    {
        _lookAtConstraint = GetComponentInChildren<LookAtConstraint>();
    }

    private void Start()
    {
         if (_lookAtConstraint == null) return;
         
         _lookAtConstraint.constraintActive = false;
         _lookAtConstraint.SetSources(new List<ConstraintSource>());
         
         ConstraintSource source = new ConstraintSource
         {
             sourceTransform = HFPS.Systems.ScriptManager.Instance.MainCamera.gameObject.transform,
             weight = 1.0f
         };

         _lookAtConstraint.AddSource(source);
         _lookAtConstraint.constraintActive = true;
    }
}
