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
        Debug.Log("trigger 0");
        if (_activeEquip || someoneMounted) return;

        Debug.Log("trigger 1");
        if (other.gameObject.layer == Layers.PLAYER)
        {
            Debug.Log("trigger 2");
            _characterModel = other.GetComponentInParent<CharacterModel>();

            if (_characterModel.team != teamID) return;
            Debug.Log("trigger 3");

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
        {
            Debug.Log("trigger 5");
            EnterMountable();
        }

        if (!isPlayerMounted) return;

        if (Input.GetMouseButtonDown(0))
            weapon.Attack();
        if (Input.GetKeyDown(KeyCode.L)) ExitMountable();

        RotateLookMouse();
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
        Debug.Log("trigger 6");
    }

    public override void ExitMountable()
    {
        isPlayerMounted = false;
        Debug.Log("update 2");
        photonView.RPC("RPCMountVehicle", RpcTarget.AllBuffered, false);
        _controlsActive = false;
        gameCanvas.NormalUI();
        HideModelCharacter(false);
        _characterModel.photonView.RPC("RPCResetNormalControls", RpcTarget.MasterClient, spawnOut.position);

        EnterTrigger();
    }

    public override void ArtificialUpdate() { }

    public override void ArtificialFixedUpdate() { }

    public override void ArtificialLateUpdate() { }

    public override void Move(float horizontal, float vertical)
    {
        Debug.Log("Move call " + horizontal + "  " + vertical);

        var horAxis = horizontal * speed * Time.deltaTime;
        var verAxis = vertical * speed * Time.deltaTime;

        var dir = new Vector3(horAxis, 0, verAxis);
        transform.position += dir;
    }
}
