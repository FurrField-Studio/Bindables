using System;
using System.Collections;
using System.Linq;
using Bindables.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Bindables
{
    [CustomPropertyDrawer(typeof(BindSelfAttribute))]
    public class BindSelfPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if ((property.objectReferenceValue == null || property.isArray) && FindValue(property))
            {
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        
            EditorGUI.PropertyField(position, property, label, true);
        }

        private bool FindValue(SerializedProperty property)
        {
            GameObject parent = property.GetGameObject();
            
            if (fieldInfo.FieldType.IsSubclassOf(typeof(Component)))
            {
                Object result = parent.GetComponent(fieldInfo.FieldType);
                
                if (property.objectReferenceValue == null && result != null)
                {
                    property.objectReferenceValue = result;

                    return true;
                }
            }
            else if (typeof(ICollection).IsAssignableFrom(fieldInfo.FieldType))
            {
                Type elementType = fieldInfo.GetArrayElementType();
                
                var components = parent.GetComponents(elementType);
                
                if(components.Length == 0) return false;
                
                BindablesUtils.CreateComponentCollection<Component>(fieldInfo, property, components, elementType);

                return true;
            }

            return false;
        }
    }
}