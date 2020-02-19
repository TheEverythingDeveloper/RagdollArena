using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Character;
public class Catapult : Mountable
{
    public float speed;
    public Transform positionCam;
    public Animator animButtonActive;
    public LookCharacter lookCharacter;
    public Transform spawnOut;
    public RamWeapon weapon;
    public bool mounted;
    bool _activeEquip;
    Server server;

    public override void Start()
    {
        base.Start();

        if (!photonView.IsMine) return;

        server = FindObjectOfType<Server>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_activeEquip || mounted) return;

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
        animButtonActive.SetTrigger("Off");
        lookCharacter.LookOff();
        mounted = true;
        HideModelCharacter(true);
        _characterModel.model.SetActive(false);
        ActiveMountable();
    }

    public override void ExitMountable()
    {
        StartCoroutine(WaitMountedAgain());
        HideModelCharacter(false);
        _characterModel.transform.position = spawnOut.position;
        _characterModel.model.transform.localPosition = Vector3.zero;
        _characterModel.NormalControls();

        EnterTrigger();
    }
    IEnumerator WaitMountedAgain()
    {
        yield return new WaitForSeconds(1);
        mounted = false;
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
        if (Input.GetMouseButtonDown(1))
            _characterModel.characterCamera.ChangeRespawnMode(CharacterCamera.CameraMode.CatapultMode);
        else if (Input.GetMouseButtonUp(1))
            _characterModel.characterCamera.ChangeRespawnMode(CharacterCamera.CameraMode.ThirdPersonMode);
    }
}
