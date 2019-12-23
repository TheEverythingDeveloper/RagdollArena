using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public static class StylePresets
{
    public static GUIStyle TITLE = new GUIStyle();
    public static GUIStyle SUBTITLE = new GUIStyle();
    public static GUIStyle COMMON = new GUIStyle();
    public static GUIStyle BOLDCOMMON = new GUIStyle();

    public static void CreateStyles()
    {
        TITLE.fontSize = 25;
        TITLE.alignment = TextAnchor.MiddleCenter;
        TITLE.fontStyle = FontStyle.BoldAndItalic;
        TITLE.normal.textColor = Color.white;
        TITLE.alignment = TextAnchor.MiddleCenter;

        SUBTITLE.fontSize = 20;
        SUBTITLE.alignment = TextAnchor.MiddleCenter;
        SUBTITLE.fontStyle = FontStyle.BoldAndItalic;
        SUBTITLE.normal.textColor = Color.white;
        TITLE.alignment = TextAnchor.MiddleCenter;

        COMMON.fontSize = 15;
        COMMON.alignment = TextAnchor.MiddleCenter;
        COMMON.normal.textColor = Color.white;

        BOLDCOMMON.fontSize = 15;
        BOLDCOMMON.alignment = TextAnchor.MiddleCenter;
        BOLDCOMMON.fontStyle = FontStyle.BoldAndItalic;
        BOLDCOMMON.normal.textColor = Color.white;
    }
}
