using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Character;

public class Ram : Mountable
{
    public float speed;
    public Animator animButtonActive;
    public LookCharacter lookCharacter;
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

        if(other.gameObject.layer == Layers.PLAYER)
        {
            animButtonActive.SetTrigger("On");
            _characterModel = other.GetComponentInParent<CharacterModel>();
            lookCharacter.LookActive(_characterModel.transform);
            _activeEquip = true;
            StartCoroutine(ActiveEquip());
        }
    }  

    IEnumerator ActiveEquip()
    {
        var WaitForEndOfFrame = new WaitForEndOfFrame();
        while (true)
        {
            if (Input.GetKey(KeyCode.M) && !mounted)
            {
                ActiveMountable();
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
    }

    public override void ExitMountable()
    {
        StartCoroutine(WaitMountedAgain());
        HideModelCharacter(false);
        _characterModel.transform.position = transform.position;
        _characterModel.NormalControls();
    }
    IEnumerator WaitMountedAgain()
    {
        yield return new WaitForSeconds(1);
        mounted = false;
    }

    public override void ArtificialUpdate()
    {
        var horAxis = Input.GetAxis("Horizontal");
        var verAxis = Input.GetAxis("Vertical");

        if (horAxis != 0 || verAxis != 0)
            server.MovePlayer(photonView.Controller, horAxis, verAxis);
    }

    public override void ArtificialFixedUpdate()
    {
        _characterModel.characterCamera.ArtificialFixedUpdate();

        if (Input.GetKeyDown(KeyCode.M)) ExitMountable();
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
}
