using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ColorSelectorWindow : EditorWindow
{
    public static ColorSelectorWindow _colorWindow;
    public static bool opened;
    static ImplosiveExplosiveEditor owner;

    public static void OpenWindow(ImplosiveExplosiveEditor newOwner)
    {
        owner = newOwner;
        opened = true;

        _colorWindow = GetWindow<ColorSelectorWindow>();

        _colorWindow.minSize = new Vector2(200, 250);
        _colorWindow.maxSize = new Vector2(500, 600);

        StylePresets.CreateStyles();
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, 1000, 1000), (Texture)Resources.Load("Fondo3"));

        EditorGUILayout.BeginHorizontal();
        GUILayout.Button((Texture)Resources.Load("Logotipo"), new GUIStyle(), GUILayout.MaxHeight(40), GUILayout.MinHeight(40));
        EditorGUILayout.LabelField("Aesthetics", StylePresets.SUBTITLE);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("Near Color", StylePresets.COMMON);
        owner.scriptTarget.nearColor = EditorGUILayout.ColorField(owner.scriptTarget.nearColor);
        EditorGUILayout.LabelField("Far Color", StylePresets.COMMON);
        owner.scriptTarget.farColor = EditorGUILayout.ColorField(owner.scriptTarget.farColor);
    }
}

