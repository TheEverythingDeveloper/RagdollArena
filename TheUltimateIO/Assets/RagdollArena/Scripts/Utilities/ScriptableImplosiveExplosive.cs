using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableImplosiveExplosive : ScriptableObject
{
    public LayerMask actualRadiusLayer;

    public float implosiveSpeed;
    public bool active;
    public float randomNoise;
    public Transform outerTransform;
    public Transform innerTransform;
    public float outerRadius;
    public float innerRadius;
    public bool affectYAxis;

    //Propiedades de materiales
    public Material nonSelectedObjectMaterial;
    public Material selectedObjectMaterial;
    public bool distanceGradient;
    public Color nearColor;
    public Color middleColor;
    public Color farColor;
}
