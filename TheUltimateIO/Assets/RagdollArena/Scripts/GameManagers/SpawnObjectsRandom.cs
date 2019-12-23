using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnObjectsRandom : MonoBehaviourPun
{
    public string namePrefabResource;
    public float minTimeSpawn, maxTimeSpawn;
    float _timeSpawn;
    float _timer;
    public float minX, maxX, y, minZ, maxZ;

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _timeSpawn) SpawnItem();
    }
    void SpawnItem()
    {
        _timer = 0;
        _timeSpawn = Random.Range(minTimeSpawn, maxTimeSpawn);
        var posItem = new Vector3(Random.Range(minX, maxX), y, Random.Range(minZ, maxZ));
        PhotonNetwork.Instantiate(namePrefabResource, posItem, Quaternion.identity);
    }
}
