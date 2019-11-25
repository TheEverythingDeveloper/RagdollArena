using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AttractionCube : SpawnedCube
{
    [Tooltip("Va a multiplicar el tamaño que tenga por este numero lo cual va a atraer mas fuerte")]
    [SerializeField] private float _attractionForce;
    [SerializeField] private float _attractionRadius; 
    [SerializeField] private LayerMask _attractedObjects;
    
    private void Update()
    {
        if (preCube) return; //que no atraiga nada todavia si no se spawneo del todo

        float attractionForce = _attractionForce * Size;
        float attractionRadius = _attractionRadius * Size;

        Physics.OverlapSphere(transform.position, attractionRadius, _attractedObjects)
            .Where(x => x.GetComponent<IAttractable>() != null)
            .Select(x => x.GetComponent<IAttractable>())
            .Select(x =>
            {
                x.Attract(transform.position, attractionForce);
                return x;
            }).ToList();
    }
}

public interface IAttractable
{
    void Attract(Vector3 attractedZone, float attractedForce);
}