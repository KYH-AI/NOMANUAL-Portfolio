using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoManual.Interaction
{
    public abstract class DoorBaseComponent : InteractionBase
    {
        public enum DoorStateEnum
        {
            Close = 0,
            Open = 1,
            Lock = 2,
        }
        
        [Header("문 상태")]
        public DoorStateEnum doorState = DoorStateEnum.Close;

        [Header("문 손잡이 오브젝트")] 
        public GameObject doorHandleObject;
        
        public bool IsBusy { get; protected set; } = false;
        
        protected void Awake()
        {
            interactionType = InteractionType.Door;
        }
        
        public sealed override void Interact()
        {
            DoorComponentInteract();
        }

        /// <summary>
        /// 문 상호작용 (추상화)
        /// </summary>
        protected abstract void DoorComponentInteract();
    }
}


