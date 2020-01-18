using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class Core : MonoBehaviourPun
{
    public int teamID;

    private void Start()
    {
        UpdateMaterial(teamID);
    }

    [PunRPC] public void RPCSetTeam(int newTeamID)
    {
        teamID = newTeamID + 1;
    }

    private void UpdateMaterial(int newTeamID)
    {
        GetComponentInChildren<MeshRenderer>().material.color =
            teamID == 1 ? Color.blue : teamID == 2 ? Color.red : teamID == 3 ? Color.yellow : Color.green;
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
}
