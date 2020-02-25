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
    bool _activeEquip;
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
            _characterModel = other.GetComponentInParent<CharacterModel>();

            if (_characterModel.team != teamID) return;

            EnterTrigger();
        }
    }

    void EnterTrigger()
    {
        animButtonActive.SetTrigger("On");
        lookCharacter.LookActive(_characterModel.transform);
        _activeEquip = true;
        _controlsActive = true;
    }

    private bool _controlsActive;
    private void Update()
    {
        if (!_controlsActive) return;

        if (Input.GetKeyDown(KeyCode.M) && !someoneMounted)
            EnterMountable();

        if (!isPlayerMounted) return;

        if (Input.GetMouseButtonDown(0))
            photonView.RPC("RPCAttack", RpcTarget.All);
        if (Input.GetKeyDown(KeyCode.L)) ExitMountable();

        RotateLookMouse();
    }

    [PunRPC] public void RPCAttack()
    {
        weapon.Attack();
    }

    private void LateUpdate()
    {
        if (!_controlsActive) return;
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
        isPlayerMounted = false;
        photonView.RPC("RPCMountVehicle", RpcTarget.AllBuffered, false);
        _controlsActive = false;
        gameCanvas.NormalUI();
        HideModelCharacter(false);
        _characterModel.photonView.RPC("RPCResetNormalControls", RpcTarget.MasterClient, spawnOut.position);

        EnterTrigger();
    }

    public override void Move(float horizontal, float vertical)
    {
        var horAxis = horizontal * speed * Time.deltaTime;
        var verAxis = vertical * speed * Time.deltaTime;

        var dir = new Vector3(horAxis, 0, verAxis);
        transform.position += dir;
    }
}
