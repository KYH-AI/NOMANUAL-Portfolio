namespace NoManual.Task
{
    public class InteractionTask : TaskData
    {
        public string putInstantiatePrefab;
        
        public override string GetTaskTargetLocalization() => LocalizationTable.Object_Item_TableTextKey.Object_.ToString();
    }
}

