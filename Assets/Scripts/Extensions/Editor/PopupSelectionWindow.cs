using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

namespace Game
{
    public class PopupSelectionWindow : EditorWindow
    {
        private static PopupSelectionWindow ActiveWindow = null;
        public static void ShowWindow(Rect pos, in string[] options, Action<int> callback)
        {
            if (ActiveWindow != null)
            {
                ActiveWindow.Close();
            }
            ActiveWindow = ScriptableObject.CreateInstance<PopupSelectionWindow>();

            pos.Set(pos.x, pos.y + EditorGUIUtility.singleLineHeight, pos.width, 200f);
            ActiveWindow.position = pos;
            ActiveWindow.Options = options;
            ActiveWindow.Callback = callback;
            ActiveWindow.ShowPopup();
        }

        private string[] Options;
        private Action<int> Callback;

        private Vector2 Scroll;
        private string Filter;
        private bool Focused = false;

        private GUIStyle Button;

        private void OnGUI()
        {
            Button = new GUIStyle("Button")
            {
                alignment = TextAnchor.MiddleLeft
            };
            GUI.SetNextControlName("FilterField");
            Filter = EditorGUILayout.TextField(Filter, new GUIStyle("SearchTextField"));
            if (!Focused)
            {
                EditorGUI.FocusTextInControl("FilterField");
                Focused = true;
            }
            Scroll = EditorGUILayout.BeginScrollView(Scroll);
            for (int i = 0; i < Options.Length; i++)
            {
                if (!string.IsNullOrEmpty(Filter) && !Options[i].ToLower().Contains(Filter.ToLower())) continue;
                if (GUILayout.Button(Options[i], Button))
                {
                    Callback.Invoke(i);
                    CloseWindow();
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void OnLostFocus()
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            ActiveWindow = null;
            this.Close();
        }
    }
}
