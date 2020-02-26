using Character;
using GameUI;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Ram : Mountable, IDamageable
{
    public float maxLife;
    float _life;
    public float speed;
    public Animator animButtonActive;
    public LookCharacter lookCharacter;
    public Transform spawnOut;
    public RamWeapon weapon;
    public bool _activeEquip;
    bool _chatActive;
    Server server;
    GameCanvas gameCanvas;

    public override void Start()
    {
        base.Start();

        FindObjectOfType<Chat>().SuscribeChat(ChatActive);
        gameCanvas = FindObjectOfType<GameCanvas>();
        server = FindObjectOfType<Server>();
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

    public void Damage(Vector3 origin, float damage)
    {
        _life -= damage;
        gameCanvas.panelsVehicles.panels[0].ChangeLife(_life / maxLife);

        if (_life <= 0) DestroyVehicle();
    }

    public void Explosion(Vector3 origin, float force)
    {
        throw new System.NotImplementedException();
    }
    public override void DestroyVehicle()
    {
        FindObjectOfType<Chat>().DesuscribeChat(ChatActive);
        if (isPlayerMounted) _characterModel.ExitActualMountable();

        PhotonNetwork.Destroy(gameObject);
    }
}
