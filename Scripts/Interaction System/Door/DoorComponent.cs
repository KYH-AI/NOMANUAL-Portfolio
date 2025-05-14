using System;
using System.Collections;
using NoManual.Managers;
using NoManual.Task;
using NoManual.Utils;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace NoManual.Interaction
{
    public class DoorComponent : DoorBaseComponent, IPut
    {
        public enum DoorAnimation
        {
            None = -1,
            Open = 0,
            Close = 1,
            Lock = 2,
            IsPunched = 3,
        }
        
        private readonly int doorOpen = Animator.StringToHash("Open");
        private readonly int doorClose = Animator.StringToHash("Close");
        private readonly int doorLock = Animator.StringToHash("Lock");
        private readonly int doorLocked = Animator.StringToHash("is Locked");
        private readonly int doorPunched = Animator.StringToHash("is Punched");
        [SerializeField] private Animator doorAnimator;
        [SerializeField] private AudioSource doorAudioSource;
        private AudioClip _doorSfxClip;

        #region AI 관련 컴포넌트

        private NavMeshLink _doorLink;
        private NavMeshObstacle _doorObstacle;

        #endregion

        #region Put 관련 데이터 

        public string TaskTargetId = string.Empty;
        public IPut.PutMode CurrentPutMode = IPut.PutMode.None;
        public int[] RequestItemId;
        private bool _removeRequestItem { get; } = true;

        #endregion


        private void Awake()
        {
            base.Awake();
            _doorLink = GetComponent<NavMeshLink>();
            _doorObstacle = GetComponent<NavMeshObstacle>();
        }


        private void Start()
        {
            // 문 초기 상태에 따른 애니메이션 설정
            InitializeDoorState();
        }

        /// <summary>
        /// 문 초기 상태 설정 및 애니메이션 재생
        /// </summary>
        private void InitializeDoorState()
        {
            switch (doorState)
            {
                case DoorStateEnum.Open : PlayDoorAnimation(doorOpen); 
                    break;

                case DoorStateEnum.Lock : DoorLock();
                    break;
            }
        }
        

        /// <summary>
        /// 상호작용 오브젝트만 강제 반환
        /// </summary>
        public sealed override GameObject GetAnotherInteractionObject()
        {
            return doorHandleObject;
        }
        
        /// <summary>
        ///  문 상태 교체
        /// </summary>
        private void ChangeState(DoorStateEnum newState)
        {
            doorState = newState;
        }
        
        
        /// <summary>
        /// 문 상호작용
        /// </summary>
        public void InteractionDoor()
        {
            if (GetMode is IPut.PutMode.None)
            {
                DoorAnimation doorAnimation = DoorAnimation.None;
                switch (doorState)
                {
                    case DoorStateEnum.Open:
                        ChangeState(DoorStateEnum.Close);
                        doorAnimation = DoorAnimation.Close;
                        break;
                    case DoorStateEnum.Close : 
                        ChangeState(DoorStateEnum.Open);
                        doorAnimation = DoorAnimation.Open;
                        break;
                }
                PlayDoorAnimation(doorAnimation);
            }
            else
            {
                PutInteraction();
            }
        }
        
        /// <summary>
        /// 문 애니메이션 실행
        /// </summary>
        private void PlayDoorAnimation(DoorAnimation doorAnimation)
        {
            int doorAniClip = -1;
            _doorSfxClip = null;
            
            // 문이 잠김 상태 확인
            if (GetDoorLockState())
            {
                PlayDoorAnimation(doorLock);
                _doorSfxClip = NoManualHotelManager.Instance.AudioManager.GetAudioClip(SfxEnum.DoorLock);
                return;
            }
            
            switch (doorAnimation)
            {
                 case DoorAnimation.Open:
                    doorAniClip = doorOpen;
                    _doorSfxClip = NoManualHotelManager.Instance.AudioManager.GetAudioClip(SfxEnum.DoorOpen);
                    break;
                 
                 case DoorAnimation.Close:
                     doorAniClip = doorClose;
                     _doorSfxClip = NoManualHotelManager.Instance.AudioManager.GetAudioClip(SfxEnum.DoorClose);
                     break;
                 
                 case DoorAnimation.Lock:
                     doorAniClip = doorLock;
                     _doorSfxClip = NoManualHotelManager.Instance.AudioManager.GetAudioClip(SfxEnum.DoorLock);
                     break;
                 
                 case DoorAnimation.IsPunched:
                     doorAniClip = doorPunched;
                     break;
            }

            if (doorAniClip == -1) return;
            PlayDoorAnimation(doorAniClip);
        }
        
        /// <summary>
        /// 문 애니메이션 실행
        /// </summary>
        private void PlayDoorAnimation(int doorAniClipId)
        {
            IsBusy = true;
            doorAnimator.SetTrigger(doorAniClipId);
            StartCoroutine(nameof(DoorBusyControl));
        }

        /// <summary>
        /// 문 강제 컨트롤 (연출 X)
        /// </summary>
        public void DoorHandler(DoorStateEnum targetState)
        {
            // 현재 상태와 동일하면 실행하지 않음
            if (doorState == targetState) return;

            int doorAniClipId = -1;
            switch (targetState)
            {
                case DoorStateEnum.Open:
                    // 잠겨있는 경우 강제로 Door UnLock
                    if (GetDoorLockState()) DoorUnLock();
                    doorAniClipId = doorOpen;
                    break;

                case DoorStateEnum.Close:
                    // 잠겨있는 경우 강제로 Door UnLock
                    if (GetDoorLockState())
                        DoorUnLock();
                    else
                        doorAniClipId = doorClose;
                    break;

                default:
                    // 문이 열려있으면 문을 닫고 잠금
                    if (doorState == DoorStateEnum.Open) doorAniClipId = doorClose;
                    DoorLock();
                    break;
            }

            if (doorAniClipId != -1) doorAnimator.SetTrigger(doorAniClipId);
            ChangeState(targetState); // 상태 업데이트
            DoorAiStateToggle();
        }

        /// <summary>
        /// 문 효과음 재생
        /// </summary>
        private void PlayDoorSFX(AudioClip clip)
        {
            if (clip is null) return;
            
            doorAudioSource.clip = clip;
            doorAudioSource.Play();
        }

        /// <summary>
        /// 문 잠금
        /// </summary>
        private void DoorLock()
        {
            doorAnimator.SetBool(doorLocked, true);
            ChangeState(DoorStateEnum.Lock);
        }

        /// <summary>
        /// 문 잠금 해체
        /// </summary>
        private void DoorUnLock()
        {
            doorAnimator.SetBool(doorLocked, false);
            ChangeState(DoorStateEnum.Close);
        }

        /// <summary>
        /// 문 잠김 상태 확인
        /// </summary>
        public bool GetDoorLockState()
        {
            if (doorState is DoorStateEnum.Lock)
            {
                // Door 오브젝트 비활성화 시 애니메이터 매개변수 초기화 문제 해결
                DoorLock();
            }
            return  doorAnimator.GetBool(doorLocked);
        }
        
        /// <summary>
        /// 문 잠금 기능
        /// </summary>
        public void ToggleDoorLock()
        {
            // 문이 잠긴 상태
            if (doorState is DoorStateEnum.Lock)
            {
                DoorUnLock(); // 잠금 해제
            }
            // 문이 닫힌 상태
            else if(doorState is DoorStateEnum.Close)
            {
                DoorLock(); // 잠금
            }
        }

        private IEnumerator DoorBusyControl()
        {
            
            /*
            // 현재 문 애니메이션 로그
            string name = string.Empty;
            if (doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Door Open"))
            {
                name = "Door Open";
            }
            else if (doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Door Close"))
            {
                name = "Door Close";
            }
            else if (doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Door Open Idle"))
            {
                name = "Door Open Idle";
            }
            else if (doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Door Close Idle"))
            {
                name = "Door Close Idle";
            }
            
            Debug.Log("Animation : " + name);
            */
            
            
            // 애니메이션 전환시간
            yield return new WaitForSeconds(0.15f);
            PlayDoorSFX(_doorSfxClip);
            yield return new WaitForSeconds(doorAnimator.GetCurrentAnimatorStateInfo(0).length);
            
            
            /*
            // 애니메이션 전환 대기를 위해 초기 지연
            yield return null;
            while (doorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            }
            */
            DoorAiStateToggle();
            IsBusy = false;
        }

        /// <summary>
        ///  문 상호작용
        /// </summary>
        protected override void DoorComponentInteract()
        {
            InteractionDoor();
        }

        /// <summary>
        /// (추상화) 문 RayCast 확인
        /// </summary>
        public override bool InteractRayCast()
        {
            return !IsBusy;
        }

        #region Door Put 관련

        public void InitializationPutMode()
        {
            DoorUnLock();
            SetYesInteractive();
            CurrentPutMode = IPut.PutMode.None;
            RequestItemId = null;

            // Todo => 문 잠금 풀기
            // Todo => SetYesInteractive 호출
            // Todo => CurrentPutMode = None 
            // Todo => RequestItemId 초기화
        }
        
        public IPut.PutMode GetMode => CurrentPutMode;
        
        [ContextMenu("Door Swap Put Mode")]
        // 현재 모드가 None이면 Put으로 변경하고, 그렇지 않으면 None으로 설정
        public void SwapPutMode()
        { 
            CurrentPutMode = CurrentPutMode == IPut.PutMode.None ? IPut.PutMode.Put : IPut.PutMode.None;
            // 문 Lock을 준비를 위해 문 닫기 진행
            if (doorState == DoorStateEnum.Open && CurrentPutMode == IPut.PutMode.Put)
            {
                DoorHandler(DoorStateEnum.Close);
            }
        } 
            
        public bool RemoveInventoryItem() => _removeRequestItem;
        
        public virtual void PutInteraction()
        {
            PutCallBackMapper putCallBack = new PutCallBackMapper(TaskHandler.TaskID.Put_Inventory_Lock_Door.ToString(), 
                                                                    TaskTargetId, 
                                                                    RequestItemId, 
                                                                    DoorLock,
                                                                    this);
            NoManualHotelManager.Instance.InventoryManager.ShowPutInventory(putCallBack);
        }

        #endregion

        #region Door AI 관련

        private void DoorAiStateToggle()
        {
            if (!_doorObstacle)
            {
                _doorObstacle = GetComponent<NavMeshObstacle>();
            }

            if (!_doorLink)
            {
                _doorLink = GetComponent<NavMeshLink>();
            }
            
            _doorObstacle.enabled = doorState is not DoorStateEnum.Open;
            _doorLink.enabled = doorState is not DoorStateEnum.Open;
        }

        #endregion
      
    }
}


