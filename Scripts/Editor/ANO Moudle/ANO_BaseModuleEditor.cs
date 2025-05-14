#if UNITY_EDITOR

using NoManual.ANO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ANO_BaseModule), true)]
public class ANO_BaseModuleEditor : Editor
{
    private GUIStyle headerStyle;
    private GUIStyle titelStyle;
    private GUIStyle moduleStyle;
    private int width = 20;
    // ANO_BaseModule �ν��Ͻ� ��������
    private ANO_BaseModule baseModule;
    
    private void OnEnable()
    {
        baseModule = (ANO_BaseModule)target;
    }
    
    public override void OnInspectorGUI()
    {
        // ���� ��Ÿ���� �ʱ�ȭ
        headerStyle =  NoManual.Editors.EditorUtils.CreateTitleStyle(14, FontStyle.Bold, Color.yellow, TextAnchor.MiddleCenter);
        titelStyle = NoManual.Editors.EditorUtils.CreateTitleStyle(13, FontStyle.Bold, Color.white, TextAnchor.MiddleLeft);
        moduleStyle =  NoManual.Editors.EditorUtils.CreateTitleStyle(12, FontStyle.Bold, new Color(153/255f, 204/255f, 255/255f), TextAnchor.MiddleLeft);
        
        serializedObject.Update();
        // ANO_BaseModule�� ���� �Ӽ��� ǥ��
        if (target.GetType() != typeof(ANO_BaseModule))
        {
            DrawPropertiesExcluding(serializedObject, 
                "autoInit", "anoStartCollider", "anoStartColliderAutoDisable", "useJumpScare", "jumpScareModule", 
                "useDamage", "damageValue", "useAudio", "audioModule");
        }
        
        // ANO_BaseModule�� �⺻ �Ӽ��� �׸���
        DrawBaseModuleProperties();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawBaseModuleProperties()
    {
        EditorGUILayout.Space(width);
        
        // �ʱ�ȭ ����
        EditorGUILayout.LabelField("====== �ʱ�ȭ ======", headerStyle);
        EditorGUILayout.LabelField("�ܵ� �ʱ�ȭ[�⺻ �� : False]", titelStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("autoInit"), new GUIContent("Auto Init"));
        
        EditorGUILayout.Space(width);

        // �ݶ��̴� ����
        EditorGUILayout.LabelField("====== �ݶ��̴� ======", headerStyle);
        EditorGUILayout.LabelField("ANO Start �ݶ��̴�", titelStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("anoStartCollider"), new GUIContent("ANO Start Collider"));
        EditorGUILayout.LabelField("ANO Start �ڵ� ���� (True : �ݶ��̴� ��Ȱ��ȭ)", titelStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("anoStartColliderAutoDisable"), new GUIContent("Auto Disable ANO Start Collider"));
        
        EditorGUILayout.Space(width);

        // ���� ���ɾ� ����
        EditorGUILayout.LabelField("====== ���� ���ɾ� ======", headerStyle);
        EditorGUILayout.LabelField("���� ���ɾ� ���", titelStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("useJumpScare"), new GUIContent("Use Jump Scare"));
        // ���� ���ɾ� ��� ǥ��
        SerializedProperty jumpScareModuleProp = serializedObject.FindProperty("jumpScareModule");
        if (serializedObject.FindProperty("useJumpScare").boolValue)
        {
            if (baseModule.jumpScareModule == null)
            {
                baseModule.jumpScareModule = new ANO_JumpScare_Module();
            }
            
            // ������ ��� ���� ����
            GUI.backgroundColor = new Color(0.8f, 0.9f, 1f); // ���� �Ķ���
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Jump Scare Module", moduleStyle);

            // ��� �Ӽ� ǥ��
            EditorGUILayout.PropertyField(jumpScareModuleProp, new GUIContent("Module Properties"), true);
            EditorGUILayout.EndVertical();
    
            // ��� ���� �ʱ�ȭ
            GUI.backgroundColor = Color.white; 
        }

        EditorGUILayout.Space(width);
        
        // ���ŷ� ���� ����
        EditorGUILayout.LabelField("====== ���ŷ� ���� ======", headerStyle);
        EditorGUILayout.LabelField("���ŷ� ���� ���", titelStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("useDamage"), new GUIContent("Use Damage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("damageValue"), new GUIContent("Damage Value"));
        
        EditorGUILayout.Space(width);

        // ����� ����
        EditorGUILayout.LabelField("====== ����� ======", headerStyle);
        EditorGUILayout.LabelField("����� ���", titelStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("useAudio"), new GUIContent("Use Audio"));
        // ����� ��� ǥ��
        SerializedProperty audioModuleProp = serializedObject.FindProperty("audioModule");
        if (serializedObject.FindProperty("useAudio").boolValue)
        {
            if (baseModule.jumpScareModule == null)
            {
                baseModule.jumpScareModule = new ANO_JumpScare_Module();
            }
            
            // ������ ��� ���� ����
            GUI.backgroundColor = new Color(0.8f, 0.9f, 1f); // ���� �Ķ���
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Audio Module", moduleStyle);
            
            // ��� �Ӽ� ǥ��
            EditorGUILayout.PropertyField(audioModuleProp, new GUIContent("Module Properties"), true);
            
            // Loop�� OneShot �Ӽ� �� ���
            SerializedProperty loopProp = audioModuleProp.FindPropertyRelative("loop");
            SerializedProperty oneShotProp = audioModuleProp.FindPropertyRelative("oneShot");
            
            EditorGUILayout.LabelField("����� ���� ���", moduleStyle);
            // Loop�� OneShot�� �������� ��ġ
            EditorGUILayout.BeginHorizontal();
                
            // Loop�� OneShot�� ��ȣ ��Ÿ�� ��� ����
            // ��� �׷� ��Ÿ�Ϸ� ����
            int selectedOption = loopProp.boolValue ? 0 : (oneShotProp.boolValue ? 1 : 0);
            int newSelectedOption = GUILayout.Toolbar(selectedOption, new string[] { "Loop", "One Shot" });

            EditorGUILayout.EndHorizontal();

            // ���õ� �ɼǿ� ���� ������Ƽ �� ������Ʈ
            if (newSelectedOption == 0)
            {
                loopProp.boolValue = true;
                oneShotProp.boolValue = false;
            }
            else
            {
                loopProp.boolValue = false;
                oneShotProp.boolValue = true;
            }
            
            EditorGUILayout.EndVertical();
    
            // ��� ���� �ʱ�ȭ
            GUI.backgroundColor = Color.white; 
        }
    }
}

#endif