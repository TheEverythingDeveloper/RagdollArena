using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon;

public class SpawnPoint : MonoBehaviourPun
{
    [Tooltip("Saber si se puede usar si otro lo esta usando en el mismo momento")] public bool isOccupied;
    [Tooltip("Si no somos del equipo correspondiente a este spawnpoint no lo podemos usar")] public int teamID; //0 => sin equipo. 1,2,3,4 => los posibles equipos
    [Tooltip("Cola de jugadores queriendo spawnear en este mismo spawnpoint")] private Queue<Player> _playerRespawnQueue = new Queue<Player>();
    private Server _server;
    [SerializeField] private GameObject _spawnPointCanvas;

    private void Start()
    {
        StartCoroutine(DelayForServer());
    }

    private IEnumerator DelayForServer()
    {
        yield return new WaitForSeconds(0.5f);
        _server = FindObjectOfType<Server>();
        _server.OnRespawnFeedback += CanvasVisibility;
    }

    /// <summary>
    /// Usar este spawnpoint para respawnear. Como tarda un poco respawnear y no pueden 2 jugadores respawnear en el mismo lugar, el spawnpoint se ocupa.
    /// </summary>
    public void ClickedOnSpawnPoint()
    {
        UseSpawnPoint(PhotonNetwork.LocalPlayer);
    }
    public void UseSpawnPoint(Player player) => photonView.RPC("RPCUseSpawnPoint", RpcTarget.MasterClient, player);
    public void CanvasVisibility(bool on) => _spawnPointCanvas.SetActive(on);

    [PunRPC] public void RPCUseSpawnPoint(Player player)
    {
        _playerRespawnQueue.Enqueue(player);
        if (!isOccupied) //se va a llamar a la corrutina solo cuando no este ocupado el spawnpoint
            StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        isOccupied = true;
        //TODO: activar algun tipo de feedback de que va a spawnear alguien ahi
        yield return new WaitForSeconds(2f);
        Player enqueuedPlayer = _playerRespawnQueue.Dequeue();
        _server.photonView.RPC("RPCRespawnPlayer", RpcTarget.MasterClient, enqueuedPlayer, transform.position);
        if (_playerRespawnQueue.Count == 0) //si somos el ultimo spawn de la queue, entonces vuelve a estar desocupado
            isOccupied = false;
        else //si no somos el ultimo, se tiene que repetir el proceso de respawn
            StartCoroutine(RespawnCoroutine());
    }

    private void LateUpdate()
    {
        _spawnPointCanvas.transform.forward = Camera.main.transform.position - transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isOccupied ? Color.red : Color.yellow;
        Gizmos.DrawSphere(transform.position, 1f);
    }
}
