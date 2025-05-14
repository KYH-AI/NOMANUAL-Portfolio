namespace NoManual.Task
{
    public abstract class TaskData
    {
        // 근무 타입 테이블 ID
        public string taskId;
        //public TaskHandler.TaskType taskTyps = TaskHandler.TaskType.None;
        public string taskTargetId = string.Empty;

        public bool CheckClear(string comparisonTargetId)
        {
            return comparisonTargetId.Equals(taskTargetId);
        }

        public abstract string GetTaskTargetLocalization();
    }
}


