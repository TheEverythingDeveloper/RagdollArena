using Character;
using GameUI;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Ram : Mountable
{
    public float speed;
    public Animator animButtonActive;
    public LookCharacter lookCharacter;
    public Transform spawnOut;
    public RamWeapon weapon;
    public bool _activeEquip;
    Server server;
    GameCanvas gameCanvas;

    public override void Start()
    {
        base.Start();

        gameCanvas = FindObjectOfType<GameCanvas>();
        server = FindObjectOfType<Server>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_activeEquip || someoneMounted) return;

        if (other.gameObject.layer == Layers.PLAYER)
        {
            var model = other.GetComponentInParent<CharacterModel>();

            if (model.team != teamID) return;

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
        if (!_activeEquip) return;

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
        if (Input.GetKeyDown(KeyCode.M) && _controlsActive && !isPlayerMounted)
            _characterModel.EnterActualMountable();
        else if (Input.GetKeyDown(KeyCode.M) && isPlayerMounted)
            _characterModel.ExitActualMountable();

        if (!isPlayerMounted) return;

        if (Input.GetMouseButtonDown(0))
            photonView.RPC("RPCAttack", RpcTarget.All);

        RotateLookMouse();
    }

    [PunRPC] public void RPCAttack()
    {
        weapon.Attack();
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
            ViewOff();
        }
    }

    public override void EnterMountable()
    {
        if (!_characterModel) return;
        isPlayerMounted = true;
        _characterModel.ChangeCameraTarget(_rb);
        photonView.RPC("RPCMountVehicle", RpcTarget.AllBuffered, true);
        gameCanvas.ChangeUI(ManagerPanelVehicles.Vehicles.Ram);
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
        var horAxis = horizontal * speed * Time.deltaTime;
        var verAxis = vertical * speed * Time.deltaTime;

        var dir = new Vector3(horAxis, 0, verAxis);
        transform.position += dir;
    }
}
