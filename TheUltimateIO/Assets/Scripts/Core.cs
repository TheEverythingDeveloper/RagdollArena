using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using GameUI;
using TMPro;
using UnityEngine.UI;

public class Core : MonoBehaviourPun, IDamageable
{
    public int teamID;
    [SerializeField] private Image _lifeBar;
    [SerializeField] private List<Color> _lifeColors;
    [SerializeField] private List<Color> _backgroundLifeColors;

    private void Start()
    {
        UpdateMaterial(teamID);
        _lifeBar.fillAmount = 100;

        if (!PhotonNetwork.IsMasterClient) return;

        FindObjectOfType<TeamManager>().OnCoreUpdate += OnCoreLifeUpdate;
        FindObjectOfType<TeamManager>().OnCoreDestroy += OnCoreDestroy;
    }

    public void OnCoreLifeUpdate(int teamUpdate, float life)
    {
        if (teamID != teamUpdate + 1 || this.gameObject == null) return;

        photonView.RPC("RPCCoreLifeUpdate", RpcTarget.All, life);
    }

    public void OnCoreDestroy(int teamDestroyed)
    {
        if (this.gameObject == null) return;
        if (teamID != teamDestroyed + 1) return;

        FindObjectOfType<TeamManager>().OnCoreUpdate -= OnCoreLifeUpdate;
        FindObjectOfType<TeamManager>().OnCoreDestroy -= OnCoreDestroy;
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC] private void RPCCoreLifeUpdate(float life)
    {
        _lifeBar.fillAmount = life;
    }
    [PunRPC] public void RPCSetTeam(int newTeamID) => teamID = newTeamID + 1;
    private void UpdateMaterial(int newTeamID)
    {
        GetComponentInChildren<MeshRenderer>().material.color =
            teamID == 1 ? Color.blue : teamID == 2 ? Color.red : teamID == 3 ? Color.yellow : Color.green;

        _lifeBar.transform.parent.GetComponent<Image>().color = _backgroundLifeColors[teamID - 1];
        _lifeBar.color = _lifeColors[teamID - 1];
    }

    private void LateUpdate()
    {
       // _spawnPointCanvas.transform.forward = Camera.main.transform.position - transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = teamID == 1 ? Color.blue : teamID == 2 ? Color.red : teamID == 3 ? Color.yellow : Color.green;
        Gizmos.DrawSphere(transform.position, 2f);
    }

    public void Damage(Vector3 origin, float damage)
    {
        FindObjectOfType<TeamManager>().photonView.RPC("RPCCoreLifeUpdateCall", RpcTarget.MasterClient , teamID-1, -damage);
    }

    public void Explosion(Vector3 origin, float force)
    {
        throw new System.NotImplementedException();
    }
}
