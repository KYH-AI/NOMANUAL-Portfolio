using System;
using System.Collections;
using System.Collections.Generic;
using NoManual.UI;
using UnityEngine;

namespace NoManual.Interaction
{
    /// <summary>
    /// 상호작용 타입
    /// </summary>
    public enum InteractionType
    {
        None = -1,
        Item = 0,
        ANO = 1,
        PC = 2,
        Door = 3,
        Put = 4,
        Press = 5,
        Read = 6,
    }
    
    public abstract class InteractionBase : MonoBehaviour
    { 
        public InteractionType interactionType { get; set; } = InteractionType.None;
        
        #region 상호작용 UI 출력
        //public TextTip controlType1 = TextTip.None;
       // public TextTip controlType2 = TextTip.None;
        public string title { get; protected set; } = string.Empty;
        
        #endregion
        

        /// <summary>
        /// RayCast Hit 오브젝트랑 특정 상호작용 오브젝트 비교해 반환
        /// </summary>
        /// <param name="target_ANO_Object">RayCast Hit된 상호작용 오브젝트</param>
        public virtual GameObject GetAnotherInteractionObject(GameObject target_ANO_Object)
        {
            if (this.gameObject == target_ANO_Object)
                return this.gameObject;

            return null;
        }
        
        /// <summary>
        /// 상호작용 가능한 오브젝트 부분만 반환
        /// </summary>
        public virtual GameObject GetAnotherInteractionObject()
        {
            return this.gameObject;
        }

        /// <summary>
        /// 각 컴포넌트 상호작용 로직
        /// </summary>
        public abstract void Interact();

        /// <summary>
        /// 각 컴포넌트 RayCast 로직
        /// </summary>
        /// <returns>RayCast 가능 여부확인</returns>
        public virtual bool InteractRayCast()
        {
            return true;
        }

        /// <summary>
        /// 해당 컴포넌트 오브젝트를 더 이상 상호작용 불가능하게 만듬
        /// </summary>
        public virtual void SetNoInteractive()
        {
            this.gameObject.layer = (int)Utils.Layer.LayerIndex.Default;
        }

        /// <summary>
        /// 해당 컴포넌트 오브젝트를 상호작용 가능하게 만듬
        /// </summary>
        public virtual void SetYesInteractive()
        {
            this.gameObject.layer = (int)Utils.Layer.LayerIndex.Interact;
        }
    } 
}


