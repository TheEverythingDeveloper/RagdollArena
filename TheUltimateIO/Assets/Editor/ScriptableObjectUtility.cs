using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class ScriptableObjectUtility
{
    public static T CreateAsset<T>(T asset, string name) where T : ScriptableObject
    {
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/ImplosiveExplosive/" + name + "_IE.asset");
       
        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
        return asset;
    }
}
