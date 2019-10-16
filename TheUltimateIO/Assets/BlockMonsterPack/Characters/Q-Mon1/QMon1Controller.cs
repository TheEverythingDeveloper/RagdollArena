using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Character;

public class QMon1Controller : MonoBehaviour
{
    private Animator _anim;
    private Rigidbody _rb;
    public float speed;
    public GameObject _model;
    public Transform actualTarget;
    public float viewRadius;
    public LayerMask playersLayermask;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        StartCoroutine(GetNearestPlayerCoroutine(Random.Range(0.5f,4f)));
    }

    private IEnumerator GetNearestPlayerCoroutine(float randomTime)
    {
        while (true)
        {
            var nearCharacters = Physics.OverlapSphere(
             transform.position, viewRadius, playersLayermask, QueryTriggerInteraction.Collide).
                OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).ToList();

            if (nearCharacters.Any())
                actualTarget = nearCharacters.First().transform;
            else
                actualTarget = null;

            yield return new WaitForSeconds(randomTime);
        }
    }

    private void Update()
    {
        if(actualTarget != null)
        {
            transform.forward = actualTarget.transform.position - transform.position;
        }
    }

    private void FixedUpdate()
    {
        if(actualTarget != null)
        {
            _rb.AddForce(transform.forward * speed, ForceMode.Force);
            _anim.SetFloat("Speed", _rb.velocity.magnitude);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }
}
