using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Bindables.Utils;
using UnityEditor;
using UnityEngine;

namespace Bindables
{
    [CustomPropertyDrawer(typeof(BindScriptableObjectAttribute))]
    public class BindScriptableObjectPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null && FindValue(property))
            {
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
            
            EditorGUI.PropertyField(position, property, label);
        }

        private bool FindValue(SerializedProperty property)
        {
            if (typeof(ICollection).IsAssignableFrom(fieldInfo.FieldType))
            {
                Type elementType = fieldInfo.GetArrayElementType();
                
                if (elementType.IsSubclassOf(typeof(ScriptableObject)))
                {
                    BindScriptableObjectAttribute bindComponent = (BindScriptableObjectAttribute)attribute;

                    ScriptableObject[] components = null;

                    if (!string.IsNullOrEmpty(bindComponent.Path))
                    {
                        components = AssetDatabaseUtils.FindAllAssetsOfTypeInPath(elementType, bindComponent.Path);
                    }
                    else
                    {
                        components = AssetDatabaseUtils.FindAllAssetsOfType(elementType);
                    }

                    if (!string.IsNullOrEmpty(bindComponent.Prefix))
                    {
                        components = components.Where(comp => comp.name.StartsWith(bindComponent.Prefix)).ToArray();
                    }

                    if(components.Length == 0) return false;
                    
                    return BindablesUtils.CreateComponentCollection<ScriptableObject>(fieldInfo, property, components, elementType, bindComponent.FilterMethod);
                }
            }
            else
            {
                if (!fieldInfo.FieldType.IsSubclassOf(typeof(ScriptableObject))) return false;

                var component = BindablesUtils.FindScriptableObject((BindAttribute) attribute, fieldInfo);
                
                if (property.objectReferenceValue == null && component != null)
                {
                    property.objectReferenceValue = component;
                    
                    return true;
                }
            }
            
            return false;
        }
    }
}