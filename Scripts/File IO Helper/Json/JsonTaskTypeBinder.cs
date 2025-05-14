using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NoManual.Task;

/// <summary>
/// Json Task에서 $type 네임스페이스와 어셈블리어 자동화 기능
/// </summary>
public class JsonTaskTypeBinder : ISerializationBinder
{
    public IList<Type> taskTypeList { get; set; }

    public Type BindToType(string assemblyName, string typeName)
    {
        return taskTypeList.SingleOrDefault(typeN => typeN.Name == typeName);
    }

    public void BindToName(Type serializedType, out string assemblyName, out string typeName)
    {
        assemblyName = null;
        typeName = serializedType.Name;
    }
}

public class JsonTaskConvertSetting
{
    /// <summary>
    /// Json Task 변환기
    /// </summary>
    /// <returns></returns>
    public static JsonSerializerSettings GetTaskConvert()
    {
        return new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = new JsonTaskTypeBinder
            {
                taskTypeList = new List<Type>
                {
                    typeof(GetTask),
                    typeof(InteractionTask),
                    typeof(PatrolTask),
                    typeof(PopUpTask),
                    typeof(PutTask),
                    typeof(ReserveTask),
                    typeof(TradersBuyTask)
                }
            }
        };
    }
}
