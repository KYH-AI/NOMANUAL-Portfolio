using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoManual.Creature
{
    public class CreatureFieldOfViewGizmo : MonoBehaviour
    {
        [SerializeField] private bool showGizmo;
        
        
        [SerializeField] private Color gizmoColor;
        [SerializeField] private float fieldOfViewAngle;
        [SerializeField] private float viewDistance;
            
    #if UNITY_EDITOR
            public  void OnDrawGizmos()
            {
                if (showGizmo)
                {
                    var oldColor = UnityEditor.Handles.color;
                    var color = gizmoColor;
                    color.a = 0.1f;
                    UnityEditor.Handles.color = color;
                
                    var halfFOV = fieldOfViewAngle * 0.5f;
                    var beginDirection = Quaternion.AngleAxis(-halfFOV, Vector3.up) * transform.forward;
                    UnityEditor.Handles.DrawSolidArc(transform.position, transform.up, beginDirection, fieldOfViewAngle, viewDistance);
                    UnityEditor.Handles.color = oldColor;
                }
            }
    #endif
    }
}

