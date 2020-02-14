using UnityEngine;
using Character;
using Photon.Pun;

public class Mountable : MonoBehaviourPun
{
    public CharacterModel _characterModel;
    protected Rigidbody _rb;

    public virtual void Start()
    {
        _rb = GetComponent<Rigidbody>();      
    }

    public virtual void ActiveMountable()
    {
        _characterModel.ChangeControls(ArtificialUpdate, ArtificialFixedUpdate, ArtificialLateUpdate, Move, _rb);
    }

    public virtual void HideModelCharacter(bool hide)
    {
        photonView.RPC("RPCMovePlayer", RpcTarget.All, hide);
    }
    [PunRPC] void RPCHideModelCharacter(bool hide) { _characterModel.model.SetActive(!hide); }

    public virtual void ArtificialUpdate()
    {

    }

    public virtual void ArtificialFixedUpdate()
    {
        
    }

    public virtual void ArtificialLateUpdate()
    {
        
    }

    public virtual void Move(float horizontal, float vertical)
    {

    }

    public virtual void EnterMountable()
    {

    }

    public virtual void ExitMountable()
    {

    }
}
