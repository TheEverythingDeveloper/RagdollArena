﻿using System.Collections;
using Photon.Pun;
using UnityEngine;
using Character;
using GameUI;
using System;

public class Catapult : Mountable
{
    public float speed;
    public Transform positionCam;
    public Animator animButtonActive;
    public LookCharacter lookCharacter;
    public Transform spawnOut;
    public CatapultWeapon weapon;
    bool _chatActive;
    bool _activeEquip;
    bool _preparingShoot;
    Server server;
    [HideInInspector]public GameCanvas gameCanvas;
    CatapultPanelUI _catapultPanel;
    public int maxAmmunitionType;
    int _ammunitionType;

    public override void Start()
    {
        base.Start();

        gameCanvas = FindObjectOfType<GameCanvas>();
        _catapultPanel = (CatapultPanelUI)gameCanvas.panelsVehicles.panels[1];
        FindObjectOfType<Chat>().SuscribeChat(ChatActive);
        server = FindObjectOfType<Server>();

        if (!photonView.IsMine) return;
    }

    public void ChatActive(bool active)
    {
        _chatActive = active;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_activeEquip || (someoneMounted && !weapon.contentPlayerOpen )) return;

        if (other.gameObject.layer == Layers.PLAYER)
        {
            _characterModel = other.GetComponentInParent<CharacterModel>();

            if (_characterModel.team != teamID) return;

            _characterModel.characterCamera.catapultLook = positionCam;
            EnterTrigger();
            _controlsActive = true;
        }
    }

    void EnterTrigger()
    {
        animButtonActive.SetTrigger("On");
        lookCharacter.LookActive(_characterModel.transform);
        _activeEquip = true;
    }

    private bool _controlsActive;
    private void Update()
    {
        if (_chatActive) return;
        if (!_controlsActive) return;

        if (Input.GetKeyDown(KeyCode.M) && !someoneMounted)
            EnterMountable();
        if (Input.GetKeyDown(KeyCode.P) && weapon.contentPlayerOpen)
            EnterWeapon();

        if (!isPlayerMounted) return;

        LookAttack();
        Attack();

        _characterModel.characterCamera.ArtificialFixedUpdate();

        weapon.WeaponActiveAddForce();
        RotateLookMouse();

        if (weapon.preparingShoot) return;

        if (Input.GetKeyDown(KeyCode.Tab)) ChangeAmmunition();
        if (Input.GetKeyDown(KeyCode.L)) ExitMountable();
    }

    private void LateUpdate()
    {
        if (_chatActive) return;
        if (!isPlayerMounted) return;

        _characterModel.characterCamera.ArtificialLateUpdate();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_activeEquip) return;

        if (other.gameObject.layer == Layers.PLAYER)
        {
            animButtonActive.SetTrigger("Off");
            lookCharacter.LookOff();
            _activeEquip = false;
            StopAllCoroutines();
        }
    }

    public override void EnterMountable()
    {
        isPlayerMounted = true;
        _characterModel.ChangeCameraTarget(_rb);
        photonView.RPC("RPCMountVehicle", RpcTarget.AllBuffered, true);
        gameCanvas.ChangeUI(ManagerPanelVehicles.Vehicles.Catapult);
        animButtonActive.SetTrigger("Off");
        lookCharacter.LookOff();
        HideModelCharacter(true);
        photonView.RPC("RPCActiveMountable", RpcTarget.MasterClient, _characterModel.myPhotonPlayer);
    }

    void EnterWeapon()
    {
        Debug.LogError("EnterWeapon");
        lookCharacter.LookOff();
        AddPlayerContent(_characterModel);
    }

    [PunRPC] void RPCEnterWeapon(bool enter)
    {
        weapon.contentPlayerOpen = enter;
    }

    public override void ExitMountable()
    {
        isPlayerMounted = false;
        _characterModel.ChangeCameraTarget(_characterModel.rb);
        EnterTrigger();
        _controlsActive = false;
        gameCanvas.NormalUI();
        HideModelCharacter(false);
        _characterModel.transform.position = spawnOut.position;
        _characterModel.model.transform.localPosition = Vector3.zero;
        _characterModel.NormalControls();
        _characterModel.characterCamera.ChangeRespawnMode(CharacterCamera.CameraMode.ThirdPersonMode);
        photonView.RPC("RPCMountVehicle", RpcTarget.AllBuffered, false);
    }

    public override void Move(float horizontal, float vertical)
    {
        if (_chatActive) return;

        var horAxis = horizontal * speed * Time.deltaTime;
        var verAxis = vertical * speed * Time.deltaTime;

        var dir = new Vector3(horAxis, 0, verAxis);
        transform.position += dir;
    }

    void ChangeAmmunition()
    {
        _ammunitionType = _ammunitionType >= maxAmmunitionType ? _ammunitionType = 0 : _ammunitionType + 1;

        weapon.ChangeAmmunition(_ammunitionType);
        _catapultPanel.ChangeAmmunition(_ammunitionType, !weapon.contentPlayerOpen);
    }

    void LookAttack()
    {
        Debug.Log("1");
        if (Input.GetMouseButtonDown(1))
            _characterModel.characterCamera.ChangeRespawnMode(CharacterCamera.CameraMode.CatapultMode);
        else if (Input.GetMouseButtonUp(1))
            _characterModel.characterCamera.ChangeRespawnMode(CharacterCamera.CameraMode.ThirdPersonMode);
    }

    void Attack()
    {
        weapon.WeaponActive();
    }

    public void UpdateForce(float fill)
    {
        _catapultPanel.BarForce(fill);
    }

    public void AddPlayerContent(CharacterModel character)
    {
        Debug.LogError("AddPlayerContent");
        if (!weapon.contentPlayerOpen) return;
        Debug.LogError("AddPlayerContent-contentPlayerOpen");
        if (weapon.AddContentPlayer(character, character.model.transform, character.rb))
        {
            photonView.RPC("RPCEnterWeapon", RpcTarget.AllBuffered, true);
            _catapultPanel.ChangeAmmunition(_ammunitionType, !weapon.contentPlayerOpen);
            character.ChangeControls(AssistantUpdate, AssistantFixedUpdate, AssistantLateUpdate, AssistantMovement);
            Debug.LogError("AddPlayerContent-AddContentPlayer");
        }
    }

    void AssistantMovement(float hor, float ver)
    {

    }

    void AssistantFixedUpdate()
    {

    }

    void AssistantLateUpdate()
    {
        _characterModel.characterCamera.ArtificialFixedUpdate();
    }

    public void AssistantUpdate()
    {
        if (_chatActive) return;

        if (Input.GetKeyDown(KeyCode.M))
        {
            photonView.RPC("RPCMountVehicle", RpcTarget.AllBuffered, true);
            weapon.ExitMountedPlayer();
        }
    }

    public override void DestroyVehicle()
    {
        FindObjectOfType<Chat>().DesuscribeChat(ChatActive);
    }
}
