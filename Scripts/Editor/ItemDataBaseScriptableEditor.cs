using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using NoManual.Inventory;

namespace NoManual.Editors
{
    //[CustomEditor(typeof(ItemDataBaseScriptable)), CanEditMultipleObjects]
    public class ItemDataBaseScriptableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();
            
            ItemDataBaseScriptable database = (ItemDataBaseScriptable)target;

            if (GUILayout.Button("DB 데이터 열기"))
            {
                ItemDataBaseWindowEditor window = (ItemDataBaseWindowEditor)EditorWindow.GetWindow(typeof(ItemDataBaseWindowEditor));
                window.SetDatabase(database);
                window.Show();
            }
        }

        /*
        private List<bool> showItemDetails = new List<bool>();

        public override void OnInspectorGUI()
        {
            ItemDataBaseScriptable database = (ItemDataBaseScriptable)target;

            serializedObject.Update();

            EditorGUILayout.LabelField("Item Database", EditorStyles.boldLabel);

            for (int i = 0; i < database.itemDataBase.Count; i++)
            {
                ItemDataBaseScriptable.ItemScriptable2 item = database.itemDataBase[i];

                while (showItemDetails.Count <= i)
                {
                    showItemDetails.Add(false);
                }

                if (item != null)
                {
                    showItemDetails[i] = EditorGUILayout.Foldout(showItemDetails[i], "Item " + (i + 1));

                    if (showItemDetails[i])
                    {
                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.LabelField("Item ID: " + item.itemId.ToString());
                        EditorGUILayout.LabelField("Item Title: " + item.itemTitle);
                        EditorGUILayout.ObjectField("Item Icon", item.itemIcon, typeof(Sprite), false);
                        // 추가로 보고 싶은 필드들을 여기에 작성해주세요.
                        EditorGUILayout.EndVertical();
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
        */
    }
}

