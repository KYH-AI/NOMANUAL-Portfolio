using NoManual.Managers;
using NoManual.UI;
using UnityEngine;

namespace NoManual.Patrol
{
    
    /// <summary>
    /// 순찰 지역 컴포넌트
    /// </summary>
    public class PatrolFieldComponent : MonoBehaviour
    {
        protected enum PatrolTaskID
        {
            None = -1,
            
            Patrol_Floor_Field = 0,
            Patrol_Room_Field = 1,
            Patrol_Facility_Field = 2,
        }

        [SerializeField] protected PatrolTaskID taskID = PatrolTaskID.None;
        public string TaskID => taskID.ToString();

        [SerializeField] protected string targetTaskID = string.Empty;
        public string TargetTaskID => targetTaskID;
        
        [SerializeField] protected UI_GuestRoomItem.RoomState roomState = UI_GuestRoomItem.RoomState.None;
        public UI_GuestRoomItem.RoomState RoomState
        {
            get => roomState;
            set => roomState = value;
        }

        private int childCount = 0;
        private int clearCount = 0;
        public bool IsTaskClear { get;  protected set; } = false;


        /// <summary>
        /// Patrol 오브젝트 초기화
        /// </summary>
        public void InitializationPatrolFieldObject()
        {
            childCount = transform.childCount;
            TriggerReset();
        }
        
        /// <summary>
        /// 순찰 지역 접촉 트리거
        /// </summary>
        public void ReportTask()
        {
            clearCount++;
            if (clearCount >= childCount)
            {
                ClearPatrol();
            }
        }

        /// <summary>
        /// 순찰 지역 초기화
        /// </summary>
        public void TriggerReset()
        {
            for (int i = 0; i < childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }

            clearCount = 0;
            IsTaskClear = false;
        }

        public virtual void ClearPatrol()
        {
            IsTaskClear = true;
            // 퀘스트 클리어
            HotelManager.Instance.ObjectManager.PatrolFieldEventTrigger(taskID.ToString(), targetTaskID, roomState);
        }

    }
}