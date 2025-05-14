using NoManual.Patrol;
using NoManual.Task;

namespace NoManual.Tutorial
{
    public class HandoverPatrolFieldComponent : PatrolFieldComponent
    {
        private TaskHandler.PatrolEventHandler _patrolEventHandler;

        public void Init(TaskHandler.PatrolEventHandler patrolEventHandler)
        {
            _patrolEventHandler -= patrolEventHandler;
            _patrolEventHandler += patrolEventHandler;
            InitializationPatrolFieldObject();
        }
        
        public override void ClearPatrol()
        {
            IsTaskClear = true;
            // ����Ʈ Ŭ����
            _patrolEventHandler?.Invoke(taskID.ToString(), targetTaskID, roomState);
           
        }
    }
}


