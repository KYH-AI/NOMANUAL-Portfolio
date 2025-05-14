#if UNITY_EDITOR
using NoManual.Interaction;
using UnityEditor;

[CustomEditor(typeof(InteractChangeComponent))]
public class InteractObjectComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        
        // Get the target object
        InteractChangeComponent script = (InteractChangeComponent)target;
        
        // Get the serialized object
        SerializedObject serializedObj = serializedObject;
        
        // Start drawing the Inspector
        serializedObj.Update();

        // Draw all fields except animatorChange and prefabChange
        SerializedProperty property = serializedObj.GetIterator();
        property.NextVisible(true); // Skip the script reference
        do
        {
            if (property.name != "animatorChange" && property.name != "prefabChange")
            {
                EditorGUILayout.PropertyField(property, true);
            }
        }
        while (property.NextVisible(false));
        
        // Get the current change type
        InteractChangeComponent.ChangeType changeType = script.GetChangeType;

        // Depending on the change type, show specific fields
        if (changeType == InteractChangeComponent.ChangeType.Animation)
        {
            SerializedProperty animatorChangeProp = serializedObj.FindProperty("animatorChange");
            EditorGUILayout.PropertyField(animatorChangeProp, true);
        }
        else if (changeType == InteractChangeComponent.ChangeType.Prefab)
        {
            SerializedProperty prefabChangeProp = serializedObj.FindProperty("prefabChange");
            EditorGUILayout.PropertyField(prefabChangeProp, true);
        }

        // Apply changes to serialized properties
        serializedObj.ApplyModifiedProperties();
    } 
}

#endif
