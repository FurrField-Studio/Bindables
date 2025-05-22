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
    [CustomPropertyDrawer(typeof(BindAttribute))]
    public class BindPropertyDrawer : PropertyDrawer
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

            if (!typeof(ICollection).IsAssignableFrom(fieldInfo.FieldType))
            {
                Object result = GetValueForBind(parent);

                if (property.objectReferenceValue == null && result != null)
                {
                    property.objectReferenceValue = result;
                    
                    return true;
                }
            }

            return false;
        }
        
        // There was once a support for arrays but I think its too wide scope for a single attribute
        // also i like to keep this one closer to BindWidget behavior from Unreal
        //
        // Type elementType = fieldInfo.GetArrayElementType();
        //
        // if (elementType.IsSubclassOf(typeof(Component)))
        // {
        //     var components = parent.GetComponentsInChildren(elementType);
        //     
        //     BindablesUtils.CreateComponentCollection<Component>(fieldInfo, property, components, elementType);
        // }
        // else if (elementType == typeof(GameObject))
        // {
        //     var gameObjects = parent.GetAllChildGameObjects();
        //     
        //     fieldInfo.SetValue(property.serializedObject.targetObject, gameObjects.ToArray());
        // }
        // else if (elementType.IsSubclassOf(typeof(ScriptableObject)))
        // {
        //     var scriptableObjects = AssetDatabaseUtils.FindAllAssetsOfType(elementType);
        //     
        //     BindablesUtils.CreateComponentCollection<ScriptableObject>(fieldInfo, property, scriptableObjects, elementType);
        // }

        private Object GetValueForBind(GameObject parent)
        {
            if (fieldInfo.FieldType == typeof(Transform))
            {
                return BindablesUtils.FindTransform(parent, (BindAttribute)attribute, fieldInfo);
            }
            else if (fieldInfo.FieldType.IsSubclassOf(typeof(Component)))
            {
                return BindablesUtils.FindComponent(parent, (BindAttribute)attribute, fieldInfo);
            }
            else if (fieldInfo.FieldType == typeof(GameObject))
            {
                return BindablesUtils.FindGameObject(parent, (BindAttribute)attribute, fieldInfo);
            }
            else if (fieldInfo.FieldType.IsSubclassOf(typeof(ScriptableObject)))
            {
                return BindablesUtils.FindScriptableObject((BindAttribute)attribute, fieldInfo);
            }

            return null;
        }
    }
}