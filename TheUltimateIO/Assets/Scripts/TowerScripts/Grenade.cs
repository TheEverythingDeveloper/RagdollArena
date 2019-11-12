using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Grenade : MonoBehaviour
{
    private MeshRenderer _meshRen;
    private void Awake()
    {
        _meshRen = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        StartCoroutine(ExplodeCoroutine());
    }

    IEnumerator ExplodeCoroutine()
    {
        float t = 6f;
        while(t > 0.5f)
        {
            t /= 1.3f;
            yield return new WaitForSeconds(t);
            _meshRen.material.color = _meshRen.material.color == Color.red ? Color.grey : Color.red;
        }
        Explode();
    }

    private void Explode()
    {
        Debug.LogWarning("Boom");
    }
}
