using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; 

namespace NoManual.Interaction
{
    public class InteractChangeComponent : InteractObjectBaseComponent
    {
        public enum ChangeType
        {
            Animation = 0, // 제자리에 놓는 컨셉
            Prefab = 1,  // 기존꺼를 방치하고 새로운걸 생성하는 컨셉
        }
        public ChangeType GetChangeType => changeType;
     
        
        [System.Serializable]
        public struct AnimatorChange
        {
             [Header("상호작용 시 애니메이션")]
             public Animator interactObjectAnimator;
             [Header("기존 Idle 오브젝트")] 
             public GameObject idleObject;
             [Header("Idle 위치")] 
             public Transform idleRoot;
             [Header("Idle Task 위치")] 
             public Transform idleTaskRoot;
             
        }
     
        [System.Serializable]
        public struct PrefabChange
        {
            [Header("상호작용 시 생성될 프리팹 오브젝트 ")]
            public GameObject interactInsPrefab;
            [Header("상호작용 시 생성될 프리팹 오브젝트 위치")]
            public Transform interactInsPrefabRoot;
            [Header("기존 Idle 오브젝트")] 
            public GameObject idleObject;
            [Header("상호작용 오브젝트 생성 시 기존 오브젝트 비활성화 여부")]
            public bool disableOriginalObject;
        }

        [Header("상호작용 시 효과음")] 
        [SerializeField] private AudioSource interactAudio;
        [Header("========== 상호작용 연출 타입 ==========")]
        [SerializeField] private ChangeType changeType;
        [Space(5)]
        [Header("(ChangeType = Animator 경우) 애니메이션 연출")] public AnimatorChange animatorChange = new AnimatorChange();
        [Space(5)]
        [Header("(ChangeType = Prefab 경우) 오브젝트 생성 연출")] public PrefabChange prefabChange = new PrefabChange();
            

        
        /// <summary>
        /// 상호작용 오브젝트만 강제 반환
        /// </summary>
        public sealed override GameObject GetAnotherInteractionObject()
        {
            if (!floatingIconRoot)
            {
                return changeType is ChangeType.Animation ? animatorChange.idleObject : prefabChange.idleObject;
            }
            return floatingIconRoot.gameObject;
        }
        
        protected override void InitInteractObject()
        {
            if (changeType is ChangeType.Animation)
            {
                animatorChange.idleObject.transform.position = animatorChange.idleRoot.position;
                animatorChange.idleObject.transform.rotation = animatorChange.idleRoot.rotation;
            }
            else
            {
                // 생성된 오브젝트 삭제
                foreach (Transform insPrefab in prefabChange.interactInsPrefabRoot)
                {
                    Destroy(insPrefab.gameObject);
                }
                if(prefabChange.disableOriginalObject) prefabChange.idleObject.SetActive(true);
            }
        }
        
        
        protected override void SetIdle_To_TaskIdle()
        {
            if (changeType is ChangeType.Animation)
            {
                animatorChange.idleObject.transform.position = animatorChange.idleTaskRoot.position;
                animatorChange.idleObject.transform.rotation = animatorChange.idleTaskRoot.rotation;
            }
            else
            {
                // ? For prefabChange What?
            }
            OnStartEvent?.Invoke();
        }
        
        protected override void SetTaskIdle_To_Idle()
        {
            if (changeType is ChangeType.Animation)
            {
                Sequence sequence = DOTween.Sequence();
                sequence.Append(animatorChange.idleObject.transform.DOMove(animatorChange.idleRoot.position, 1f)
                    .SetEase(Ease.InOutSine)).Join(
                    animatorChange.idleObject.transform.DORotateQuaternion(animatorChange.idleRoot.rotation, 1f)
                        .SetEase(Ease.InOutSine)
                );

            }
            else
            {
                if(prefabChange.disableOriginalObject) prefabChange.idleObject.SetActive(false);
                 Instantiate(prefabChange.interactInsPrefab, prefabChange.interactInsPrefabRoot.position, prefabChange.interactInsPrefabRoot.rotation, prefabChange.interactInsPrefabRoot);
            }
            
            // 상호작용 효과음 재생
            interactAudio.Play();
            OnEndEvent?.Invoke();
        }

        public override void SetNoInteractive()
        {
            if (changeType is ChangeType.Animation) animatorChange.idleObject.layer = (int)Utils.Layer.LayerIndex.Default;
            else prefabChange.idleObject.layer = (int)Utils.Layer.LayerIndex.Default;
        }

        public override void SetYesInteractive()
        {
            if (changeType is ChangeType.Animation) animatorChange.idleObject.layer = (int)Utils.Layer.LayerIndex.Interact;
            else prefabChange.idleObject.layer = (int)Utils.Layer.LayerIndex.Interact;
        }
    }
}


