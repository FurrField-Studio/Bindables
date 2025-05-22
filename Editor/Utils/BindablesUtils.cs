using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Bindables.Utils
{
    public static class BindablesUtils
    {
        public static GameObject GetGameObject(this SerializedProperty property)
        {
            return (property.serializedObject.targetObject as Component).gameObject;
        }
        
        public static GameObject FindGameObjectByName(this GameObject parent, string name)
        {
            if (name == parent.name)
            {
                return parent;
            }
            else
            {
                return parent.GetChildGameObjectByName(name)?.gameObject;
            }
        }
        
        public static GameObject GetChildGameObjectByName(this GameObject fromGameObject, string name)
        {
            var kid = fromGameObject.GetAllChildGameObjects().FirstOrDefault(k => k.gameObject.name == name);
            if (kid == null) return null;
            return kid.gameObject;
        }

        public static List<GameObject> GetAllChildGameObjects(this GameObject gameObject)
        {
            Transform[] childs = gameObject.GetComponentsInChildren<Transform>();
            List<GameObject> childGos = childs.Select(t => t.gameObject).ToList();
            childGos.Remove(gameObject);
            return childGos;
        }

        #region Arrays

        public static bool CreateComponentCollection<T>(FieldInfo fieldInfo, SerializedProperty property, T[] components, Type elementType, string filterMethod = null)
        {
            if (IsGenericCollection(fieldInfo))
            {
                ICollection instance = (ICollection) Activator.CreateInstance(fieldInfo.FieldType);
                
                MethodInfo addMethod = fieldInfo.FieldType.GetMethod("Add");
                
                for (int i = 0; i < components.Length; i++)
                {
                    addMethod.Invoke(instance, new object[]{components.GetValue(i)});
                }
                
                if (!string.IsNullOrEmpty(filterMethod))
                {
                    instance = FilterArray(fieldInfo, filterMethod, property, instance);
                }
                
                SetArray(fieldInfo, property, instance);
                        
                return true;
            }
            else
            {
                ICollection genericArray = CreateGenericArray(components, elementType);

                if (!string.IsNullOrEmpty(filterMethod))
                {
                    genericArray = FilterArray(fieldInfo, filterMethod, property, genericArray);
                }
                
                SetArray(fieldInfo, property, genericArray);
                        
                return true;
            }
        }

        public static Type GetArrayElementType(this FieldInfo fieldInfo)
        {
            Type elementType;
            if (fieldInfo.FieldType.GenericTypeArguments.Length != 0)
            {
                elementType = fieldInfo.FieldType.GenericTypeArguments[0];
            }
            else
            {
                elementType = fieldInfo.FieldType.GetElementType();
            }

            return elementType;
        }
        
        public static Array CreateGenericArray(Array array, Type elementType)
        {
            Array genericArray = Array.CreateInstance(elementType, array.Length);

            for (int i = 0; i < array.Length; i++)
            {
                genericArray.SetValue(array.GetValue(i), i);
            }

            return genericArray;
        }
        
        public static void SetArray(FieldInfo fieldInfo, SerializedProperty property, object array)
        {
            fieldInfo.SetValue(property.serializedObject.targetObject, array);
            property.serializedObject.Update();
        }

        public static ICollection FilterArray(FieldInfo fieldInfo, string filterMethod, SerializedProperty property, ICollection genericArray)
        {
            return (ICollection) fieldInfo.DeclaringType.GetMethod(filterMethod, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .Invoke(property.serializedObject.targetObject, new object[] { genericArray });
        }

        #endregion
        
        public static GameObject FindGameObject(GameObject parent, BindAttribute bindAttribute, FieldInfo fieldInfo)
        {
            if (!GetNameFromAttribute(bindAttribute, fieldInfo, out string name))
            {
                
                StripName(ref name, BindablesSettings.instance.gameObjectStripNames);
            }
            
            return parent.FindGameObjectByName(name);
        }
        
        public static Transform FindTransform(GameObject parent, BindAttribute bindAttribute, FieldInfo fieldInfo)
        {
            if (!GetNameFromAttribute(bindAttribute, fieldInfo, out string name))
            {
                StripName(ref name, BindablesSettings.instance.transformStripNames);
            }

            return parent.FindGameObjectByName(name)?.transform;
        }
        
        public static Component FindComponent(GameObject parent, BindAttribute bindAttribute, FieldInfo fieldInfo)
        {
            if (!GetNameFromAttribute(bindAttribute, fieldInfo, out string name))
            {
                StripName(ref name, BindablesSettings.instance.componentStripNames);
            }
            
            return parent.FindGameObjectByName(name)?.GetComponent(fieldInfo.FieldType);
        }

        public static Component[] FindComponents(GameObject parent, BindAttribute bindAttribute, FieldInfo fieldInfo)
        {
            if (!GetNameFromAttribute(bindAttribute, fieldInfo, out string name))
            {
                StripName(ref name, BindablesSettings.instance.componentStripNames);
            }
            
            return parent.FindGameObjectByName(name)?.GetComponents(fieldInfo.FieldType);
        }

        public static ScriptableObject FindScriptableObject(BindAttribute bindAttribute, FieldInfo fieldInfo)
        {
            if (!GetNameFromAttribute(bindAttribute, fieldInfo, out string name))
            {
                StripName(ref name, BindablesSettings.instance.scriptableObjectStripNames);
            }
            
            return AssetDatabaseUtils.FindAssetTypeByName(fieldInfo.FieldType, name);
        }
        
        public static ScriptableObject[] FindScriptableObjects(FieldInfo fieldInfo)
        {   
            return AssetDatabaseUtils.FindAllAssetsOfType(fieldInfo.FieldType);
        }
        
        public static ScriptableObject[] FindScriptableObjectsInPath(FieldInfo fieldInfo, string path)
        {
            return AssetDatabaseUtils.FindAllAssetsOfTypeInPath(fieldInfo.FieldType, path);
        }

        public static bool GetNameFromAttribute(BindAttribute bindAttribute, FieldInfo fieldInfo, out string name)
        {
            if (string.IsNullOrEmpty(bindAttribute.Name))
            {
                name = ObjectNames.NicifyVariableName(fieldInfo.Name).Replace(" ", "");
                return false;
            }
            else
            {
                name = bindAttribute.Name;
                return true;
            }
        }

        public static bool IsGenericCollection(FieldInfo field)
        {
            Type fieldType = field.FieldType;
            
            if (!fieldType.IsGenericType)
                return false;
            
            return fieldType.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        private static void StripName(ref string name, string[] strips)
        {
            foreach (string strip in strips)
            {
                if (!name.EndsWith(strip, StringComparison.Ordinal)) continue;
                
                name = name.Substring(0, name.Length - strip.Length);
                break;
            }
        }
    }
}