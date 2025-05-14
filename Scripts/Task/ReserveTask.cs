namespace NoManual.Task
{
    public class ReserveTask : TaskData
    {
        public override string GetTaskTargetLocalization()
        {
            /*
            if (taskTargetId.Equals(TaskHandler.TaskID.Reserve_All_Clear.ToString()))
            {
                return LocalizationTable.Reserve_Item_TableTextKey.RandomReserv_.ToString();
            }
            
            return LocalizationTable.Reserve_Item_TableTextKey.TargetReserv_.ToString();
            */
            
            return string.Empty;
        } 
    }
}

