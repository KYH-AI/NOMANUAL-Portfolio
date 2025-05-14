using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoManual.NPC
{
    public class NPC_Nurse : NPC_Reality
    {
        public enum NurseAnimationTrigger
        {
            Idle = 0,
            Walk = 1,
            Return = 2,
        }
        
        public Transform[] wayPoints { get; set; }
        private int _currentWayPointIndex = 0;
        private float _moveSpeed = 1f;
        private float _rotationSpeed = 150f;
        public bool ArrivedAtLastWayPoint { get; set; } = false; // 마지막 WayPoint 도착 여부
        public bool IsReverseWayPoint { get; set; } = false; // WayPoint 반대로 이동
        public bool IsTurn { get; set; } = false;
        public bool CanMove { get; set; } = true;

    
        private void Start()
        {
            // 첫  WayPoint로 이동
            if (wayPoints.Length > 0)
            {
                transform.position = wayPoints[_currentWayPointIndex].position;
                PlayAnimation(NurseAnimationTrigger.Walk.ToString());
            }
        }

        private void Update()
        {
            //  Debug.Log($"canMove : {CanMove}, IsTurn : {IsTurn}, ArrivedAtLastWayPoint : {ArrivedAtLastWayPoint}");
            
            if (ArrivedAtLastWayPoint)
            {
                CanMove = false;
            }

            if (IsTurn)
            {
                PlayAnimation(NurseAnimationTrigger.Return.ToString());
                return;
            }

            if (!CanMove) return;
  
            MoveToWayPoint();
            RotateToWayPoint();
        }
        

        /// <summary>
        /// 다음 웨이 포인트 얻기
        /// </summary>
        private int GetNextWayPoint()
        {
            // 다음 웨이포인트 인덱스 계산
            int nextWayPointIndex = _currentWayPointIndex + (IsReverseWayPoint ? -1 : 1);

            // 웨이포인트 인덱스를 클램프하여 범위 내에 유지
            nextWayPointIndex = Mathf.Clamp(nextWayPointIndex, 0, wayPoints.Length - 1);

            return nextWayPointIndex;
        }
        
        
        /// <summary>
        /// 웨이 포인트 이동
        /// </summary>
        private void MoveToWayPoint()
        {
            // 현재 Waypoint에 도착하면 다음 Waypoint로 이동
            if (Vector3.Distance(transform.position, wayPoints[_currentWayPointIndex].position) < 0.1f)
            {
                // 마지막 웨이 포인트 도착여부 확인
                if ((_currentWayPointIndex == 0 && IsReverseWayPoint) || (_currentWayPointIndex == wayPoints.Length - 1 && !IsReverseWayPoint))
                {
                    ArrivedAtLastWayPoint = true;
                    PlayAnimation(NurseAnimationTrigger.Idle.ToString());
                    if(IsReverseWayPoint) this.gameObject.SetActive(false); // 최종적으로 도착하면 간호사 오브젝트 비활성화
                }
                _currentWayPointIndex = GetNextWayPoint();
            }
            
            transform.position = Vector3.MoveTowards(transform.position, wayPoints[_currentWayPointIndex].position, _moveSpeed * Time.deltaTime);
        }

        /// <summary>
        /// 웨이 포인트 회전
        /// </summary>
        private void RotateToWayPoint()
        {
            if (wayPoints.Length == 0) return;

            Vector3 direction = (wayPoints[_currentWayPointIndex].position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            
            
            float rotationStep = _rotationSpeed * Time.deltaTime;
            
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationStep);
          // transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationStep);
        }

        /// <summary>
        /// 도착지점 반대로 설정
        /// </summary>
        public void ReverseWayPoint()
        {
            ArrivedAtLastWayPoint = false;
            IsReverseWayPoint = true;
        }
        
    }
}

