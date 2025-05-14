#if UNITY_EDITOR

using NoManual.ANO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ANO_BaseModule), true)]
public class ANO_BaseModuleEditor : Editor
{
    /*
    // 폴더의 열림/닫힘 상태를 저장할 변수
    private bool audioModuleFoldout = true;
    private bool jumpScareFoldout = true;
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 기본 인스펙터 요소 그리기 (특정 모듈들 제외)
        DrawPropertiesExcluding(serializedObject, "audioModule", "jumpScareModule");

        // ANO_BaseModule 인스턴스 가져오기
        ANO_BaseModule baseModule = (ANO_BaseModule)target;
        
        // JumpScare
        if (baseModule.UseJumpScare)
        {
            
            jumpScareFoldout = EditorGUILayout.Foldout(jumpScareFoldout, "Jump Scare Module", true);

            if (jumpScareFoldout)
            {
                if (baseModule.jumpScareModule == null)
                {
                    baseModule.jumpScareModule = new ANO_JumpScare_Module();
                }
                
                EditorGUI.indentLevel++;
                SerializedProperty jumpScareModuleProp = serializedObject.FindProperty("jumpScareModule");
                EditorGUILayout.PropertyField(jumpScareModuleProp, new GUIContent("Jump Scare Module"));
                EditorGUI.indentLevel--;
            }

        }

        // UseAudio
        if (baseModule.UseAudio)
        {
            // audioModule이 null인 경우 인스턴스를 생성
            if (baseModule.audioModule == null)
            {
                baseModule.audioModule = new ANO_Audio_Module();
            }

            // audioModule 필드를 인스펙터에 표시
            SerializedProperty audioModuleProp = serializedObject.FindProperty("audioModule");
            
            // Foldout으로 오디오 모듈 섹션 생성
            audioModuleFoldout = EditorGUILayout.Foldout(
                audioModuleFoldout, 
                new GUIContent("AUDIO MODULE"), 
                true, 
                EditorStyles.foldoutHeader
            );

            if (audioModuleFoldout)
            {
                EditorGUI.indentLevel++;

                // 기존 필드들 표시
                SerializedProperty audioSourceProp = audioModuleProp.FindPropertyRelative("audioSource");
                SerializedProperty audioClipProp = audioModuleProp.FindPropertyRelative("audioClip");
                SerializedProperty loopProp = audioModuleProp.FindPropertyRelative("loop");
                SerializedProperty oneShotProp = audioModuleProp.FindPropertyRelative("oneShot");

                // 기본 필드들 표시
                EditorGUILayout.PropertyField(audioSourceProp);
                EditorGUILayout.PropertyField(audioClipProp);

                // Loop와 OneShot을 수평으로 배치
                EditorGUILayout.BeginHorizontal();
                
                // Loop와 OneShot의 상호 배타적 토글 설정
                // 토글 그룹 스타일로 변경
                int selectedOption = loopProp.boolValue ? 0 : (oneShotProp.boolValue ? 1 : 0);
                int newSelectedOption = GUILayout.Toolbar(selectedOption, new string[] { "Loop", "One Shot" });

                EditorGUILayout.EndHorizontal();

                // 선택된 옵션에 따라 프로퍼티 값 업데이트
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
                EditorGUI.indentLevel--;
            }
        }
        else
        {
            // UseAudio가 비활성화되면 audioModule 인스턴스 제거
            if (baseModule.audioModule != null)
            {
                baseModule.audioModule = null;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
    */
    
    private GUIStyle headerStyle;
    private GUIStyle titelStyle;
    private GUIStyle moduleStyle;
    private int width = 20;
    // ANO_BaseModule 인스턴스 가져오기
    private ANO_BaseModule baseModule;
    
    private void OnEnable()
    {
        baseModule = (ANO_BaseModule)target;
    }
    
    public override void OnInspectorGUI()
    {
        // 제목 스타일을 초기화
        headerStyle =  NoManual.Editors.EditorUtils.CreateTitleStyle(14, FontStyle.Bold, Color.yellow, TextAnchor.MiddleCenter);
        titelStyle = NoManual.Editors.EditorUtils.CreateTitleStyle(13, FontStyle.Bold, Color.white, TextAnchor.MiddleLeft);
        moduleStyle =  NoManual.Editors.EditorUtils.CreateTitleStyle(12, FontStyle.Bold, new Color(153/255f, 204/255f, 255/255f), TextAnchor.MiddleLeft);
        
        serializedObject.Update();
        // ANO_BaseModule의 공통 속성만 표시
        if (target.GetType() != typeof(ANO_BaseModule))
        {
            DrawPropertiesExcluding(serializedObject, 
                "autoInit", "anoStartCollider", "anoStartColliderAutoDisable", "useJumpScare", "jumpScareModule", 
                "useDamage", "damageValue", "useAudio", "audioModule");
        }
        
        // ANO_BaseModule의 기본 속성들 그리기
        DrawBaseModuleProperties();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawBaseModuleProperties()
    {
        EditorGUILayout.Space(width);
        
        // 초기화 관련
        EditorGUILayout.LabelField("====== 초기화 ======", headerStyle);
        EditorGUILayout.LabelField("단독 초기화[기본 값 : False]", titelStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("autoInit"), new GUIContent("Auto Init"));
        
        EditorGUILayout.Space(width);

        // 콜라이더 관련
        EditorGUILayout.LabelField("====== 콜라이더 ======", headerStyle);
        EditorGUILayout.LabelField("ANO Start 콜라이더", titelStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("anoStartCollider"), new GUIContent("ANO Start Collider"));
        EditorGUILayout.LabelField("ANO Start 자동 중지 (True : 콜라이더 비활성화)", titelStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("anoStartColliderAutoDisable"), new GUIContent("Auto Disable ANO Start Collider"));
        
        EditorGUILayout.Space(width);

        // 점프 스케어 관련
        EditorGUILayout.LabelField("====== 점프 스케어 ======", headerStyle);
        EditorGUILayout.LabelField("점프 스케어 사용", titelStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("useJumpScare"), new GUIContent("Use Jump Scare"));
        // 점프 스케어 모듈 표시
        SerializedProperty jumpScareModuleProp = serializedObject.FindProperty("jumpScareModule");
        if (serializedObject.FindProperty("useJumpScare").boolValue)
        {
            if (baseModule.jumpScareModule == null)
            {
                baseModule.jumpScareModule = new ANO_JumpScare_Module();
            }
            
            // 강조된 배경 색상 설정
            GUI.backgroundColor = new Color(0.8f, 0.9f, 1f); // 연한 파란색
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Jump Scare Module", moduleStyle);

            // 모듈 속성 표시
            EditorGUILayout.PropertyField(jumpScareModuleProp, new GUIContent("Module Properties"), true);
            EditorGUILayout.EndVertical();
    
            // 배경 색상 초기화
            GUI.backgroundColor = Color.white; 
        }

        EditorGUILayout.Space(width);
        
        // 정신력 공격 관련
        EditorGUILayout.LabelField("====== 정신력 공격 ======", headerStyle);
        EditorGUILayout.LabelField("정신력 공격 사용", titelStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("useDamage"), new GUIContent("Use Damage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("damageValue"), new GUIContent("Damage Value"));
        
        EditorGUILayout.Space(width);

        // 오디오 관련
        EditorGUILayout.LabelField("====== 오디오 ======", headerStyle);
        EditorGUILayout.LabelField("오디오 사용", titelStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("useAudio"), new GUIContent("Use Audio"));
        // 오디오 모듈 표시
        SerializedProperty audioModuleProp = serializedObject.FindProperty("audioModule");
        if (serializedObject.FindProperty("useAudio").boolValue)
        {
            if (baseModule.jumpScareModule == null)
            {
                baseModule.jumpScareModule = new ANO_JumpScare_Module();
            }
            
            // 강조된 배경 색상 설정
            GUI.backgroundColor = new Color(0.8f, 0.9f, 1f); // 연한 파란색
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Audio Module", moduleStyle);
            
            // 모듈 속성 표시
            EditorGUILayout.PropertyField(audioModuleProp, new GUIContent("Module Properties"), true);
            
            // Loop와 OneShot 속성 값 얻기
            SerializedProperty loopProp = audioModuleProp.FindPropertyRelative("loop");
            SerializedProperty oneShotProp = audioModuleProp.FindPropertyRelative("oneShot");
            
            EditorGUILayout.LabelField("오디오 실행 방식", moduleStyle);
            // Loop와 OneShot을 수평으로 배치
            EditorGUILayout.BeginHorizontal();
                
            // Loop와 OneShot의 상호 배타적 토글 설정
            // 토글 그룹 스타일로 변경
            int selectedOption = loopProp.boolValue ? 0 : (oneShotProp.boolValue ? 1 : 0);
            int newSelectedOption = GUILayout.Toolbar(selectedOption, new string[] { "Loop", "One Shot" });

            EditorGUILayout.EndHorizontal();

            // 선택된 옵션에 따라 프로퍼티 값 업데이트
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
    
            // 배경 색상 초기화
            GUI.backgroundColor = Color.white; 
        }
    }
}

#endif