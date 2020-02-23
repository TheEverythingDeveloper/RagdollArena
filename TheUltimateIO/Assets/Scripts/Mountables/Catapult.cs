using System.Collections;
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

        if (!photonView.IsMine) return;

        gameCanvas = FindObjectOfType<GameCanvas>();
        _catapultPanel = (CatapultPanelUI)gameCanvas.panelsVehicles.panels[1];
        FindObjectOfType<Chat>().SuscribeChat(ChatActive);
        server = FindObjectOfType<Server>();
    }

    public void ChatActive(bool active)
    {
        _chatActive = active;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_activeEquip || (mounted && !weapon.contentPlayerOpen )) return;

        if (other.gameObject.layer == Layers.PLAYER)
        {
            _characterModel = other.GetComponentInParent<CharacterModel>();
            _characterModel.characterCamera.catapultLook = positionCam;
            EnterTrigger();
        }
    }

    void EnterTrigger()
    {
        animButtonActive.SetTrigger("On");
        lookCharacter.LookActive(_characterModel.transform);
        _activeEquip = true;
        StartCoroutine(ActiveEquip());
    }

    IEnumerator ActiveEquip()
    {
        var WaitForEndOfFrame = new WaitForEndOfFrame();
        while (true)
        {
            if (Input.GetKey(KeyCode.M))
            {
                if (mounted) EnterWeapon();
                else EnterMountable();

                StopCoroutine(ActiveEquip());
            }

            yield return WaitForEndOfFrame;
        }
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
        photonView.RPC("RPCMountVehicle", RpcTarget.AllBuffered, true);
        gameCanvas.ChangeUI(ManagerPanelVehicles.Vehicles.Catapult);
        animButtonActive.SetTrigger("Off");
        lookCharacter.LookOff();
        HideModelCharacter(true);
        ActiveMountable();
    }

    void EnterWeapon()
    {
        lookCharacter.LookOff();
        AddPlayerContent(_characterModel);
    }

    [PunRPC] void RPCEnterWeapon(bool enter)
    {
        weapon.contentPlayerOpen = enter;
    }

    public override void ExitMountable()
    {
        StartCoroutine(WaitMountedAgain());
        gameCanvas.NormalUI();
        HideModelCharacter(false);
        _characterModel.transform.position = spawnOut.position;
        _characterModel.model.transform.localPosition = Vector3.zero;
        _characterModel.NormalControls();
        _characterModel.characterCamera.ChangeRespawnMode(CharacterCamera.CameraMode.ThirdPersonMode);
        EnterTrigger();
    }
    IEnumerator WaitMountedAgain()
    {
        yield return new WaitForSeconds(1);
        photonView.RPC("RPCMountVehicle", RpcTarget.AllBuffered, false);
    }

    public override void ArtificialUpdate()
    {
        if (_chatActive) return;
        LookAttack();
        Attack();

        if (weapon.preparingShoot) return;

        if (Input.GetKeyDown(KeyCode.Tab)) ChangeAmmunition();
        if (Input.GetKeyDown(KeyCode.M)) ExitMountable();
    }

    public override void ArtificialFixedUpdate()
    {
        if (_chatActive) return;
        _characterModel.characterCamera.ArtificialFixedUpdate();

        weapon.WeaponActiveAddForce();
        RotateLookMouse();

        var horAxis = Input.GetAxis("Horizontal");
        var verAxis = Input.GetAxis("Vertical");

        if (horAxis != 0 || verAxis != 0)
            server.MovePlayer(photonView.Controller, horAxis, verAxis);
    }

    public override void ArtificialLateUpdate()
    {
        if (_chatActive) return;
        _characterModel.characterCamera.ArtificialLateUpdate();
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
        if (!weapon.contentPlayerOpen) return;

        if(weapon.AddContentPlayer(character.transform, character.model.transform, character.rb))
        {
            photonView.RPC("RPCMountVehicle", RpcTarget.AllBuffered, false);
            photonView.RPC("RPCEnterWeapon", RpcTarget.AllBuffered, true);
            _catapultPanel.ChangeAmmunition(_ammunitionType, !weapon.contentPlayerOpen);
            character.ChangeControls(AssistantUpdate, delegate { }, AssistantLateUpdate, AssistantMovement);
        }
    }

    void AssistantMovement(float hor, float ver)
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
