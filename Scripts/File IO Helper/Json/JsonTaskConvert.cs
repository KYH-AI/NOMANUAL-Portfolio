using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using NoManual.Task;


/* 다음과 같은 방법으로 해결해서 'JsonConverter'는 사용 X =>  JSON 추상화 클래스를 역직렬화  참조 : https://appetere.com/blog/serializing-interfaces-with-jsonnet/
   1. $type을 Json에서 이용
   2. $type과 매핑이 되게 ISerializationBinder 상속받은 클래스를 구현
 
 */
/// <summary>
/// 퀘스트 Json 상속 캐스팅 (24.08.06 사용 X)
/// </summary>
public class JsonTaskConvert : JsonConverter
{
    /// <summary>
    /// 해당 타입부터 변환 시작
    /// </summary>
    public override bool CanConvert(Type taskType)
    {
        return taskType == typeof(TaskHandler.StandardTask);
    }
    
    /// <summary>
    /// JSON 상속 캐스팅 
    /// </summary>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        
        JObject jsonObject = JObject.Load(reader);
        
        // 전체 속성을 매핑
        TaskHandler.StandardTask standardTask = new TaskHandler.StandardTask();
        serializer.Populate(jsonObject.CreateReader(), standardTask);
        
        // taskType 파트 부분만 읽어와 변환시작
        TaskHandler.TaskType taskType = jsonObject["taskType"].ToObject<TaskHandler.TaskType>();
        
        // taskType 분류하기
        TaskData taskData = standardTask.taskData;
        switch (taskType)
        {
            case TaskHandler.TaskType.PopUp:
                taskData = new PopUpTask();
                break;
            
            case TaskHandler.TaskType.Patrol:
                taskData = new PatrolTask();
                break;
            
            case TaskHandler.TaskType.Get:
                taskData = new GetTask();
                break;
            
            case TaskHandler.TaskType.Put:
                taskData = new PutTask();
                break;
            
            case TaskHandler.TaskType.Interaction:
                taskData = new InteractionTask();
                break;
            
            case TaskHandler.TaskType.Reserve:
                taskData = new ReserveTask();
                break;
        }

        if (taskData != null)
            // JSON의 나머지 속성을 taskData에 매핑
            serializer.Populate(jsonObject["taskData"].CreateReader(), taskData);
        
        return taskData;
    }

   
    
    /// <summary>
    /// 사용 X
    /// </summary>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
