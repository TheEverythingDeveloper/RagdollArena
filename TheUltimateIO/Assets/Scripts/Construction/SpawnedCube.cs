﻿using UnityEngine;
using System.Collections;
using System.Linq;
using Photon;
using Photon.Realtime;
using Photon.Pun;

public class SpawnedCube : MonoBehaviourPun, IDamageable, IAttractable
{
    private float _life = 1;
    private float _maxLife;
    private float _size = 1;

    public ParticleSystem particlesBreak;
    public GameObject model;
    public Collider colliderCube;

    public float Life
    {
        get { return _life; }
        set { _life = Mathf.Max(0, value); UpdateHealthState(_life, _maxLife); if (_life <= 0) BreakCube(); }
    }
    public float Size
    {
        get { return _size; }
        set
        {
            _size = Mathf.Max(0, value);
            _rb.mass = _size * 25f;
            transform.localScale = new Vector3(_size, _size, _size);
        }
    }

    public SpawnedCube SetSize(float size) { Size = size; return this; }
    public SpawnedCube SetLife(float life) { _maxLife = life; Life = life; return this; }

    private Rigidbody _rb;
    private BoxCollider _boxColl;
    private MeshRenderer _meshRen;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _boxColl = GetComponent<BoxCollider>();
        _meshRen = GetComponentInChildren<MeshRenderer>();
        _rb.isKinematic = true;
        _boxColl.enabled = false;
    }

    public bool IsColliding(LayerMask _collideLayermask)
    {
        return Physics.OverlapBox
            (transform.position + new Vector3(0,_size/2f,0), new Vector3(_size, _size, _size) / 2f, Quaternion.identity, _collideLayermask).Count() > 0;
    }

    public SpawnedCube SetColor(Color newColor)
    {
        _meshRen.material.color = newColor; 
        return this;
    }

    public SpawnedCube CorrectPosition(Vector3 newPos)
    {
        transform.position = new Vector3(newPos.x, newPos.y + Size / 2f, newPos.z);
        return this;
    }

    public bool preCube; //saber si se spawneo o no

    /// <summary>
    /// "Constructor" de cuando spawneamos el cubo
    /// </summary>
    /// <param name="isPreCube">Si es true significa que no es un cubo real, todavia no tiene collider ni rigidbody</param>
    public SpawnedCube Constructor(bool isPreCube)
    {
        preCube = isPreCube;
        if (preCube) return this;

        photonView.RPC("RPCConstructor", RpcTarget.All);
        return this;
    }

    [PunRPC] public void RPCConstructor()
    {
        _rb.isKinematic = false;
        _boxColl.enabled = true;
    }

    private void UpdateHealthState(float life, float maxLife)
    {
        _meshRen.material.color = _meshRen.material.color * (life / maxLife);
        _meshRen.material.color = new Color(_meshRen.material.color.r, _meshRen.material.color.g, _meshRen.material.color.b, 1f);
    }

    private void BreakCube()
    {
        photonView.RPC("RPCBreakCube", RpcTarget.AllBuffered);
        StartCoroutine(DestroyCube());
    }

    IEnumerator DestroyCube()
    {
        yield return new WaitForSeconds(3f);
        photonView.RPC("RPCDestroyCube", RpcTarget.MasterClient);
    }

    [PunRPC] private void RPCDestroyCube()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC] void RPCBreakCube()
    {
        model.gameObject.SetActive(false);
        colliderCube.enabled = false;
        _rb.isKinematic = true;
        particlesBreak.Play();
    }

    public void Damage(Vector3 origin, float damage)
    {
        Life -= damage;
    }

    public void Explosion(Vector3 origin, float force)
    {
        Vector3 difference = transform.position - origin;
        float magnitude = difference.magnitude;

        _rb.AddForce(difference * force, ForceMode.Impulse);
        Damage(origin, magnitude * force);
    }

    public void Attract(Vector3 attractedZone, float attractedForce)
    {
        _rb.AddForce((attractedZone - transform.position).normalized * attractedForce);
    }
}
