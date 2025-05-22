using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bindables.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Bindables
{
    [CustomPropertyDrawer(typeof(BindComponentAttribute))]
    public class BindComponentPropertyDrawer : PropertyDrawer
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
            GameObject parent = property.GetGameObject();

            if (typeof(ICollection).IsAssignableFrom(fieldInfo.FieldType))
            {
                Type elementType = fieldInfo.GetArrayElementType();

                if (elementType.IsSubclassOf(typeof(Component)))
                {
                    BindComponentAttribute bindComponent = (BindComponentAttribute)attribute;
                    
                    var components = parent.GetComponentsInChildren(elementType);
                    
                    BindablesUtils.CreateComponentCollection<Component>(fieldInfo, property, components, elementType, bindComponent.FilterMethod);

                    return true;
                }
            }
            else
            {
                if (!fieldInfo.FieldType.IsSubclassOf(typeof(Component))) return false;

                var component = BindablesUtils.FindComponent(parent, (BindAttribute) attribute, fieldInfo);

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