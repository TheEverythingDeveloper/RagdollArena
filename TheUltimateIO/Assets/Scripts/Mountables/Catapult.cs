using System.Collections;
using Photon.Pun;
using UnityEngine;
using Character;
using GameUI;
using System;

public class Catapult : Mountable, IDamageable
{
    public float maxLife;
    float _life;
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

    public override void Start()
    {
        base.Start();
        _life = maxLife;
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
        if (_activeEquip || someoneMounted) return;

        if (other.gameObject.layer == Layers.PLAYER)
        {
            var model = other.GetComponentInParent<CharacterModel>();

            if (model.team != teamID) return;

            if (model.owned == false) return;

            _characterModel = model;
            _characterModel.ViewMountable(this);
            ViewOn();
        }
    }

    public override void ViewOn()
    {
        animButtonActive.SetTrigger("On");
        lookCharacter.LookActive(_characterModel.transform);
        _activeEquip = true;
        _controlsActive = true;
    }

    public override void ViewOff()
    {
        //if (!_activeEquip) return;

        animButtonActive.SetTrigger("Off");
        lookCharacter.LookOff();
        _activeEquip = false;
        _controlsActive = false;
        isPlayerMounted = false;
        StopAllCoroutines();
    }

    private bool _controlsActive;
    private void Update()
    {
        if (_chatActive) return;

        if (Input.GetKeyDown(KeyCode.M) && _controlsActive && !isPlayerMounted)
            _characterModel.EnterActualMountable();
        else if (Input.GetKeyDown(KeyCode.M) && isPlayerMounted && !weapon.preparingShoot)
            _characterModel.ExitActualMountable();

        if (!isPlayerMounted) return;

        LookAttack();
        Attack();

        _characterModel.characterCamera.ArtificialFixedUpdate();

        weapon.WeaponActiveAddForce();
        RotateLookMouse();
    }

    private void LateUpdate()
    {
        if (!isPlayerMounted) return;

        _characterModel.characterCamera.ArtificialLateUpdate();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == Layers.PLAYER)
        {
            if (other.gameObject.GetComponentInParent<CharacterModel>().owned == false) return;
            ViewOff();
        }
    }

    public override void EnterMountable()
    {
        if (!_characterModel) return;
        isPlayerMounted = true;
        _characterModel.ChangeCameraTarget(_rb);
        photonView.RPC("RPCMountVehicle", RpcTarget.AllBuffered, true);
        gameCanvas.ChangeUI(ManagerPanelVehicles.Vehicles.Catapult);
        animButtonActive.SetTrigger("Off");
        lookCharacter.LookOff();
        HideModelCharacter(true);
        _characterModel.model.SetActive(false);
        photonView.RPC("RPCActiveMountable", RpcTarget.MasterClient, _characterModel.myPhotonPlayer);
    }

    public override void ExitMountable()
    {
        if (!_characterModel) return;
        _characterModel.photonView.RPC("RPCResetNormalControls", RpcTarget.MasterClient, spawnOut.position);
        isPlayerMounted = false;
        photonView.RPC("RPCMountVehicle", RpcTarget.AllBuffered, false);
        _controlsActive = false;
        gameCanvas.NormalUI();
        HideModelCharacter(false);
        _characterModel.ChangeCameraTarget(_characterModel.rb);

        ViewOn();
    }

    public override void Move(float horizontal, float vertical)
    {
        if (_chatActive) return;

        var horAxis = horizontal * speed * Time.deltaTime;
        var verAxis = vertical * speed * Time.deltaTime;

        var dir = new Vector3(horAxis, 0, verAxis);
        transform.position += dir;
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

    public override void DestroyVehicle()
    {
        FindObjectOfType<Chat>().DesuscribeChat(ChatActive);
        if(isPlayerMounted) _characterModel.ExitActualMountable();

        photonView.RPC("RPCDestroyVehicle", RpcTarget.MasterClient);
    }
    [PunRPC] void RPCDestroyVehicle()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    public void Damage(Vector3 origin, float damage)
    {
        _life -= damage;
        _catapultPanel.ChangeLife(_life / maxLife);

        if (_life <= 0) DestroyVehicle();
    }

    public void Explosion(Vector3 origin, float force)
    {
        throw new NotImplementedException();
    }
}
