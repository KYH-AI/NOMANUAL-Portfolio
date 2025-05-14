using System;
using System.Collections;
using NoManual.Managers;
using NoManual.Interaction;
using NoManual.Task;
using UnityEngine;

namespace  NoManual.ANO
{
    /// <summary>
    /// ANO 오브젝트 부모 컴포넌트
    /// </summary>
    public class ANO_Component : InteractionBase
    {
        // ANO 복사본 데이터
        public ANO_CloneData AnoCloneData { get; private set; } = null;
        
        // ANO 조치 확인용
        protected bool IsClear { get; set; } = false;
        // ANO 악화 진행 확인용
        protected bool IsDeterioration { get; private set; } = false;
        // 스티커 부착 위치
        [Header("스티거 부착 대상 ANO 오브젝트 (생략 가능)")] 
        [SerializeField] protected GameObject[] differentialStickerTargetObjects;

        #region ANO Legacy
        
        // ANO 복사본 데이터 (Legacy)
        public ANO_CloneData_Legacy AnoCloneDataLegacy { get; private set; }
        // ANO 위험도(Legacy)
        public ANORatingType anoRatingType { get; private set; } = ANORatingType.None;
        // ANO Idle 오브젝트 비활성화 ID (기본 값 : -1) (Legacy)
        protected int anoIdleObjectId = -1;
        
        #endregion

        /// <summary>
        /// 스티커 부착 좌표
        /// </summary>
        protected Vector3 interactANO_StickerPoint { get; private set; }
        protected Vector3 interactANO_StickerNormal { get; private set; }

        protected void Awake()
        {
            interactionType = InteractionType.ANO;
        }

        /// <summary>
        /// ANO 데이터 초기화 
        /// </summary>
        public void InitializeANO(ANO_CloneData anoCloneData)
        {
            this.AnoCloneData = anoCloneData;
        }

        
        /// <summary>
        /// ANO 데이터 초기화 (Legacy)
        /// </summary>
        public void InitializeANO(ANO_CloneData_Legacy anoCloneDataLegacy, ANORatingType anoRatingType, int anoIdleObjectId)
        {
            this.AnoCloneDataLegacy = anoCloneDataLegacy;
            this.anoRatingType = anoRatingType;
            // -1이 아닌경우 특수한 Idle 오브젝트를 가진 경우 (ANO15 같은경우)
            if (this.anoIdleObjectId == -1)
            {
                this.anoIdleObjectId = anoIdleObjectId;
            }
            
            Debug.Log($"{anoCloneDataLegacy.ANOTitle}의 위험 등급은 : {anoRatingType}");
            
            // ANO idle 오브젝트 비활성화
            Disable_ANO_IdleObject();
        }
        

        /// <summary>
        /// ANO 스티커 부착 위치 기록
        /// </summary>
        public void Set_ANO_StickerPoint(Vector3 point, Vector3 normal)
        {
            interactANO_StickerPoint = point;
            interactANO_StickerNormal = normal;
        }

        /// <summary>
        /// ANO 조치 결과 확인 [Floating Icon UI 확인 용도]
        /// </summary>
        public bool Get_ANO_ClearStatus()
        {
            return IsClear;
        }
        
        /// <summary>
        /// ANO 최종 조치 결과 확인 (Idle 오브젝트 원상복구) (Legacy)
        /// </summary>
        public bool Reporting_ANO_Clear()
        {
            // ANO idle 오브젝트 활성화 (원상복구)
            Enable_ANO_IdleObject();
            return IsClear;
        }

        /// <summary>
        /// ANO 악화 확인 (Legacy)
        /// </summary>
        public bool CheckANODeterioration()
        {
            return IsDeterioration;
        }
        
        
        /// <summary>
        /// ANO 클리어 처리
        /// </summary>
        /// <param name="isClearANO">클리어 여부 확인</param>
        public virtual void ANO_Clear(bool isClearANO)
        {
            IsClear = isClearANO;
            HotelManager.Instance.ANO.BonusTaskTrigger(TaskHandler.TaskType.Interaction, AnoCloneData.ANO_Id.ToString());
            ANO_ClearAction();
            SetNoInteractive();
        }
        
        /// <summary>
        /// ANO 비활성화
        /// </summary>
        protected void DisableANO()
        {
            HotelManager.Instance.ANO.DisableANO(this);
        }
        
        /// <summary>
        /// ANO 오브젝트 삭제
        /// </summary>
        protected void DeleteANO()
        {
            HotelManager.Instance.ANO.DeleteANO(this);
        }
        
        /// <summary>
        /// 악화 시작 타이머 (Legacy)
        /// </summary>
        protected void StartDeteriorationTimer()
        {
            StartCoroutine(nameof(DeteriorationTimerProcess));
        }

        /// <summary>
        /// 악화 타이머 코루틴 종료 (Legacy)
        /// </summary>
        protected void StopANODeteriorationTimer()
        {
            StopAllCoroutines();
        }
        
        /// <summary>
        /// 악화 타이머 코루틴 (Legacy)
        /// </summary>
        private IEnumerator DeteriorationTimerProcess()
        {
            float currentTimer = 0f;
            // anoCloneData에서 악화 최대 시간 설정
            float anoDeteriorationMaxTime = AnoCloneDataLegacy.anoDeteriorationSettings.deteriorationTimer;
            
            // 현재 타이머가 설정한 최대 시간에 도달할 때까지 반복
            while (currentTimer <= anoDeteriorationMaxTime)
            {
                // Time.deltaTime은 마지막 프레임이 완료된 이후 경과한 시간(초)을 반환
                // 이 값을 현재 타이머에 추가
                currentTimer += Time.deltaTime;
        
                // 다음 프레임까지 기다림
                yield return null;
            }

            // 악화 진행 완료
            IsDeterioration = true;
        }
        
        
        /// <summary>
        /// ANO Damage Trigger Zone 확인 (TODO : 수정)
        /// </summary>
        protected void ANO_DamageTriggerCheck()
        {
            int mentalityDamage = AnoCloneDataLegacy.anoRatingSettings.mentalityDamage;
            
            // ANO에 정신력 피해가 없으면 생략
            if(mentalityDamage <= 0) return;
            
            // 플레이어 정신력 피해
            HFPS.Player.PlayerController.Instance.DecreaseMentality(mentalityDamage);
            
        }
        
        
        #region ANO Idle Object (Legacy)

        
        /// <summary>
        /// ANO Idle 오브젝트 활성화 (-1 경우 Idle 오브젝트 없음)
        /// </summary>
        private void Enable_ANO_IdleObject()
        {
            if (anoIdleObjectId == -1) return;
            HotelManager.Instance.ANO.Enable_ANO_IdleObject(anoIdleObjectId);
        }

        /// <summary>
        /// ANO Idle 오브젝트 비활성화  (-1 경우 Idle 오브젝트 없음)
        /// </summary>
        protected void Disable_ANO_IdleObject()
        {
            if (anoIdleObjectId == -1) return;
            HotelManager.Instance.ANO.Disable_ANO_IdleObject(anoIdleObjectId);
        }

        #endregion

        #region DropItem 확인

        public bool DropItemCheck(int itemId)
        {



            return default;
        }

        #endregion


        #region 추상화 함수

        /// <summary>
        /// ANO Trigger Zone 확인
        /// </summary>
        /// <param name="anoTriggerZone">플레이어가 감지한 ANO Trigger Zone</param>
        public virtual void ANO_TriggerCheck(Collider anoTriggerZone) {}

        /// <summary>
        /// ANO End시 행동 (생략가능) (예 : 스티커 부착 시 애니메이션 종료)
        /// </summary>
        protected virtual void ANO_ClearAction() {}

        /// <summary>
        /// 스티커 부착 가능한 오브젝트 얻기
        /// </summary>
        public sealed override GameObject GetAnotherInteractionObject(GameObject target_ANO_Object)
        {
            foreach (var anoObject in differentialStickerTargetObjects)
            {
                if(target_ANO_Object == anoObject)
                    return anoObject;
            }
            return null;
        }

        /// <summary>
        /// (추상화) ANO 상호작용
        /// </summary>
        public override void Interact()
        {
            GameObject sticker = Instantiate(HotelManager.Instance.ANO.anoStickerPrefab, interactANO_StickerPoint, Quaternion.identity);
            
            // hit.Normal 값 기준으로 스티커를 바라보게 하기
            sticker.transform.forward = interactANO_StickerNormal;
            sticker.transform.parent = this.gameObject.transform;// ?? GetAnotherInteractionObject(this.gameObject).transform;
            
            // ANO 스티커 처리 완료
            ANO_Clear(true);
        }

        /// <summary>
        /// (추상화) ANO RayCast 확인
        /// </summary>
        public override bool InteractRayCast()
        {
            return !Get_ANO_ClearStatus();
        }

        /// <summary>
        /// (추상화) 상호작용 Layer Default로 변경
        /// </summary>
        public override void SetNoInteractive()
        {
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.layer = (int)Utils.Layer.LayerIndex.Default;
            }
        }

        #endregion
    }
}



