using NoManual.Inventory;
using UnityEditor;
using UnityEngine;

namespace NoManual.Editors
{
    [CustomEditor(typeof(ItemScriptable)), CanEditMultipleObjects]
    public class ItemScriptableEditor : Editor
    {
        static float Spacing => EditorGUIUtility.standardVerticalSpacing * 2;
        private Vector2 _scrollPosition;
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            var id = serializedObject.FindProperty("itemId");
            var title = serializedObject.FindProperty("itemTitle");
            var icon = serializedObject.FindProperty("itemIcon");
            var description = serializedObject.FindProperty("description");
            var itemUseType = serializedObject.FindProperty("inventoryItemUseType");
            var dropPrefab = serializedObject.FindProperty("dropPrefab");

            var itemToggles = serializedObject.FindProperty("itemToggle");
            var itemSettings = serializedObject.FindProperty("itemSettings");
            var itemCombineSettings = serializedObject.FindProperty("combineSettings");
            var itemSounds = serializedObject.FindProperty("itemSounds");
            
            // 이미지 선택
            Rect iconRect = GUILayoutUtility.GetRect(120, 100);
            iconRect.xMax = 120;
            icon.objectReferenceValue = EditorGUI.ObjectField(iconRect, icon.objectReferenceValue, typeof(Sprite), false);
            
            // id, 이름, 설명 입력
            Rect titleRect = GUILayoutUtility.GetRect(1, 20);
            titleRect.y = 0f;
            titleRect.xMin = iconRect.xMax + Spacing;
            
            // 라벨과 PropertyField 간의 간격 조절을 위한 변수
            float labelWidth = 80f; // 라벨의 너비
            float fieldWidth = EditorGUIUtility.currentViewWidth - 220f;//titleRect.width - labelWidth - 50f; // PropertyField의 너비

            float offset = 25f;
            
            EditorGUI.BeginChangeCheck();
            {
                // id
                titleRect.y = 5f;
                // EditorGUIUtility.labelWidth = labelWidth; // 라벨의 너비 설정
                EditorGUI.LabelField(new Rect(titleRect.x, titleRect.y, labelWidth, titleRect.height), new GUIContent("Item ID"));
                EditorGUI.PropertyField(new Rect(titleRect.x + labelWidth, titleRect.y, fieldWidth - 100f, titleRect.height), id, new GUIContent());
                //  EditorGUIUtility.labelWidth = 0; // 라벨의 너비 리셋
                
                // 이름
                titleRect.y += offset;
                EditorGUI.LabelField(new Rect(titleRect.x, titleRect.y, labelWidth, titleRect.height), new GUIContent("Item Name"));
                EditorGUI.PropertyField(new Rect(titleRect.x + labelWidth, titleRect.y, fieldWidth, titleRect.height), title, new GUIContent());

                // 설명
                titleRect.y += offset;
                Rect descriptionRect = titleRect;
                descriptionRect.height = 50f;
                EditorGUI.LabelField(new Rect(titleRect.x, titleRect.y, labelWidth, titleRect.height), new GUIContent("Description"));
                EditorGUI.PropertyField(new Rect(descriptionRect.x + labelWidth, descriptionRect.y, fieldWidth, descriptionRect.height), description, new GUIContent());
                
                // 사용 기준, 버리는 프리팹 설정
               // EditorGUILayout.Space(-20 + EditorGUIUtility.standardVerticalSpacing);
                EditorGUILayout.PropertyField(itemUseType);
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(dropPrefab);
                EditorGUILayout.Space(5);

              //  bool showToggle = false;
              //  showToggle = EditorGUILayout.Foldout(showToggle, "Item Toggle");
              // 아이템 속성 값 설정
              DrawPropertyWithHeader(itemToggles, "Item Toggles");
              DrawPropertyWithHeader(itemSettings, "Item Settings");
              DrawPropertyWithHeader(itemCombineSettings, "Combine Settings");
              DrawPropertyWithHeader(itemSounds, "Item Sounds");
                

            }

            /*
            Rect itemViewRect = new Rect(5f, 5f, EditorGUIUtility.currentViewWidth - 10f, 300f);
            EditorUtils.DrawHeaderWithBorder("Item View", 20, ref itemViewRect, true);

            Rect itemViewArea = itemViewRect;
            itemViewArea.y += Spacing;
            itemViewArea.yMax -= Spacing;
            itemViewArea.xMin += Spacing;
            itemViewArea.xMax -= Spacing;

            GUILayout.BeginArea(itemViewArea);
            {
   
       
            // _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
             // 아이콘 그리기   
                  Rect iconRect = GUILayoutUtility.GetRect(100, 100);
                iconRect.xMax = 100;
                icon.objectReferenceValue = EditorGUI.ObjectField(iconRect, icon.objectReferenceValue, typeof(Sprite), false);
       
       EditorGUI.BeginChangeCheck();
       {

    
       }
       if(EditorGUI.EndChangeCheck())
       {
           
       }
    
     
      // 타이틀 그리기
      Rect titleRect = GUILayoutUtility.GetRect(1, 20);
      titleRect.y = 0f;
      titleRect.xMin = iconRect.xMax + Spacing;
  
      // 라벨과 PropertyField 간의 간격 조절을 위한 변수
      float labelWidth = 80f; // 라벨의 너비
      float fieldWidth = titleRect.width - labelWidth; // PropertyField의 너비
      
      EditorGUI.BeginChangeCheck();
      {
          EditorGUIUtility.labelWidth = labelWidth; // 라벨의 너비 설정

          EditorGUI.LabelField(new Rect(titleRect.x, titleRect.y, labelWidth, titleRect.height), new GUIContent("Item Name"));
          EditorGUI.PropertyField(new Rect(titleRect.x + labelWidth, titleRect.y, fieldWidth - 15f, titleRect.height), title, new GUIContent());

          EditorGUIUtility.labelWidth = 0; // 라벨의 너비 리셋
      }
      if (EditorGUI.EndChangeCheck())
      {
          //treeElements[selectedIndex].displayName = item.p_Title.stringValue;          
      }
      
      EditorGUILayout.EndScrollView();
         
            }
            GUILayout.EndArea();
            */
            

            /*
            DrawHeader("General Information");
            EditorGUI.indentLevel++;
            itemScriptable.itemId = EditorGUILayout.IntField("ID", itemScriptable.itemId);
            itemScriptable.itemTitleName = EditorGUILayout.TextField("Name", itemScriptable.itemTitleName);
            itemScriptable.itemIcon = (Sprite)EditorGUILayout.ObjectField("Image", itemScriptable.itemIcon, typeof(Sprite), false);
            itemScriptable.description = EditorGUILayout.TextArea("Description", itemScriptable.description);
            itemScriptable.itemUseType = (ItemUseType)EditorGUILayout.EnumPopup("Use Type", itemScriptable.itemUseType);
            itemScriptable.dropPrefab = (GameObject)EditorGUILayout.ObjectField("Drop Prefab", itemScriptable.dropPrefab, typeof(GameObject), false);
            EditorGUI.indentLevel--;

            DrawHeader("Toggle Settings");
            EditorGUI.indentLevel++;
            itemScriptable.itemToggle.UsageOp_Read = EditorGUILayout.Toggle("Readable", itemScriptable.itemToggle.UsageOp_Read);
            itemScriptable.itemToggle.UsageOp_Equip = EditorGUILayout.Toggle("Equipable", itemScriptable.itemToggle.UsageOp_Equip);
            itemScriptable.itemToggle.UsageOp_Use = EditorGUILayout.Toggle("Usable", itemScriptable.itemToggle.UsageOp_Use);
            itemScriptable.itemToggle.Usage_Drop = EditorGUILayout.Toggle("Droppable", itemScriptable.itemToggle.Usage_Drop);
            itemScriptable.itemToggle.Usage_Combine = EditorGUILayout.Toggle("Combinable", itemScriptable.itemToggle.Usage_Combine);
            itemScriptable.itemToggle.Usage_Eat = EditorGUILayout.Toggle("Eatable", itemScriptable.itemToggle.Usage_Eat);
            EditorGUI.indentLevel--;

            DrawHeader("Item Settings");
            EditorGUI.indentLevel++;
            itemScriptable.itemSettings.maxStackAmount = EditorGUILayout.IntField("Max Stack Amount", itemScriptable.itemSettings.maxStackAmount);
            itemScriptable.itemSettings.mentalityAmount = EditorGUILayout.Slider("Mentality Amount", itemScriptable.itemSettings.mentalityAmount, 0f, 1f);
            itemScriptable.itemSettings.staminaAmount = EditorGUILayout.Slider("Stamina Amount", itemScriptable.itemSettings.staminaAmount, 0f, 1f);
            EditorGUI.indentLevel--;

            DrawHeader("Item Sounds");
            EditorGUI.indentLevel++;
            itemScriptable.itemSounds.useSound = (AudioClip)EditorGUILayout.ObjectField("Use Sound", itemScriptable.itemSounds.useSound, typeof(AudioClip), false);
            itemScriptable.itemSounds.useVolume = EditorGUILayout.Slider("Use Volume", itemScriptable.itemSounds.useVolume, 0f, 1f);
            itemScriptable.itemSounds.combineSound = (AudioClip)EditorGUILayout.ObjectField("Combine Sound", itemScriptable.itemSounds.combineSound, typeof(AudioClip), false);
            itemScriptable.itemSounds.combineVolume = EditorGUILayout.Slider("Combine Volume", itemScriptable.itemSounds.combineVolume, 0f, 1f);
            EditorGUI.indentLevel--;

            DrawHeader("Combine Settings");
            EditorGUI.indentLevel++;
            SerializedProperty combineSettings = serializedObject.FindProperty("combineSettings");
            EditorGUILayout.PropertyField(combineSettings, true);
            EditorGUI.indentLevel--;

*/
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPropertyWithHeader(SerializedProperty property, string label)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            if (property.isExpanded = EditorUtils.DrawFoldoutHeader(20f, label, property.isExpanded, true))
            {
                EditorGUILayout.Space(Spacing);
                EditorUtils.DrawRelativeProperties(property, 5f);
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            }
            EditorGUILayout.EndVertical();
        }
    }
}


