using System.Collections;
using UnityEngine;
using Character;
using Photon.Pun;

public class CatapultWeapon : WeaponVehicle
{
    public Catapult catapult;
    public Vector3 directionAngle;
    public Vector3 directionShoot;
    public GameObject content;
    public Rigidbody contentRb;

    public float minForce;
    public float maxForce;
    public float minMultiplySpeedAttack;
    public float maxMultiplySpeedAttack;
    public float speedAmountForce;
    public bool contentPlayerOpen = true;
    [HideInInspector] public bool preparingShoot;
    float _force;

    public GameObject rockBullet;
    CharacterModel _actualMounted;

    private void Awake()
    {
        _force = minForce;
        contentPlayerOpen = true;
    }

    public void WeaponActiveAddForce()
    {
        if (_attack) return;

        if (Input.GetMouseButton(0))
            AddForce();
    }
    public override void WeaponActive()
    {
        if (_attack) return;

        if (Input.GetMouseButtonUp(0))
            Attack();
    }
    public override void Attack()
    {
        if (_coroutineAttack == null)
        {
            _attack = true;
            _coroutineAttack = StartCoroutine(Shoot());
        }
        else ResetForce();
    }

    IEnumerator Shoot()
    {
        var WaitForEndOfFrame = new WaitForEndOfFrame();
        var localEulerAngleX = transform.localEulerAngles.x;

        while (_attack)
        {
            transform.localEulerAngles += directionAngle * (speedAttack * Remap(_force, minForce, maxForce, minMultiplySpeedAttack, maxMultiplySpeedAttack)) * Time.deltaTime;

            localEulerAngleX = transform.localEulerAngles.x > 180 ? transform.localEulerAngles.x - 360 : transform.localEulerAngles.x;

            if (localEulerAngleX >= limit) _attack = false;

            yield return WaitForEndOfFrame;
        }
        if (contentPlayerOpen) ActiveRock(false);
        ShootContent();

        while (localEulerAngleX > 0)
        {
            transform.localEulerAngles -= directionAngle * speedReturn * Time.deltaTime;

            localEulerAngleX = transform.localEulerAngles.x > 180 ? transform.localEulerAngles.x - 360 : transform.localEulerAngles.x;

            yield return WaitForEndOfFrame;
        }
        if (contentPlayerOpen) ActiveRock(true);
        _coroutineAttack = null;
    }

    public void ChangeAmmunition(int ammunition)
    {
        switch (ammunition)
        {
            case 0:
                catapult.photonView.RPC("RPCEnterWeapon", RpcTarget.AllBuffered, false);
                ActiveRock(true);
                CheckMountPlayer();
                break;
            case 1:
                catapult.photonView.RPC("RPCEnterWeapon", RpcTarget.AllBuffered, true);
                ActiveRock(false);
                break;
            default:
                catapult.photonView.RPC("RPCEnterWeapon", RpcTarget.AllBuffered, false);
                ActiveRock(true);
                CheckMountPlayer();
                break;
        }
    }

    void ActiveRock(bool active)
    {
        photonView.RPC("RPCActiveRock", RpcTarget.AllBuffered, active);

        if (active) CheckMountPlayer();
    }
    [PunRPC] void RPCActiveRock(bool active) { rockBullet.SetActive(active); }

    void CheckMountPlayer()
    {
        if (contentRb == null) return;

        ExitMountedPlayer();
    }

    public void ExitMountedPlayer()
    {
        _actualMounted.transform.parent = null;
        _actualMounted.NormalControls();
        _actualMounted.transform.position = catapult.spawnOut.position;
        _actualMounted = null;
    }
    void AddForce()
    {
        _force += speedAmountForce * Time.deltaTime;
        _force = Mathf.Clamp(_force, minForce, maxForce);

        catapult.UpdateForce(Remap(_force, minForce, maxForce, 0, 1));

        preparingShoot = true;
    }

    void ResetForce()
    {
        preparingShoot = false;
        _force = minForce;
        catapult.UpdateForce(0);
    }

    void ShootContent()
    {
        if (contentRb == null)
        {
            var rock = PhotonNetwork.Instantiate("CatapultRock", content.transform.position, content.transform.rotation).GetComponent<RockCatapult>();
            rock.transform.position = content.transform.position;
            contentRb = rock.rb;
        }
        else
        {
            _actualMounted.transform.parent = null;
            _actualMounted.NormalControls();
            _actualMounted = null;
            contentRb.isKinematic = false;
        }

        contentRb.AddForce((catapult.transform.forward * _force) + (catapult.transform.up * _force), ForceMode.Impulse);

        contentRb = null;
        ResetForce();
    }

    public bool AddContentPlayer(CharacterModel player, Transform model, Rigidbody rb)
    {
        if (contentRb != null) return false;

        _actualMounted = player;
        _actualMounted.transform.parent = content.transform;
        _actualMounted.transform.localPosition = Vector3.zero;
        model.localPosition = Vector3.zero;
        contentRb = rb;
        contentRb.isKinematic = true;

        return true;
    }

    float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

}
