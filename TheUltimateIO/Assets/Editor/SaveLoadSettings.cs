using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SaveLoadSettings : EditorWindow
{
    public static SaveLoadSettings _saveLoadWindow;
    public static bool opened;
    static ImplosiveExplosiveEditor owner;

    public List<Object> assetList = new List<Object>();
    private Object _focusedObject;

    public string name;

    public static void OpenWindow(ImplosiveExplosiveEditor newOwner)
    {
        owner = newOwner;
        opened = true;

        _saveLoadWindow = GetWindow<SaveLoadSettings>();

        _saveLoadWindow.minSize = new Vector2(200, 250);
        _saveLoadWindow.maxSize = new Vector2(500, 600);

        StylePresets.CreateStyles();
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, 1000, 1000), (Texture)Resources.Load("Fondo4"));

        EditorGUILayout.BeginHorizontal();
        GUILayout.Button((Texture)Resources.Load("Logotipo"), new GUIStyle(), GUILayout.MaxHeight(40), GUILayout.MinHeight(40));
        EditorGUILayout.LabelField("Save and Load", StylePresets.SUBTITLE);
        EditorGUILayout.EndHorizontal();
        name = EditorGUILayout.TextField(name);
        if (!AssetDatabase.IsValidFolder("Assets/ImplosiveExplosive"))
        {
            AssetDatabase.CreateFolder("Assets", "ImplosiveExplosive");
        }
        if (GUILayout.Button("Save"))
        {
            ScriptableObjectUtility.CreateAsset<ScriptableImplosiveExplosive>(owner.scriptTarget.SaveScriptableObject(), name);
        }
        EditorGUILayout.LabelField("Save", StylePresets.COMMON);
        if (PrefabFinder("_IE"))
        {
            owner.scriptTarget.LoadScriptableObject((ScriptableImplosiveExplosive)_focusedObject);
        }

    }

    private bool PrefabFinder(string searchName)
    {
        assetList.Clear();
        //AssetDatabase.FindAssets me retorna todos los paths de los assets que coinciden con el parámetro, en formato GUID
        string[] paths = AssetDatabase.FindAssets(searchName);

        for (int i = 0; i < paths.Length; i++)
        {
            //Convierto el GUID al formato "normal"
            paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);

            //cargo el asset en memoria
            var loaded = AssetDatabase.LoadAssetAtPath(paths[i], typeof(Object));

            assetList.Add(loaded);
        }

        for (int i = 0; i < assetList.Count; i++)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            string fulltext = assetList[i].name;
            string[] name = fulltext.Split(new[] { "_IE" }, System.StringSplitOptions.None);
            EditorGUILayout.LabelField(name[0], StylePresets.BOLDCOMMON);
            if (GUILayout.Button("Seleccionar"))
            {
                _focusedObject = assetList[i];
                return true;
            }
            if (GUILayout.Button("Borrar"))
            {
                _focusedObject = assetList[i];
                AssetDatabase.DeleteAsset(paths[i]);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            ScriptableImplosiveExplosive actualSelection = (ScriptableImplosiveExplosive)assetList[i];
            EditorGUILayout.LabelField(actualSelection.implosiveSpeed.ToString("F2"), StylePresets.COMMON, GUILayout.Width(60));
            EditorGUILayout.LabelField(actualSelection.randomNoise.ToString("F2"), StylePresets.COMMON, GUILayout.Width(60));
            EditorGUILayout.ColorField(actualSelection.nearColor, GUILayout.Width(60));
            EditorGUILayout.ColorField(actualSelection.farColor, GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        return false;
    }

}
