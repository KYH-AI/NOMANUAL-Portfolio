namespace NoManual.Task
{
    public class PutTask : TaskData
    {
        public string [] putRequireItemId;

        public override string GetTaskTargetLocalization()
        {
            if (taskId.Equals(TaskHandler.TaskID.Put_Inventory_Item_Return.ToString()))
                return LocalizationTable.Inventory_Item_TableTextKey.Inventory_Item_.ToString();

            return LocalizationTable.Object_Item_TableTextKey.Object_.ToString();
        } 
    }
}

