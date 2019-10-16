using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AdvancedSettingsWindow : EditorWindow
{
    public static AdvancedSettingsWindow _colorWindow;
    public static bool opened;
    static ImplosiveExplosiveEditor owner;

    public static void OpenWindow(ImplosiveExplosiveEditor newOwner)
    {
        owner = newOwner;
        opened = true;

        _colorWindow = GetWindow<AdvancedSettingsWindow>();

        _colorWindow.minSize = new Vector2(200, 250);
        _colorWindow.maxSize = new Vector2(500, 600);

        StylePresets.CreateStyles();
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, 1000, 1000), (Texture)Resources.Load("Fondo"));

        EditorGUILayout.BeginHorizontal();
        GUILayout.Button((Texture)Resources.Load("Logotipo"), new GUIStyle(), GUILayout.MaxHeight(40), GUILayout.MinHeight(40));
        EditorGUILayout.LabelField("Advanced Settings", StylePresets.SUBTITLE);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("Inner Radius", StylePresets.COMMON);
        owner.scriptTarget.innerRadius = EditorGUILayout.Slider(owner.scriptTarget.innerRadius,0,100);
        EditorGUILayout.LabelField("Outer Radius", StylePresets.COMMON);
        owner.scriptTarget.outerRadius = EditorGUILayout.Slider(owner.scriptTarget.outerRadius,0,100);

        EditorGUILayout.LabelField("Selectable Layers", StylePresets.COMMON);
        owner.scriptTarget.actualRadiusLayer = EditorGUILayout.LayerField(owner.scriptTarget.actualRadiusLayer);
    }
}
