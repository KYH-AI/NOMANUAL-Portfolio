using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NoManual.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace NoManual.Interaction
{
    public class ElevatorComponent : MonoBehaviour
    {
        public enum ElevatorType
        {
            Default = 0,
            ANO = 1,
        }

        public enum ElevatorState
        {
            Close = 0,
            Open = 1,
            Busy = 2,
        }

        public ElevatorType CurrentElevatorType = ElevatorType.Default;
        public ElevatorState CurrentElevatorState = ElevatorState.Close;
        [Header("ANO LED 점등 주기")]
        [SerializeField] private AnimationCurve anoLedChangeCurve;
        public int CurrentElevatorFloor { get; private set; } = 1;


        [Header("엘베 자동 문 닫기 타이머")] 
        [SerializeField]  private float autoCloseTimer = 5f;
        [Header("엘베 호출 & 이동 딜레이")] 
        [SerializeField] private float elevatorFloorMoveDelay;
        [Header("엘베 도착 위치")] 
        [SerializeField] private ElevatorTargetPos[] teleportPosition;

        [Space(10)] 
        [Header("엘배 외부 컴포넌트")] 
        [SerializeField] private ElevatorOutSideFrame[] elevatorOutSideFrames;

        [Space(10)] 
        [Header("엘베 내부 전광판 MeshRenderer")] 
        [SerializeField] private MeshRenderer elevatorLedPanel;

        [Space(10)]
        [Header("엘베 내부 문 애니메이터")] 
        [SerializeField] private Animator elevatorAnimator;

        [Space(10)]
        [Header("엘베 내부 버튼")]
        private ElevatorFloorButton[] _elevatorFloorButtons;

        [Space(10)]
        [Header("엘베 내부 SFX 오디오(One Shot)")]
        [SerializeField] private AudioSource elevatorOneShotAudio;

        [Space(10)]
        [Header("엘베 내부 SFX 오디오1(Loop)")]
        [SerializeField] private AudioSource elevatorLoopAudio1;
        [Space(10)]
        [Header("엘베 내부 SFX 오디오2(Loop)")]
        [SerializeField] private AudioSource elevatorLoopAudio2;
        

        [Space(10)] [Header("=== 엘베 버튼 머티리얼 & LED 텍스쳐  ===")]  [Space(10)]
        [Header("버튼 하이라이팅")] [SerializeField]  private Material buttonSelectHighLightMaterial;
        public Material GetBtnHighLightMaterial => buttonSelectHighLightMaterial;
        
        [System.Serializable]
        public struct LedPanelTexture
        {
            public string Key;
            public Texture EmissionMap;
        }

        [Header("LED")] [SerializeField] private LedPanelTexture[] ledPanelTextures;


        // 문 자동 닫기 타이머
        private Coroutine _autoCloseProcess = null;
        
        // 애니메이터 State string
        private const string _ClOSE_TO_OPEN = "Close_To_Open";
        private const string _OPEN_TO_CLOSE = "Open_To_Close";
        public readonly int OPEN_HASH = Animator.StringToHash("Open");
        public readonly int CLOSE_HASH = Animator.StringToHash("Close");
        
        
        // 엘베 도착 이벤트
        private List<UnityAction> _elevatorArrivedEvent;
        private ElevatorOutSideFrame _targetElevatorOutSideFrame;
        
        private void Awake()
        {
            _elevatorFloorButtons = GetComponentsInChildren<ElevatorFloorButton>();
            foreach (var floorButton in _elevatorFloorButtons) floorButton.InitElevatorButton(this, buttonSelectHighLightMaterial);
            
            // LED 1층으로 초기화
            UpdateAllElevatorFloorLedPanel("1");
        }
        


        /// <summary>
        /// 엘베 초기화 (오브젝트 매니저에서 ANO로 변경시킬건지 ㅇㅇ)
        /// </summary>
        public void InitializationElevator()
        {
            
        }
        
        
        /// <summary>
        /// 엘베 호출 (외부 버튼)
        /// </summary>
        public void CallElevator(int callerFloor, List<UnityAction> endEvent)
        {
            SetNoInteractElevatorButton();
            
            float floorTravelTime = CalcElevatorDelay(callerFloor);
            var targetPos = teleportPosition.FirstOrDefault(floor => floor.TargetFloor == callerFloor);

            _elevatorArrivedEvent = endEvent;
            StartCoroutine(ElevatorTeleportProcess(floorTravelTime, targetPos, false));
        }

        /// <summary>
        /// 엘베 이동 (내부 버튼)
        /// </summary>
        public void TeleportElevator(int targetFloor, List<UnityAction> endEvent)
        {
            SetNoInteractElevatorButton();
            
            // 엘베 도착 이벤트 등록
            _elevatorArrivedEvent = endEvent;
           
            
            // 목적 층이 현재 층과 같은경우
            if (targetFloor == CurrentElevatorFloor)
            {
                if(CurrentElevatorState == ElevatorState.Close)
                {
                    var currentOutSideFrame = elevatorOutSideFrames.FirstOrDefault(frame => frame.GetCurrentFloor == CurrentElevatorFloor);
                    if(currentOutSideFrame) currentOutSideFrame.PlayDoorAnimation(OPEN_HASH);
                    StartCoroutine(DoorAnimationProcess(_ClOSE_TO_OPEN, OPEN_HASH, ElevatorState.Open, true));
                }
                RunElevatorEndEvent();
                return;
            }
            
            ElevatorTargetPos targetPos = teleportPosition.FirstOrDefault(floor => floor.TargetFloor == targetFloor);
            float floorTravelTime  = CalcElevatorDelay(targetFloor);

            if (targetPos != null)
            {
                StartCoroutine(ElevatorTeleportProcess(floorTravelTime, targetPos));
            }
        }


        #region 엘베 코루틴

            private IEnumerator DoorCloseAutoTimer()
            {
                yield return new WaitForSeconds(autoCloseTimer);
                if (CurrentElevatorState != ElevatorState.Busy)
                {
                    StartCoroutine(DoorAnimationProcess(_OPEN_TO_CLOSE,  CLOSE_HASH, ElevatorState.Close, true));
                    if(_targetElevatorOutSideFrame) _targetElevatorOutSideFrame.PlayDoorAnimation(CLOSE_HASH);
                }
            }

            private IEnumerator DoorAnimationProcess(string animStateName, int animHashKey, ElevatorState newState, bool buttonUnLockHandler = false)
            {
                //SetNoInteractElevatorButton();
                ChangeElevatorState(ElevatorState.Busy);
                PlayDoorAnimator(animHashKey);
                
                AnimatorStateInfo animStateInfo;

                // 애니메이션 상태가 매개변수로 전달된 상태와 다르면 기다림
                do
                {
                    animStateInfo = elevatorAnimator.GetCurrentAnimatorStateInfo(0);
                    yield return null; 
                }
                while (!animStateInfo.IsName(animStateName));
                
#if UNITY_EDITOR                
                Debug.Log("애니메이션 변환 완료");
#endif                

                PlayDoorAudioClip(newState);
                
                // 애니메이션이 끝날 때까지 대기
                while (animStateInfo.normalizedTime < 1f)
                {
#if UNITY_EDITOR  
                    Debug.Log("애니메이션 진행 대기 중");
#endif                          
                    animStateInfo = elevatorAnimator.GetCurrentAnimatorStateInfo(0);
                    yield return null; 
                }

                // 상태변경
                ChangeElevatorState(newState);
                
                // Auto Close 타이머 시작
                if (newState == ElevatorState.Open)
                {
                    if(_autoCloseProcess != null)
                    {
                        StopCoroutine(_autoCloseProcess);
                        _autoCloseProcess = null;
                    }
                    _autoCloseProcess = StartCoroutine(DoorCloseAutoTimer());
                }
                
                if(buttonUnLockHandler) SetYesInteractElevatorButton();
            }

            /// <summary>
            /// 엘베 이동
            /// </summary>
            private IEnumerator ElevatorTeleportProcess(float delay, ElevatorTargetPos elevatorTargetPos, bool inPlayer = true)
            {
                bool isANO = CurrentElevatorType is ElevatorType.ANO;
                int defaultTargetFloor = elevatorTargetPos.TargetFloor;
                // ANO 경우 목적지 바꿔치기
                if(isANO && inPlayer) elevatorTargetPos = teleportPosition.FirstOrDefault(floor => floor.TargetFloor == 12);
                
                var currentOutSideFrame = elevatorOutSideFrames.FirstOrDefault(frame => frame.GetCurrentFloor == CurrentElevatorFloor);
                _targetElevatorOutSideFrame = elevatorOutSideFrames.FirstOrDefault(frame => frame.GetCurrentFloor == elevatorTargetPos.TargetFloor);
                
                if (CurrentElevatorState == ElevatorState.Open)
                {
                    // 즉시 문 닫기
                    if(_autoCloseProcess != null)
                    {
                        StopCoroutine(_autoCloseProcess);
                        _autoCloseProcess = null;
                    }
                    StartCoroutine(DoorAnimationProcess(_OPEN_TO_CLOSE, CLOSE_HASH, ElevatorState.Close, false));
                    if (currentOutSideFrame) currentOutSideFrame.PlayDoorAnimation(CLOSE_HASH);
                }

                yield return new WaitUntil(() => CurrentElevatorState == ElevatorState.Close);

                if (inPlayer)
                {
                    // 엘베 점프스케어
                    HFPS.Player.PlayerController.Instance.transform.SetParent(this.transform); // (10.01 절대위치 및 오프셋 보다 안정적이고 끊기는게 없음)
                }

                // ANO 경우 엘베 대기시간(목적지 + ANO) 재설정
               // float totalDelay = delay;
                float anoDelay = 0f;
                if (elevatorTargetPos != null) anoDelay = elevatorTargetPos.TargetFloor;
                
                   // totalDelay = isANO && inPlayer ? delay + elevatorTargetPos.TargetFloor : delay;
                   // anoDelay = elevatorTargetPos.TargetFloor;
                
                StartCoroutine(ElevatorLedProcess(elevatorFloorMoveDelay, defaultTargetFloor, inPlayer, isANO, anoDelay));
                PlayElevatorAudio(SfxEnum.ElevatorMove, true, 0);
                
                
                // 엘베 기본 이동 딜레이
                yield return new WaitForSeconds(delay);
                
                if (isANO && inPlayer)
                {
                    //엘베 이상현상 점프스케어
                    
                    //엘베 ANO Warning 사운드
                    PlayElevatorAudio(SfxEnum.ElevatorAnoWarning, true, 1);
                    
                    // 엘베 ANO 딜레이
                    yield return new WaitForSeconds(anoDelay);
                    
                    StopElevatorAudio(true, true, 1, 2.5f);
                }
                StopElevatorAudio(true, true, 2, 2.5f);
                
                // 엘리베이터 자체 텔레포트
                this.transform.position = elevatorTargetPos.TargetPos.position;
                
                if (inPlayer)
                {
                    // 플레이어 엘베 자식으로 부터 해체
                    HFPS.Player.PlayerController.Instance.transform.SetParent(null);

                    // (10.01 절대위치 및 오프셋 버젼)   
                    /* 플레이어 텔레포트 시 X, Z 오프셋 유지 (문제점 Y축 때문에 발을 보면 1프레임으로 뚝 끊기는게 느껴짐)
                    var lastElevatorPos = teleportPosition.FirstOrDefault(frame => frame.TargetFloor == CurrentElevatorFloor);
                    if (lastElevatorPos != null)
                    {
                        Vector3 lastPlayerPosition = HFPS.Player.PlayerController.Instance.transform.position;
                    
                        // 플레이어와 엘리베이터 간의 상대적인 X, Z 차이 계산
                        Vector3 relativePosition = new Vector3(lastPlayerPosition.x - lastElevatorPos.TargetPos.position.x, 0f, lastPlayerPosition.z - lastElevatorPos.TargetPos.position.z ); 
            
                        Vector3 elevatorTargetPosition = elevatorTargetPos.TargetPos.position;
                        var targetPlayerPosition = new Vector3(
                            elevatorTargetPosition.x + relativePosition.x,  // X축 상대 위치 유지
                            elevatorTargetPosition.y + 0.45f,  // Y축은 목적지 기준으로 고정
                            elevatorTargetPosition.z + relativePosition.z   // Z축 상대 위치 유지
                        );
                        HFPS.Player.PlayerController.Instance.transform.position = targetPlayerPosition;
                        */
                }

                // 엘베 도착 이벤트
                RunElevatorEndEvent();

                // 현재 층 기억 
                if (inPlayer && isANO) CurrentElevatorFloor = 0;
                else CurrentElevatorFloor = elevatorTargetPos.TargetFloor;
                
                
                // 엘베 문 열기 (In, Out)
                StartCoroutine(DoorAnimationProcess(_ClOSE_TO_OPEN, OPEN_HASH, ElevatorState.Open, true));
                if (_targetElevatorOutSideFrame)
                {
                    _targetElevatorOutSideFrame.PlayDoorAnimation(OPEN_HASH);
                }
            }

            /// <summary>
            /// 엘베 LED
            /// </summary>
            private IEnumerator ElevatorLedProcess(float defaultFloorDelay, int targetFloor, bool inPlayer = false, bool shuffle = false, float anoDelay = 0f)
            {
                // 현재 층에서 목적지 층까지 업데이트
                int currentFloor = CurrentElevatorFloor;  // 현재 엘리베이터 층 가져오기

                // 1초마다 층 업데이트
                WaitForSeconds waitForSeconds = new WaitForSeconds(defaultFloorDelay);
                
                
                // 현재 층에서 목적지 층으로 이동하는 과정
                while (currentFloor != targetFloor)
                {
                    // 3초 후에 층이 변경된다고 가정
                    yield return waitForSeconds;

                    // 층을 증가하거나 감소하여 이동
                    currentFloor = currentFloor < targetFloor ? currentFloor + 1 : currentFloor - 1;

                    // 해당 층에 맞게 LED 업데이트
                    UpdateAllElevatorFloorLedPanel(currentFloor.ToString());
                }
                
                // 목표 층에 도착하면 LED 업데이트 종료
                UpdateAllElevatorFloorLedPanel(targetFloor.ToString());
                
                // 갑자기 ANO 
                if( shuffle && inPlayer)
                {
                    float elapsedTime = 0f;
                    float ranValue;

                    // Delay초 동안 랜덤하게 LED 변경
                    while (elapsedTime <= anoDelay)
                    {
                        ranValue = anoLedChangeCurve.Evaluate(elapsedTime / anoDelay);
                        ranValue = Mathf.Max(0.1f, ranValue); // 최소 0.1초로 설정
                        Debug.Log(ranValue);
                        
                        // 랜덤 시간 간격으로 대기
                        yield return new WaitForSeconds(ranValue);

                        // LED 텍스처를 랜덤으로 변경
                        UpdateAllElevatorFloorLedPanel("", true);

                        // 경과 시간 증가
                        elapsedTime += ranValue;
                    }

                    // Delay초 후 마지막으로 LED를 Empty로 설정
                    UpdateAllElevatorFloorLedPanel("Empty");
                }
            }
            
        #endregion

        /// <summary>
        /// 엘베 이동거리 시간 계산
        /// </summary>
        private float CalcElevatorDelay(int targetFloor)
        {
            if (targetFloor == CurrentElevatorFloor) return 0f;
            return Mathf.Abs(CurrentElevatorFloor - targetFloor) * elevatorFloorMoveDelay;
        }

        private void ChangeElevatorState(ElevatorState newState)
        {
            CurrentElevatorState = newState;
        }
        
        private void PlayDoorAnimator(int animHashKey)
        {
            elevatorAnimator.SetTrigger(animHashKey);
        }

        /// <summary>
        /// 엘베 도착 이벤트
        /// </summary>
        private void RunElevatorEndEvent()
        {
            if (_elevatorArrivedEvent != null)
            {
                foreach (var endEvent in _elevatorArrivedEvent)
                {
                    endEvent?.Invoke();
                }
                _elevatorArrivedEvent = null;
            }
            PlayElevatorAudio(SfxEnum.ElevatorBell, false);
            StopElevatorAudio(false, true);
        }

        /// <summary>
        /// 엘베 내부 및 외부 버튼 상호작용 비활성화
        /// </summary>
        public void SetNoInteractElevatorButton()
        {
            foreach (var floorButton in _elevatorFloorButtons) floorButton.SetNoInteractive();
            foreach (var outSideFrame in elevatorOutSideFrames) outSideFrame.GetRecallButton.SetNoInteractive();
        }

        /// <summary>
        /// 엘베 내부 및 외부 버튼 상호작용 활성화
        /// </summary>
        public void SetYesInteractElevatorButton()
        {
            foreach (var floorButton in _elevatorFloorButtons) floorButton.SetYesInteractive();
            foreach (var outSideFrame in elevatorOutSideFrames) outSideFrame.GetRecallButton.SetYesInteractive();
        }

        /// <summary>
        /// 엘베 내부 및 외부 LED 
        /// </summary>
        private void UpdateAllElevatorFloorLedPanel(string ledTextureKey, bool shuffle = false)
        {
            Texture ledTexture = null;
            if (shuffle)
            {
                int randomIndex = UnityEngine.Random.Range(0, ledPanelTextures.Length-1); // 마지막 Empty는 제외
                ledTexture = ledPanelTextures[randomIndex].EmissionMap; 
            }
            else
            {
                ledTexture = ledPanelTextures.FirstOrDefault(t => t.Key.Equals(ledTextureKey)).EmissionMap;
            }
            
            if (!ledTexture) return;

            // 내부 LED    
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            elevatorLedPanel.GetPropertyBlock(mpb);
            mpb.SetTexture("_EmissionMap", ledTexture);
            elevatorLedPanel.SetPropertyBlock(mpb);
            
            // 외부 LED
            foreach (var elevatorOutSideFrame in elevatorOutSideFrames) elevatorOutSideFrame.UpdateLedFloorLED(ledTexture);
        }


        /// <summary>
        /// 엘베 Door 오디오 실행
        /// </summary>
        private void PlayDoorAudioClip(ElevatorState elevatorDoorSoundType)
        {
            SfxEnum sfxEnum;
            
            switch (elevatorDoorSoundType)
            {
                case ElevatorState.Open: sfxEnum = SfxEnum.ElevatorDoorOpen;
                    break;
                case ElevatorState.Close : sfxEnum = SfxEnum.ElevatorDoorClose;
                    break;
                default: return;
            }
            PlayElevatorAudio(sfxEnum, false);
        }

        /// <summary>
        /// 엘베 오디오 실행
        /// </summary>
        private void PlayElevatorAudio(SfxEnum sfxEnum, bool isLoopChannel, int loopChannel = 0)
        {
            AudioClip clip = NoManualHotelManager.Instance.AudioManager.GetAudioClip(sfxEnum);
            
            if (isLoopChannel)
            {
                AudioSource loopAudio = loopChannel == 0 ? elevatorLoopAudio1 : elevatorLoopAudio2;
                if(loopAudio.isPlaying) StopElevatorAudio(false, isLoopChannel, loopChannel);
                loopAudio.clip = clip;
                loopAudio.loop = true;
                loopAudio.volume = 1f;
                loopAudio.Play();
            }
            else
            {
                elevatorOneShotAudio.volume = 1f;
                elevatorOneShotAudio.PlayOneShot(clip);
            }
        }

        /// <summary>
        /// 엘베 오디오 종료
        /// </summary>
        private void StopElevatorAudio(bool isFadeOut, bool isLoopChannel, int loopChannel = 0, float fadeOutDuration = 0f)
        {
            AudioSource audioSource;

            if (isLoopChannel) audioSource = loopChannel == 0 ? elevatorLoopAudio1 : elevatorLoopAudio2;
            else audioSource = elevatorOneShotAudio;

            if (isFadeOut)
            {
                DOTweenManager.FadeAudioSource(!isFadeOut, audioSource, fadeOutDuration, 0f, new List<UnityAction> { audioSource.Stop });
            }
            else
            {
                audioSource.Stop();
            }
        }
    }
}


