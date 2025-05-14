using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NoManual.Task;

public class ScenarioTask
{
    [JsonConverter(typeof(StringEnumConverter))]
    public ProcessType ProcessType = ProcessType.None;

    public int RootId;
    [JsonConverter(typeof(StringEnumConverter))]
    public EndingType EndingType = EndingType.None;

    public int RootDay;

    public int RootRound;
    
    public TaskHandler.StandardTask ScTask;
}

public enum EndingType
{
    None = 0,
    A = 1,
    B = 2,
}

public enum ProcessType
{
    None = -1,
    
    Start = 0,
    Middle = 1,
    End = 2,
}
