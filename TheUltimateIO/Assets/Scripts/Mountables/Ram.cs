using System.Collections;
using Photon.Pun;
using UnityEngine;
using Character;
using GameUI;

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

        if (!photonView.IsMine) return;

        gameCanvas = FindObjectOfType<GameCanvas>();
        server = FindObjectOfType<Server>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_activeEquip || mounted) return;

        if(other.gameObject.layer == Layers.PLAYER)
        {
            _characterModel = other.GetComponentInParent<CharacterModel>();
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
            if (Input.GetKey(KeyCode.M) && !mounted)
            {
                EnterMountable();
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
        gameCanvas.ChangeUI(ManagerPanelVehicles.Vehicles.Ram);
        animButtonActive.SetTrigger("Off");
        lookCharacter.LookOff();
        HideModelCharacter(true);
        _characterModel.model.SetActive(false);
        ActiveMountable();
    }

    public override void ExitMountable()
    {
        StartCoroutine(WaitMountedAgain());
        gameCanvas.NormalUI();
        HideModelCharacter(false);
        _characterModel.transform.position = spawnOut.position;
        _characterModel.model.transform.localPosition = Vector3.zero;
        _characterModel.NormalControls();

        EnterTrigger();
    }
    IEnumerator WaitMountedAgain()
    {
        yield return new WaitForSeconds(1);
        photonView.RPC("RPCMountVehicle", RpcTarget.AllBuffered, false);
    }

    public override void ArtificialUpdate()
    {
        Attack();
        if (Input.GetKeyDown(KeyCode.M)) ExitMountable();
    }

    public override void ArtificialFixedUpdate()
    {
        _characterModel.characterCamera.ArtificialFixedUpdate();

        RotateLookMouse();

        var horAxis = Input.GetAxis("Horizontal");
        var verAxis = Input.GetAxis("Vertical");

        if (horAxis != 0 || verAxis != 0)
            server.MovePlayer(photonView.Controller, horAxis, verAxis);
    }

    public override void ArtificialLateUpdate()
    {
        _characterModel.characterCamera.ArtificialLateUpdate();
    }

    public override void Move(float horizontal, float vertical)
    {
        var horAxis = horizontal * speed * Time.deltaTime;
        var verAxis = vertical * speed * Time.deltaTime;

        var dir = new Vector3(horAxis, 0, verAxis);
        transform.position += dir;
    }

    void Attack()
    {
        if (Input.GetMouseButtonDown(0))
            weapon.Attack();
    }

}
