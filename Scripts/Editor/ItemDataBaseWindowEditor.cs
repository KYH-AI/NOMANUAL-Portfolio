using NoManual.Inventory;
using UnityEngine;
using UnityEditor;

namespace NoManual.Editors
{
    public class ItemDataBaseWindowEditor : EditorWindow
    {
        private ItemDataBaseScriptable database;
        private int selected = -1;

        public void SetDatabase(ItemDataBaseScriptable database)
        {
            this.database = database;
        }

        void OnGUI()
        {
            for (int i = 0; i < database.itemDataBase.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                ItemScriptable item = database.itemDataBase[i];
                EditorGUILayout.LabelField("Item ID: " + item.itemId.ToString());
                EditorGUILayout.LabelField("Item Title: " + item.itemTitle);
                EditorGUILayout.ObjectField("Item Icon", item.itemIcon, typeof(Sprite), false);

                if (GUILayout.Button("Delete"))
                {
                    database.itemDataBase.RemoveAt(i);
                    return;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Item"))
            {
                database.itemDataBase.Add(new ItemScriptable());
            }
        }
    }
}

