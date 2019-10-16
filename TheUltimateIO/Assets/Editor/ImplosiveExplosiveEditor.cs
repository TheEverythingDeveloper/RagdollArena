using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ImplosiveExplosiveScript))] [CanEditMultipleObjects]
public class ImplosiveExplosiveEditor : Editor
{
    public ImplosiveExplosiveScript scriptTarget;

    private float movement;
    private bool activated;
    private AdvancedSettingsWindow _myAdvancedSettingsWIndow;
    private ColorSelectorWindow _myColorWindow;
    private SaveLoadSettings _mySaveLoadWindow;

    private enum TypeOfSpeed
    {
        Implosive,
        Explosive,
        Both
    }

    private TypeOfSpeed _typeOfSpeed;

    Rect mainToolbarArea = new Rect(20, 20, 300, 250);

    private void OnEnable()
    {
        StylePresets.CreateStyles();
        scriptTarget = (ImplosiveExplosiveScript)target;
    }

    [MenuItem("/IE/Create IE Handler")]
    public static void CreateImplosiveExplosive()
    {
        GameObject prefab = new GameObject();
        prefab.AddComponent<ImplosiveExplosiveScript>();
        prefab.AddComponent<MeshFilter>();
        prefab.AddComponent<MeshRenderer>();
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        prefab.GetComponent<MeshFilter>().sharedMesh = go.GetComponent<MeshFilter>().sharedMesh;
        DestroyImmediate(go);
        GameObject newPrefab = new GameObject();
        GameObject newPrefab2 = new GameObject();
        newPrefab.transform.parent = prefab.transform;
        newPrefab2.transform.parent = prefab.transform;
        newPrefab.name = "Outer Transform";
        newPrefab2.name = "Inner Transform";
        newPrefab.transform.position = new Vector3(10, 0, 0);
        newPrefab2.transform.position = new Vector3(5, 0, 0);
        prefab.name = "IE Handler";
        prefab.GetComponent<ImplosiveExplosiveScript>().innerTransform = newPrefab2.transform;
        prefab.GetComponent<ImplosiveExplosiveScript>().outerTransform = newPrefab.transform;
        prefab.GetComponent<ImplosiveExplosiveScript>().nonSelectedObjectMaterial = (Material)Resources.Load("UnselectedMaterial");
        prefab.GetComponent<ImplosiveExplosiveScript>().selectedObjectMaterial = (Material)Resources.Load("IENearMaterial");
        prefab.GetComponent<Renderer>().sharedMaterial = prefab.GetComponent<ImplosiveExplosiveScript>().nonSelectedObjectMaterial;
        prefab.GetComponent<ImplosiveExplosiveScript>().farColor = Color.green;
        prefab.GetComponent<ImplosiveExplosiveScript>().nearColor = Color.red;
    }

    private void OnSceneGUI()
    {
        UpdateCall();
    }

    public void UpdateCall()
    {
        HandlesPanel();
        Handles.BeginGUI();
        var addValue = 40 / Vector3.Distance(Camera.current.transform.position, scriptTarget.transform.position);

        DrawButton(scriptTarget.transform.position + addValue * Vector3.up,ButtonType.Add, (Texture)Resources.Load("RedBanner"));
        DrawButton(scriptTarget.outerTransform.position + addValue * Vector3.up, ButtonType.Add, (Texture)Resources.Load("YellowBanner"));
        DrawButton(scriptTarget.innerTransform.position + addValue * Vector3.up, ButtonType.Add, (Texture)Resources.Load("OrangeBanner"));
        GUIPanel();

        Handles.EndGUI();

        //Actualizar el update del script real para que vaya cambiando de color y empujando todo
        scriptTarget.EditorUpdate();
    }

    private void DrawButton(Vector3 position, ButtonType typ, Texture loadTexture)
    {
        var p = Camera.current.WorldToScreenPoint(position);
        var size = 1500 / Vector3.Distance(Camera.current.transform.position, position);
        var r = new Rect(p.x - size / 2, Screen.height - p.y - size, size, size *2.3f);
        //var r = new Rect(p.x, p.y, size, size * 4);

        GUI.DrawTexture(r, loadTexture);
    }

    public enum ButtonType
    {
        Add,
        ChangeType
    }

    private void HandlesPanel()
    {
        scriptTarget.outerTransform.position = Handles.PositionHandle(scriptTarget.outerTransform.position, Quaternion.identity);
        scriptTarget.innerTransform.position = Handles.PositionHandle(scriptTarget.innerTransform.position, Quaternion.identity);
    }

    private void GUIPanel()
    {
        GUI.DrawTexture(mainToolbarArea, (Texture)Resources.Load("Fondo2"));
        GUILayout.BeginArea(mainToolbarArea);
        GUILayout.Button((Texture)Resources.Load("LogoCompleto"), new GUIStyle(), GUILayout.Width(300), GUILayout.Height(50));
        var rec = EditorGUILayout.BeginVertical();
        //GUI.Box(rec, GUIContent.none);
        GUI.color = new Color32(255, 102, 102, 255);

        GUI.color = Color.white;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Speed", StylePresets.SUBTITLE);
        _typeOfSpeed = (TypeOfSpeed)EditorGUILayout.EnumPopup(_typeOfSpeed);
        GUI.color = scriptTarget.implosiveSpeed >= 0 ? Color.green : Color.red;
        GUILayout.Label(scriptTarget.implosiveSpeed.ToString("F2"), StylePresets.SUBTITLE);
        GUILayout.EndHorizontal();
        switch (_typeOfSpeed)
        {
            case TypeOfSpeed.Implosive:
                if (scriptTarget.implosiveSpeed > 0)
                    scriptTarget.implosiveSpeed = -1;
                scriptTarget.implosiveSpeed = GUILayout.HorizontalSlider(scriptTarget.implosiveSpeed, 0, -100);
                break;
            case TypeOfSpeed.Explosive:
                if(scriptTarget.implosiveSpeed < 0)
                    scriptTarget.implosiveSpeed = 1;
                scriptTarget.implosiveSpeed = GUILayout.HorizontalSlider(scriptTarget.implosiveSpeed, 0, 100);
                break;
            case TypeOfSpeed.Both:
                scriptTarget.implosiveSpeed = GUILayout.HorizontalSlider(scriptTarget.implosiveSpeed, -100, 100);
                break;
        }

        GUI.color = Color.white;

        GUILayout.Label("Noise Amount", StylePresets.COMMON);
        scriptTarget.randomNoise = GUILayout.HorizontalSlider(scriptTarget.randomNoise, 0,30);
        scriptTarget.active = GUILayout.Toggle(scriptTarget.active, scriptTarget.active ? "Activate" : "Deactivate");
        scriptTarget.distanceGradient = GUILayout.Toggle(scriptTarget.distanceGradient, 
            scriptTarget.distanceGradient ? "Activate Distance Gradient" : "Deactivate Distance Gradient");
        GUILayout.FlexibleSpace();
        float buttonSize = 50;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Advanced", StylePresets.COMMON);
        if (GUILayout.Button((Texture)Resources.Load("settings"), GUILayout.MaxHeight(buttonSize), GUILayout.MaxWidth(buttonSize)))
        {
            if (_myAdvancedSettingsWIndow == null)
                _myAdvancedSettingsWIndow = new AdvancedSettingsWindow();
            AdvancedSettingsWindow.OpenWindow(this);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Aesthetics", StylePresets.COMMON);
        if(GUILayout.Button((Texture)Resources.Load("Color"), GUILayout.MaxHeight(buttonSize), GUILayout.MaxWidth(buttonSize)))
        {
            if (_myColorWindow == null)
                _myColorWindow = new ColorSelectorWindow();
            ColorSelectorWindow.OpenWindow(this);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Save/Load", StylePresets.COMMON);
        if(GUILayout.Button((Texture)Resources.Load("Search"), GUILayout.MaxHeight(buttonSize), GUILayout.MaxWidth(buttonSize)))
        {
            if (_mySaveLoadWindow == null)
                _mySaveLoadWindow = new SaveLoadSettings();
            SaveLoadSettings.OpenWindow(this);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;

        GUILayout.EndArea();
    }
}

