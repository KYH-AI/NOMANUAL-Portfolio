using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using NoManual.Interaction;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class BT_DoorHandler : HasEnteredTrigger
{
    public SharedNavMeshAgent navMeshAgent;
    public SharedFloat openDelay;
    private float openTimer = 0f;
    private DoorComponent _door;
    private bool _doorOpenPath = false;
    private Vector3 _targetDest; // 문 Open 후 이동할 위치

    /*
    public override TaskStatus OnUpdate()
    {
        // 문 트리거 감지 X
        if (!enteredTrigger)
        {
            navMeshAgent.Value.isStopped = false;
            return TaskStatus.Failure;
        }
        
        // 문 열기 성공
        if (openTimer >= openDelay || _door.doorState == DoorBaseComponent.DoorStateEnum.Open)
        {
            if (_door.doorState != DoorBaseComponent.DoorStateEnum.Open)
            {
                _door.InteractionDoor();
                // 문이 잠겨있는 경우 
                if (_door.doorState == DoorBaseComponent.DoorStateEnum.Lock) return TaskStatus.Success; 
            }
            
            navMeshAgent.Value.isStopped = false;
            return TaskStatus.Failure;
        }
        
        // 문 열기 진행 타이머
        openTimer += Time.deltaTime;
        return TaskStatus.Running;
    }
    
    public override void OnTriggerEnter(Collider other)
    {
      //  Debug.Log(other.gameObject);
        if (string.IsNullOrEmpty(tag.Value) || other.gameObject.CompareTag(tag.Value)) 
        {
            if (other.TryGetComponent(out _door))
            {
                if (_door.doorState != DoorBaseComponent.DoorStateEnum.Open)
                {
                    openTimer = 0f;
                    enteredTrigger = true;
                    navMeshAgent.Value.isStopped = true;
                }
            }
        }
    }
    */
    
    public override TaskStatus OnUpdate()
    {
        if (!navMeshAgent.Value.isOnOffMeshLink)  return TaskStatus.Failure;
        
        // 퀘스트 관련 Door 무시
        if (_door != null && _door.GetMode == IPut.PutMode.Put) return TaskStatus.Success;
        
        // 문 열기 시작
        if (openTimer >= openDelay.Value)   // || _door.doorState == DoorBaseComponent.DoorStateEnum.Open)
        {
            if (_doorOpenPath)
            {
                Debug.Log("문 개방 후 경로 재계산");
                if (!navMeshAgent.Value.pathPending) return TaskStatus.Running;
                navMeshAgent.Value.isStopped = _doorOpenPath = false;
                return TaskStatus.Failure;
            }

            if (_door.doorState != DoorBaseComponent.DoorStateEnum.Open)
            {
                // 문 열기 시도
                _door.InteractionDoor();
                // 문이 잠겨있는 경우 
                if (_door.doorState == DoorBaseComponent.DoorStateEnum.Lock) return TaskStatus.Success;
            }

            // 문 열기 성공
            if (_door.doorState == DoorBaseComponent.DoorStateEnum.Open && !_door.IsBusy && !_doorOpenPath)
            {
                // 문 열기 전 위치 기억
                _targetDest = navMeshAgent.Value.pathEndPosition;
                
                // 현재 위치를 유지하고 링크를 완료
                Vector3 currentPosition = navMeshAgent.Value.transform.position;  // 현재 위치 기억
                navMeshAgent.Value.CompleteOffMeshLink();  // 링크 완료 처리

                // 워프를 방지하기 위해 다시 현재 위치로 설정 (워프 후 Path는 초기화 됨)
                 navMeshAgent.Value.Warp(currentPosition);

                 navMeshAgent.Value.SetDestination(_targetDest);
                 
                 // 문 Open 후 이동할 위치 계산
                 if (!navMeshAgent.Value.pathPending)
                     _doorOpenPath = true;
#if UNITY_EDITOR
                Debug.Log("문 열기 성공");
#endif                 
                
            }
        }

        // 문 컴포넌트 찾기
        if (!enteredTrigger)
        {
            SetDoor();
            if (!_door)
            {
                navMeshAgent.Value.isStopped = false;
                return TaskStatus.Success;
            }
            navMeshAgent.Value.isStopped = enteredTrigger = true;
            openTimer = 0f;
        }

        // 문을 찾은 경우 문 방향으로 회전
        if (_door) LookAtDoor();
        
        // 문 열기 진행 타이머
#if UNITY_EDITOR
        Debug.Log("문 여는 중");
#endif
        openTimer += Time.deltaTime;
        return TaskStatus.Running;
    }

    private void SetDoor()
    {
        /*
        // 현재 사용 중인 OffMeshLink 정보를 가져옴
        OffMeshLinkData linkData = navMeshAgent.Value.currentOffMeshLinkData;
        
        // 링크의 시작과 끝 위치를 확인
        Vector3 linkStart = linkData.startPos;
        Vector3 linkEnd = linkData.endPos;
        Vector3 direction = (linkEnd - linkStart).normalized;
        float distance = Vector3.Distance(linkStart, linkEnd);
        
        // Y축에 오프셋 추가
        linkStart.y += 0.5f;
        linkEnd.y += 0.5f;

        linkGizomStart = linkStart;
        linkGizmoEnd = linkEnd;
        
        // 라인캐스트로 링크의 시작과 끝 사이에 있는 모든 오브젝트 탐색
        RaycastHit[] hits = Physics.RaycastAll(linkStart, direction, distance);

        // 모든 충돌한 오브젝트를 확인
        foreach (RaycastHit hit in hits)
        {
            // 문 오브젝트인지 확인 (문을 식별할 수 있는 태그나 컴포넌트로 확인)
            if (!hit.collider.CompareTag(tag.Value)) continue;
            if (!hit.collider.gameObject.TryGetComponent(out _door)) continue;
            if (_door.doorState != DoorBaseComponent.DoorStateEnum.Open)
            {
                openTimer = 0f;
                // 문 여는 동안 이동대기
                navMeshAgent.Value.isStopped = true;
                return _door;
            }
        }
        return _door;
        */

        if (navMeshAgent.Value.navMeshOwner is NavMeshLink doorLink)
        {
            doorLink.gameObject.TryGetComponent(out _door);
        }
    }

    private void LookAtDoor()
    {
        Vector3 doorDir = (_door.transform.position - transform.position).normalized;
        if (doorDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(doorDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f); // 회전
        }
    }
    
    public override void OnEnd()
    {
        // 현재 위치를 유지하고 링크를 완료
        if(navMeshAgent.Value.isOnOffMeshLink)
        {
#if UNITY_EDITOR
            Debug.Log("BT_Door End Is OffMeshLink");
#endif              
            Vector3 currentPosition = navMeshAgent.Value.transform.position;  // 현재 위치 기억
            navMeshAgent.Value.CompleteOffMeshLink();  // 링크 완료 처리
            navMeshAgent.Value.Warp(currentPosition);   // 워프를 방지하기 위해 다시 현재 위치로 설정
        }

        _doorOpenPath = false;
        navMeshAgent.Value.isStopped = false;
        enteredTrigger = false;
        openTimer = 0f;
#if UNITY_EDITOR
        Debug.Log("BT_Door On End");
#endif        
     
    }
    
}
