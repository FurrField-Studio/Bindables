#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class AssetDatabaseUtils
{
    public static ScriptableObject[] FindAllAssetsOfType(Type type)
    {
        string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", type));

        return guids.Select(guid => AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid))).ToArray();
    }
    
    public static ScriptableObject[] FindAllAssetsOfTypeInPath(Type type, string searchPath)
    {
        string[] guids = AssetDatabase.FindAssets($"t:{type}", new[] { searchPath });
        
        return guids.Select(guid => AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid))).ToArray();
    }
    
    public static ScriptableObject FindAssetTypeByName(Type type, string name)
    {
        string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", type));

        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            
            if (assetPath.EndsWith(name + ".asset"))
            {
                return AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
            }
        }

        return null;
    }
}
#endif