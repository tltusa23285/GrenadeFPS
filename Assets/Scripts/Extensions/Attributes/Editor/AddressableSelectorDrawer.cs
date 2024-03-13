using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine.AddressableAssets;

namespace Game
{
    [CustomPropertyDrawer(typeof(AddressableSelectorAttribute))]
    public class AddressableSelectorDrawer : PropertyDrawer
    {
        private bool IsInit = false;
        private string[] Options;
        private HashSet<string> GroupFilter;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsInit) Init();
            DrawGui(position, property, label);
        }

        void Init()
        {
            IsInit = true;
            SetOptions();
        }

        void SetOptions()
        {
            List<string> options = new List<string>();

            GroupFilter = (attribute as AddressableSelectorAttribute).Groups;

            foreach (var item in AddressableAssetSettingsDefaultObject.Settings.groups)
            {
                if (GroupFilter != null && GroupFilter.Count > 0)
                {
                    if (!GroupFilter.Contains(item.Name)) continue;
                }
                foreach (var entry in item.entries)
                {
                    options.Add(entry.address);
                }
            } 
            Options = options.ToArray();
        }

        void DrawGui(Rect position, SerializedProperty property, GUIContent label)
        {
            int current = 0;
            for (int i = 0; i < Options.Length; i++)
            {
                if (Options[i] == property.stringValue)
                {
                    current = i;
                    break;
                }
            }

            current = EditorGUILayout.Popup(current, Options);
            property.stringValue = Options[current];
        }
    }
}
