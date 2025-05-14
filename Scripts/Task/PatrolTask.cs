using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NoManual.UI;

namespace NoManual.Task
{
    public class PatrolTask : TaskData
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public UI_GuestRoomItem.RoomState roomState = UI_GuestRoomItem.RoomState.None;
        public override string GetTaskTargetLocalization() => LocalizationTable.Object_Item_TableTextKey.Object_.ToString();
     
    }
}

