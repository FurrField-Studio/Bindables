using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bindables
{
    [FilePath("ProjectSettings/BindablesSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class BindablesSettings : ScriptableSingleton<BindablesSettings>
    {
        public string[] gameObjectStripNames;
        public string[] transformStripNames;
        public string[] componentStripNames;
        public string[] scriptableObjectStripNames;
        
        internal static BindablesSettings GetOrCreateSettings()
        {
            return instance == null ? CreateInstance<BindablesSettings>() : instance;
        }
        
        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        public void Save()
        {
            base.Save(true);
        }
    }
    
    static class MyPluginSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            var provider = new SettingsProvider(
                path: "Project/Bindables",
                scopes: SettingsScope.Project)
            {
                label = "Bindables",
                guiHandler = DrawGUI,
                keywords = new[] { "bindables", "bind" }
            };
            return provider;
        }

        static void DrawGUI(string searchContext)
        {
            var so = BindablesSettings.GetSerializedSettings();
            so.Update();

            EditorGUILayout.LabelField("Bindables", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(
                so.FindProperty(nameof(BindablesSettings.gameObjectStripNames)),
                new GUIContent("Game Object names to strip:")
            );
            EditorGUILayout.PropertyField(
                so.FindProperty(nameof(BindablesSettings.transformStripNames)),
                new GUIContent("Transform names to strip:")
            );
            EditorGUILayout.PropertyField(
                so.FindProperty(nameof(BindablesSettings.componentStripNames)),
                new GUIContent("Component names to strip:")
            );
            EditorGUILayout.PropertyField(
                so.FindProperty(nameof(BindablesSettings.scriptableObjectStripNames)),
                new GUIContent("Scriptable Object names to strip:")
            );

            so.ApplyModifiedProperties();
            BindablesSettings.GetOrCreateSettings().Save();
        }
    }
}