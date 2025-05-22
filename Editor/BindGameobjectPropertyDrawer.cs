using System;
using System.Collections;
using Bindables.Utils;
using UnityEditor;
using UnityEngine;

namespace Bindables
{
    [CustomPropertyDrawer(typeof(BindGameobjectAttribute))]
    public class BindGameobjectPropertyDrawer : PropertyDrawer
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
                
                if (elementType == typeof(GameObject))
                {
                    BindGameobjectAttribute bindGameobject = (BindGameobjectAttribute)attribute;
                    
                    var gameobjects = parent.GetAllChildGameObjects().ToArray();
                    
                    BindablesUtils.CreateComponentCollection<GameObject>(fieldInfo, property, gameobjects, elementType, bindGameobject.FilterMethod);

                    return true;
                }
            }
            else
            {
                if (fieldInfo.FieldType != typeof(GameObject)) return false;
                
                var gameobject = BindablesUtils.FindGameObject(parent, (BindAttribute) attribute, fieldInfo);

                if (property.objectReferenceValue == null && gameobject != null)
                {
                    property.objectReferenceValue = gameobject;
                    return true;
                }
            }

            return false;
        }
    }
}