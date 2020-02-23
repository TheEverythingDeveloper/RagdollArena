﻿using UnityEngine;
using Character;
using Photon.Pun;
using System.Collections.Generic;

public class Mountable : MonoBehaviourPun, IUpdatable
{
    public CharacterModel _characterModel;
    public float sqrMagnitudeInTimeSpeed;
    public float rotationSpeed;
    public LayerMask floorLayers;
    public bool mounted;
    protected Rigidbody _rb;

    protected Quaternion _initialRot;
    protected Quaternion _lookRotation;
    protected Vector3 _direction;

    public List<MeshRenderer> allRenderers = new List<MeshRenderer>();

    public int teamID;

    public virtual void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _initialRot = _rb.transform.localRotation;
    }

    [PunRPC] public void RPCSetTeamID(int newTeamID)
    {
        teamID = newTeamID;
        foreach (var x in allRenderers)
        {
            x.material.SetColor("_SkinColor", newTeamID == 0 ? Color.blue : newTeamID == 1 ? Color.red : newTeamID == 2 ? Color.yellow : Color.green);
        }
    }

    [PunRPC]
    public void RPCMountVehicle(bool mount)
    {
        mounted = mount;
    }

    public virtual void ActiveMountable()
    {
        _characterModel.ChangeControls(ArtificialUpdate, ArtificialFixedUpdate, ArtificialLateUpdate, Move, _rb);
    }

    public virtual void HideModelCharacter(bool hide)
    {
        _characterModel.HideModel(hide);
    }

    public virtual void ArtificialUpdate()
    {

    }

    public virtual void ArtificialFixedUpdate()
    {
        
    }

    public virtual void ArtificialLateUpdate()
    {
        
    }

    public virtual void Move(float horizontal, float vertical)
    {

    }

    public virtual void EnterMountable()
    {

    }

    public virtual void ExitMountable()
    {

    }

    public virtual void DestroyVehicle()
    {

    }

    public virtual void RotateLookMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, floorLayers))
        {
            if (Vector3.Distance(hit.point, _rb.transform.position) < 2) return;
            _direction = (hit.point - _rb.transform.position).normalized;
            _lookRotation = Quaternion.LookRotation(_direction);

            _rb.transform.localRotation = Quaternion.Slerp(
                _rb.transform.localRotation, _lookRotation, Time.deltaTime * rotationSpeed);

            _rb.transform.localRotation = Quaternion.Euler(_initialRot.eulerAngles.x, _lookRotation.eulerAngles.y, _initialRot.eulerAngles.z);
        }
    }
}