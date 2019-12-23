using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Queries))]
public class BoxQEditor : Editor
{
    Queries _target;
    GUIStyle style;

    private void OnEnable()
    {
        _target = (Queries)target;
        style = new GUIStyle();
        style.fontSize = 20;
        style.fontStyle = FontStyle.Bold;
    }
}
