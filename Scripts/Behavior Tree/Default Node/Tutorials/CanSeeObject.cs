using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]
    [TaskIcon("Assets/!! MAIN CONTENTS !!/A. Large Files/Unity Assets/Behavior Designer/Editor/{SkinColor}CanSeeObjectIcon.png")]
    public class CanSeeObject : Conditional
    {
        [Tooltip("The object that we are searching and seeking")]
        public SharedGameObject targetObject;
        [Tooltip("The field of view angle of the agent (in degrees)")]
        public SharedFloat fieldOfViewAngle = 90;
        [Tooltip("The distance that the agent can see")]
        public SharedFloat viewDistance = 1000;
        [Tooltip("The object that is within sight")]
        public SharedGameObject returnedObject;

        [Tooltip("크리처 눈 높이")]
        public float eyeHeightOffset = 1.8f;  // 눈 높이를 캐릭터 높이에 맞게 설정
        
        private Vector3 lastRayStart, lastRayEnd; // 레이 시작점과 끝점을 저장
        private bool drawRayCastGizome = false;
        private bool rayCastGizome = false;
        
        public SharedLayerMask rayCastLayer;

     //   public Color gizmoColor;

        
        
        /// <summary>
        /// Returns success if an object was found otherwise failure
        /// </summary>
        /// <returns></returns>
        public override TaskStatus OnUpdate()
        {
            returnedObject.Value = WithinSight(targetObject.Value, fieldOfViewAngle.Value, viewDistance.Value);
            if (returnedObject.Value != null) {
                // Return success if an object was found
                return TaskStatus.Success;
            }
            // An object is not within sight so return failure
            return TaskStatus.Failure;
        }

        /// <summary>
        /// Determines if the targetObject is within sight of the transform.
        /// </summary>
        protected GameObject WithinSight(GameObject targetObject, float fieldOfViewAngle, float viewDistance)
        {
            if (targetObject == null) {
                return null;
            }
            var direction = targetObject.transform.position - transform.position;
            direction.y = 0;
            var angle = Vector3.Angle(direction, transform.forward);
            if (direction.magnitude < viewDistance && angle < fieldOfViewAngle * 0.5f) {
                // The hit agent needs to be within view of the current agent
                if (LineOfSight(targetObject)) {
                    return targetObject; // return the target object meaning it is within sight
                }
                drawRayCastGizome = false;
            }

            drawRayCastGizome = false;
            return null;
        }

        /// <summary>
        /// Returns true if the target object is within the line of sight.
        /// </summary>
        protected bool LineOfSight(GameObject targetObject)
        {
            RaycastHit hit;
            Vector3 eyePosition = transform.position + Vector3.up * eyeHeightOffset;
            
            lastRayStart = eyePosition;  // 레이의 시작점 저장
            lastRayEnd = targetObject.transform.position;  // 레이의 끝점 저장
            drawRayCastGizome = true;
            
            if (Physics.Linecast(eyePosition, targetObject.transform.position, out hit, rayCastLayer.Value,  QueryTriggerInteraction.Ignore)){//,rayCastLayer.Value)){
#if UNITY_EDITOR
                Debug.Log(hit.collider.gameObject.name);
#endif
                if (hit.transform.IsChildOf(targetObject.transform) || targetObject.transform.IsChildOf(hit.transform)) {
                    return rayCastGizome = true;
                }
                rayCastGizome = false;
            }
            return false;
        }

        
        /// <summary>
        /// Draws the line of sight representation (시야 관련 기즈모는 Creature 스크립트로 대체)
        /// </summary>
/*
        public override void OnDrawGizmos()
        {

#if UNITY_EDITOR
            var oldColor = UnityEditor.Handles.color;
            var color = gizmoColor;
            color.a = 0.1f;
            UnityEditor.Handles.color = color;
            
            var halfFOV = fieldOfViewAngle.Value * 0.5f;
            var beginDirection = Quaternion.AngleAxis(-halfFOV, Vector3.up) * Owner.transform.forward;
            UnityEditor.Handles.DrawSolidArc(Owner.transform.position, Owner.transform.up, beginDirection, fieldOfViewAngle.Value, viewDistance.Value);

            UnityEditor.Handles.color = oldColor;
#endif
        }
        */
    }
}