using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ImplosiveExplosiveScript : MonoBehaviour
{
    //A quienes afecta
    public List<Collider> perfectRangeColliders = new List<Collider>();
    public LayerMask actualRadiusLayer;


    //Stats generales del funcionamiento del editor
    [Tooltip("Si esto es negativo, entonces va a explotar en vez de implosionar")]
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

    public ScriptableImplosiveExplosive SaveScriptableObject()
    {
        ScriptableImplosiveExplosive newScriptableIE = new ScriptableImplosiveExplosive();
        newScriptableIE.actualRadiusLayer = actualRadiusLayer;
        newScriptableIE.implosiveSpeed = implosiveSpeed;
        newScriptableIE.active = active;
        newScriptableIE.randomNoise = randomNoise;
        newScriptableIE.outerTransform = outerTransform;
        newScriptableIE.innerTransform = innerTransform;
        newScriptableIE.outerRadius = outerRadius;
        newScriptableIE.innerRadius = innerRadius;
        newScriptableIE.affectYAxis = affectYAxis;
        newScriptableIE.nonSelectedObjectMaterial = nonSelectedObjectMaterial;
        newScriptableIE.selectedObjectMaterial = selectedObjectMaterial;
        newScriptableIE.distanceGradient = distanceGradient;
        newScriptableIE.nearColor = nearColor;
        newScriptableIE.middleColor = middleColor;
        newScriptableIE.farColor = farColor;
        return newScriptableIE;
    }

    public void LoadScriptableObject(ScriptableImplosiveExplosive selected)
    {
        actualRadiusLayer = selected.actualRadiusLayer;
        implosiveSpeed = selected.implosiveSpeed;
        active = selected.active;
        randomNoise = selected.randomNoise;
        outerTransform.position = selected.outerTransform.position;
        innerTransform.position = selected.innerTransform.position;
        outerRadius = selected.outerRadius;
        innerRadius = selected.innerRadius;
        affectYAxis = selected.affectYAxis;
        nonSelectedObjectMaterial = selected.nonSelectedObjectMaterial;
        selectedObjectMaterial = selected.selectedObjectMaterial;
        distanceGradient = selected.distanceGradient;
        nearColor = selected.nearColor;
        middleColor = selected.middleColor;
        farColor = selected.farColor;
    }

    public void EditorUpdate()
    {
        outerRadius = Vector3.Distance(outerTransform.position, transform.position);
        innerRadius = Vector3.Distance(innerTransform.position, transform.position);
        foreach (var item in perfectRangeColliders)
        {
            item.GetComponent<Renderer>().material = nonSelectedObjectMaterial; //Reiniciar colores de todos
        }
        //Obtener los que estan afuera del inner, pero adentro del outer
        perfectRangeColliders = Physics.OverlapSphere(transform.position, outerRadius, actualRadiusLayer).ToList();
        List<Collider> minimumRadiusList = new List<Collider>();
        minimumRadiusList = Physics.OverlapSphere(transform.position, innerRadius, actualRadiusLayer).ToList();
        foreach (var item in minimumRadiusList)
        {
            perfectRangeColliders.Remove(item);
        }

        middleColor = Color.Lerp(nearColor, farColor, 0.5f);
        foreach (var item in perfectRangeColliders)
        {
            Vector3 distance = item.transform.position - transform.position;


            ImplosiveExplosiveMaterial(item, distance); //Todo el sistema de material de si estas cerca o lejos, etc.

            if (!active) continue; //si no esta activo entonces no sucede nada de movimiento
            ImplosiveExplosiveMovement(item, distance);
        }
    }

    private void ImplosiveExplosiveMaterial(Collider actualSelection, Vector3 distanceVector)
    {
        float distance = distanceVector.magnitude;
        Renderer itemRenderer = actualSelection.GetComponent<Renderer>();
        itemRenderer.sharedMaterial = selectedObjectMaterial; //Si estan adentro de la seleccion entonces 

        //Si esta en modo play va a tener todo el gradiente
        if (distanceGradient)
        {
            if ((distance - innerRadius) / (outerRadius - innerRadius) < 0.33f)
            {
                itemRenderer.material = (Material)Resources.Load("IENearMaterial");
                itemRenderer.sharedMaterial.color = nearColor;
            }
            else if ((distance - innerRadius) / (outerRadius - innerRadius) < 0.66f)
            {
                itemRenderer.material = (Material)Resources.Load("IEMiddleMaterial");
                itemRenderer.sharedMaterial.color = middleColor;
            }
            else
            {
                itemRenderer.material = (Material)Resources.Load("IEFarMaterial");
                itemRenderer.sharedMaterial.color = farColor;
            }
        }
        //Si esta en editor, va a tener solo 3 materiales.
    }

    public void ImplosiveExplosiveMovement(Collider actualSelection, Vector3 distance)
    {
        distance.Normalize();
        distance += new Vector3(
            Random.Range(-randomNoise, randomNoise),
            Random.Range(-randomNoise, randomNoise),
            Random.Range(-randomNoise, randomNoise));
        distance *= (implosiveSpeed * 0.01f);
        actualSelection.transform.position += distance;
        actualSelection.transform.position = 
            new Vector3(actualSelection.transform.position.x, transform.position.y, actualSelection.transform.position.z);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, outerRadius);
        Gizmos.DrawWireSphere(transform.position, innerRadius);
    }
}
