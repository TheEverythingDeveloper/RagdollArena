using UnityEngine;

public enum SpawnItem
{
    None = 0,
    ConstructionBlock = 1,
    ConstructionBlockIron = 2,
}

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private SpawnItem _spawningItem;
    public SpawnItem SpawningItem
    {
        get { return _spawningItem; }
        set
        {
            _spawningItem = value;
            ChangePreSpawnedCube(value);
        }
    }
    private bool _canSpawn;
    [SerializeField] private LayerMask _spawnLayermask;
    private SpawnedCube _preSpawnedCube;
    public float wheelSizeSpeed;
    public Vector2 minMaxSize;

    private void Awake()
    {
        _preSpawnedCube = Instantiate((GameObject)Resources.Load(SpawnItem.ConstructionBlock.ToString()), Vector3.zero, Quaternion.identity)
                    .GetComponent<SpawnedCube>();

        _preSpawnedCube
            .SetLife(1f)
            .SetSize(1f)
            .Constructor(true)
            .CorrectPosition(Vector3.zero);

        _preSpawnedCube.gameObject.SetActive(false);

        ChangePreSpawnedCube(0);
    }

    private void ChangePreSpawnedCube(SpawnItem spawningCube)
    {
        if (spawningCube == 0)
        {
            if (_preSpawnedCube != null)
                _preSpawnedCube.gameObject.SetActive(false);
        }
        else
        {
            Destroy(_preSpawnedCube.gameObject);

            _preSpawnedCube = Instantiate((GameObject)Resources.Load(spawningCube.ToString()), Vector3.zero, Quaternion.identity)
                            .GetComponent<SpawnedCube>();

            _preSpawnedCube
                .SetLife((int)spawningCube)
                .SetSize((int)spawningCube)
                .Constructor(true)
                .CorrectPosition(Vector3.zero);

            _preSpawnedCube.gameObject.SetActive(true);
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) //nada
            SpawningItem = SpawnItem.None;
        else if (Input.GetKeyDown(KeyCode.Alpha2)) //bloque construction
            SpawningItem = SpawnItem.ConstructionBlock;
        else if (Input.GetKeyDown(KeyCode.Alpha3)) //bloque construccion reforzada
            SpawningItem = SpawnItem.ConstructionBlockIron;

        if (_spawningItem == 0) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _spawnLayermask))
            _canSpawn = true;
        else
            _canSpawn = false;

        if (_preSpawnedCube != null)
        {
            float newSize = Mathf.Clamp(_preSpawnedCube.Size + Input.mouseScrollDelta.y * wheelSizeSpeed * Time.deltaTime, minMaxSize.x, minMaxSize.y);
            _preSpawnedCube.SetSize(newSize);
            _preSpawnedCube.CorrectPosition(hit.point);
            if (_preSpawnedCube.IsColliding(_spawnLayermask))
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
                        SpawnCube(SpawningItem, hit.point);
                    else
                        Debug.Log("No se pudo spawnear");
                }
            }
        }
    }

    public void SpawnCube(SpawnItem spawnCube, Vector3 hitPos)
    {
        Debug.Log("se spawneo el cubo " + spawnCube.ToString());
        var spawnedCube = Instantiate((GameObject)Resources.Load(spawnCube.ToString()), hitPos, Quaternion.identity)
            .GetComponent<SpawnedCube>();

        spawnedCube
            .SetLife((int)spawnCube)
            .SetSize(_preSpawnedCube.Size)
            .Constructor(false)
            .CorrectPosition(hitPos)
            .SetColor(Color.white);
    }
}
