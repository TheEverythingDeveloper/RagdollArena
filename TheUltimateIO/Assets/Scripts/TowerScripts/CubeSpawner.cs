using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private int _spawningCube;
    public int SpawningCube
    {
        get { return _spawningCube; }
        set
        {
            _spawningCube = value;
            ChangePreSpawnedCube(value);
        }
    }
    private bool _canSpawn;
    [SerializeField] private LayerMask _spawnLayermask;
    private SpawnedCube _preSpawnedCube;
    public float wheelSizeSpeed;

    private void Awake()
    {
        _preSpawnedCube = Instantiate((GameObject)Resources.Load("SpawnedCube"), Vector3.zero, Quaternion.identity)
                    .GetComponent<SpawnedCube>();

        _preSpawnedCube.SetLife(1).SetSize(1f).Constructor(true).CorrectPosition(Vector3.zero);

        _preSpawnedCube.gameObject.SetActive(false);

        ChangePreSpawnedCube(0);
    }

    private void ChangePreSpawnedCube(int spawningCube)
    {
        if (spawningCube == 0)
        {
            if (_preSpawnedCube != null)
                _preSpawnedCube.gameObject.SetActive(false);
        }
        else
        {
            Destroy(_preSpawnedCube.gameObject);

            _preSpawnedCube = Instantiate((GameObject)Resources.Load("SpawnedCube"), Vector3.zero, Quaternion.identity)
                            .GetComponent<SpawnedCube>();

            _preSpawnedCube.SetLife(spawningCube).SetSize(spawningCube).Constructor(true).CorrectPosition(Vector3.zero);

            _preSpawnedCube.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SpawningCube = SpawningCube == 1 ? 0 : 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SpawningCube = SpawningCube == 2 ? 0 : 2;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SpawningCube = SpawningCube == 3 ? 0 : 3;

        if (_spawningCube == 0) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _spawnLayermask))
            _canSpawn = true;
        else
            _canSpawn = false;

        if (_preSpawnedCube != null)
        {
            _preSpawnedCube.SetSize(_preSpawnedCube.Size + Input.mouseScrollDelta.y * wheelSizeSpeed * Time.deltaTime);
            _preSpawnedCube.CorrectPosition(hit.point);
            if(_preSpawnedCube.IsColliding(_spawnLayermask))
            {
                _preSpawnedCube.SetColor(Color.red);
            }
            else
            {
                _preSpawnedCube.SetColor(Color.green);

                Debug.DrawLine(Camera.main.transform.position, ray.direction * 100f, _canSpawn ? Color.green : Color.red);

                if (Input.GetMouseButtonDown(0))
                {
                    if (_canSpawn)
                        SpawnCube(SpawningCube, hit.point);
                    else
                        Debug.Log("No se pudo spawnear");
                }
            }
        }
    }

    public void SpawnCube(int spawnCube, Vector3 hitPos)
    {
        Debug.Log("se spawneo el cubo " + spawnCube);
        var spawnedCube = Instantiate((GameObject)Resources.Load("SpawnedCube"), hitPos, Quaternion.identity)
            .GetComponent<SpawnedCube>();

        spawnedCube.SetLife(spawnCube).SetSize(_preSpawnedCube.Size).Constructor(false).CorrectPosition(hitPos).SetColor(Color.white);
    }
}
