using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Grenade : MonoBehaviour
{
    private MeshRenderer _meshRen;
    public float explosionRadius = 5;
    public float explosionForce = 5;
    public LayerMask explodeLayermask;

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
        float t = 1f;
        while(t > 0.1f)
        {
            t /= 1.15f;
            yield return new WaitForSeconds(t);
            _meshRen.material.color = _meshRen.material.color == Color.red ? Color.grey : Color.red;
        }
        Explode();
    }

    private void Explode()
    {
        Debug.LogWarning("Boom");
        Physics.OverlapSphere(transform.position, explosionRadius, explodeLayermask)
                .Where(x => x.GetComponentInChildren<IDamageable>() != null)
                .Select(x =>
                {
                    Debug.Log("llego aca con " + x.name);
                    x.GetComponentInChildren<IDamageable>().Explosion(transform.position, explosionForce);
                    return x;
                }).ToList();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
