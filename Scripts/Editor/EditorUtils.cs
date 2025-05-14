using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NoManual.Editors
{
    public static class EditorUtils 
    {
        public static Rect DrawHeaderWithBorder(string title, float height, ref Rect rect, bool rounded)
        {
            GUI.Box(rect, GUIContent.none, new GUIStyle(rounded ? "HelpBox" : "Tooltip"));
            rect.x += 1;
            rect.y += 1;
            rect.height -= 1;
            rect.width -= 2;

            var headerRect = rect;
            headerRect.height = height + EditorGUIUtility.standardVerticalSpacing;

            rect.y += headerRect.height;
            rect.height -= headerRect.height;

            EditorGUI.DrawRect(headerRect, new Color(0.1f, 0.1f, 0.1f, 0.4f));

            var labelRect = headerRect;
            labelRect.y += EditorGUIUtility.standardVerticalSpacing;
            labelRect.x += 2f;

            EditorGUI.LabelField(labelRect, title, EditorStyles.miniBoldLabel);

            return headerRect;
        }
        
        public static bool DrawFoldoutHeader(float height, string title, bool state, bool miniLabel = false, bool hoverable = true)
        {
            Rect rect = GUILayoutUtility.GetRect(1f, height + EditorGUIUtility.standardVerticalSpacing);
            state = DrawFoldoutHeader(rect, title, state, miniLabel, hoverable);
            return state;
        }
        
        public static bool DrawFoldoutHeader(Rect rect, string title, bool state, bool miniLabel, bool hoverable)
        {
            Color headerColor = new Color(0.1f, 0.1f, 0.1f, 0f);

            var foldoutRect = rect;
            foldoutRect.y += 4f;
            foldoutRect.x += 2f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            var labelRect = rect;
            labelRect.y += miniLabel ? EditorGUIUtility.standardVerticalSpacing : 0f;
            labelRect.xMin += 16f;
            labelRect.xMax -= 20f;

            // events
            var e = Event.current;
            if (rect.Contains(e.mousePosition))
            {
                if(hoverable) headerColor = new Color(0.6f, 0.6f, 0.6f, 0.2f);

                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    state = !state;
                    e.Use();
                }
            }

            // background
            EditorGUI.DrawRect(rect, headerColor);

            // foldout toggle
            state = GUI.Toggle(foldoutRect, state, GUIContent.none, EditorStyles.foldout);

            // title
            EditorGUI.LabelField(labelRect, new GUIContent(title), miniLabel ? EditorStyles.miniBoldLabel : EditorStyles.boldLabel);

            return state;
        }
        
        public static void DrawRelativeProperties(SerializedProperty root, float width)
        {
            var childrens = root.GetVisibleChildrens();

            foreach (var childProperty in childrens)
            {
                float height = EditorGUI.GetPropertyHeight(childProperty, true);

                Rect rect = GUILayoutUtility.GetRect(1f, height);
                rect.xMin += width;
                EditorGUI.PropertyField(rect, childProperty, true);
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            }
        }
        
        public static IEnumerable<SerializedProperty> GetVisibleChildrens(this SerializedProperty serializedProperty)
        {
            SerializedProperty currentProperty = serializedProperty.Copy();
            SerializedProperty nextSiblingProperty = serializedProperty.Copy();
            {
                nextSiblingProperty.NextVisible(false);
            }

            if (currentProperty.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                        break;

                    yield return currentProperty;
                }
                while (currentProperty.NextVisible(false));
            }
        }
        
        // 제목 스타일을 생성하는 함수
        public static GUIStyle CreateTitleStyle(int fontSize, FontStyle fontStyle, Color textColor, TextAnchor align)
        {
            return new GUIStyle(GUI.skin.label)
            {
                fontSize = fontSize,
                fontStyle = fontStyle,
                normal = { textColor = textColor },
                alignment = align // Center alignment
            };
        }

    }
}


