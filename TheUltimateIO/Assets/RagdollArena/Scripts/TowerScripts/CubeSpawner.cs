using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SpawnItem
{
    None = 0,
    ConstructionBlock = 1,
    ConstructionBlockIron = 2,
    ConstructionBlockAttraction = 3
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
    private bool _cooldownOn;
    public float spawnCooldown = 0.5f;
    [SerializeField] private float _constructionPointsAmount;
    public float ConstructionPoints
    {
        get { return _constructionPointsAmount; }
        set
        {
            _constructionPointsAmount = Mathf.RoundToInt(Mathf.Clamp(value, 0, 100000));
            _constructionPointsText.text = _constructionPointsAmount.ToString();
        }
    }
    [SerializeField] private LayerMask _spawnLayermask;
    private List<SpawnedCube> _preSpawnedCubes = new List<SpawnedCube>();
    public float wheelSizeSpeed;
    public Vector2 minMaxSize;

    #region UI
    TextMeshProUGUI _constructionPointsText;
    [SerializeField] Image _constructionCooldownImg;
    #endregion

    private void Awake()
    {
        _constructionPointsText = GetComponentInChildren<TextMeshProUGUI>();
        ConstructionPoints = _constructionPointsAmount;

        CreatePreCube(1, 1f, 1f);

        ChangePreSpawnedCube(0);
    }

    private void CreatePreCube(int amount, float life, float size)
    {
        amount = Mathf.Max(1, amount);
        while (amount != _preSpawnedCubes.Count)
        {
            int last = _preSpawnedCubes.Count;
            Debug.Log("amount is " + amount + " and my last is " + last);
            if(_preSpawnedCubes.Count < amount)
            {
                _preSpawnedCubes.Add(Instantiate((GameObject)Resources.Load(SpawnItem.ConstructionBlock.ToString()), Vector3.zero, Quaternion.identity).GetComponent<SpawnedCube>());

                _preSpawnedCubes[0]
                    .SetLife(life)
                    .SetSize(size)
                    .Constructor(true)
                    .CorrectPosition(Vector3.zero);

                _preSpawnedCubes[0].gameObject.SetActive(false);
            }
            else
            {
                GameObject goToDelete = _preSpawnedCubes[last-1].gameObject;
                _preSpawnedCubes.RemoveAt(last - 1);
                Destroy(goToDelete);
            }
        }
    }

    private void ChangePreSpawnedCube(SpawnItem spawningCube)
    {
        if (spawningCube == 0)
        {
            foreach (var x in _preSpawnedCubes)
            {
                x.gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < _preSpawnedCubes.Count; i++)
            {
                Destroy(_preSpawnedCubes[i].gameObject);

                _preSpawnedCubes[i] = Instantiate((GameObject)Resources.Load(spawningCube.ToString()), Vector3.zero, Quaternion.identity)
                .GetComponent<SpawnedCube>()
                .SetLife((int)spawningCube)
                .SetSize((int)spawningCube)
                .Constructor(true)
                .CorrectPosition(Vector3.zero);

                _preSpawnedCubes[i].gameObject.SetActive(true);
            }
        }
    }

    IEnumerator SpawnCubeCooldown()
    {
        _cooldownOn = true;
        foreach (var x in _preSpawnedCubes)
        {
            x.SetColor(Color.red);
        }

        float t = 0;
        while (t < spawnCooldown)
        {
            t += Time.deltaTime;
            _constructionCooldownImg.fillAmount = t / spawnCooldown;
            yield return new WaitForEndOfFrame();
        }
        _cooldownOn = false;
        foreach (var x in _preSpawnedCubes)
        {
            x.SetColor(Color.green);
        }
    }

    float _newSize;
    float _lastCubeSize;

    private Vector3 _mouseDownHitPoint;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) //nada
            SpawningItem = SpawnItem.None;
        else if (Input.GetKeyDown(KeyCode.Alpha2)) //bloque construction
            SpawningItem = SpawnItem.ConstructionBlock;
        else if (Input.GetKeyDown(KeyCode.Alpha3)) //bloque construccion reforzada
            SpawningItem = SpawnItem.ConstructionBlockIron;
        else if (Input.GetKeyDown(KeyCode.Alpha4)) //bloque de atraccion de bloques
            SpawningItem = SpawnItem.ConstructionBlockAttraction;

        if (_spawningItem == 0 || _cooldownOn) return;

        //GameObject _initialSpawn = _preSpawnedCubes[0].gameObject;
        //_preSpawnedCubes.Clear();
        //_preSpawnedCubes.Add(_initialSpawn.GetComponentInChildren<SpawnedCube>());
        //Raycast y Construccion

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _spawnLayermask))
            _canSpawn = true;
        else
            _canSpawn = false;

        _newSize = Mathf.Clamp(_lastCubeSize + Input.mouseScrollDelta.y * wheelSizeSpeed * Time.deltaTime, minMaxSize.x, minMaxSize.y);
        _lastCubeSize = _newSize;
        float gridSize = Mathf.Ceil(_newSize) - 0.002f;
        if (_preSpawnedCubes[0].Size != gridSize)
        {
            foreach (var x in _preSpawnedCubes)
            {
                x.SetSize(gridSize);
            }
            _lastCubeSize = gridSize;
        }

        float scaleOffset = (1 - (_preSpawnedCubes[0].Size % 2)) * 0.5f;
        Vector3 gridHitPoint = new Vector3(Mathf.Ceil(hit.point.x) - scaleOffset, Mathf.Ceil(hit.point.y), Mathf.Ceil(hit.point.z) - scaleOffset);

        if (Input.GetMouseButtonDown(0))
        {
            _mouseDownHitPoint = gridHitPoint;
        }

        int dx = Mathf.FloorToInt(Mathf.Abs(gridHitPoint.x - _mouseDownHitPoint.x));
        int dy = Mathf.FloorToInt(Mathf.Abs(gridHitPoint.y - _mouseDownHitPoint.y));
        int dz = Mathf.FloorToInt(Mathf.Abs(gridHitPoint.z - _mouseDownHitPoint.z));

        if (Input.GetMouseButton(0))
        {
            int cubesAmoount = Mathf.FloorToInt(Mathf.Max(dx, dy, dz) / _lastCubeSize);
            Debug.Log("Se pueden spawnear: " + cubesAmoount + "cubos de este tamaño");
            Debug.DrawLine(_mouseDownHitPoint, gridHitPoint, Color.red);
            CreatePreCube(cubesAmoount, _lastCubeSize, _lastCubeSize);
        }

        if (_preSpawnedCubes == null) return;

        foreach (var spawnCube in _preSpawnedCubes)
        {
            scaleOffset = (1 - (spawnCube.Size % 2)) * 0.5f; //para que todos los bloques esten en el mismo tipo de grilla sin importar la escala hay que hacer que los impares esten 0.5 mas al costado
            gridHitPoint = new Vector3(Mathf.Ceil(hit.point.x) - scaleOffset, Mathf.Ceil(hit.point.y), Mathf.Ceil(hit.point.z) - scaleOffset);
            int x, z = 0;
            for (x = Mathf.FloorToInt(_mouseDownHitPoint.x); x < Mathf.FloorToInt(gridHitPoint.x); x++)
            {
                z = Mathf.FloorToInt(gridHitPoint.z + dz * (x - gridHitPoint.x) / dx);
            }
            Vector3 spawnPos = new Vector3(x, gridHitPoint.y, z);
            Debug.DrawLine(transform.position, spawnPos, Color.cyan, 0.1f);
            spawnCube.CorrectPosition(spawnPos);

            if (spawnCube.IsColliding(_spawnLayermask))
            {
                spawnCube.SetColor(Color.red);
            }
            else
            {
                spawnCube.SetColor(Color.green);

                Debug.DrawLine(Camera.main.transform.position, ray.direction * 100f, _canSpawn ? Color.green : Color.red);

                if (Input.GetMouseButtonUp(0))
                {
                    if (_canSpawn && ConstructionPoints > 0)
                    {
                        StartCoroutine(SpawnCubeCooldown());

                        SpawnCube(SpawningItem, spawnPos);
                    }
                    else
                        Debug.Log("No se pudo spawnear");
                }
            }
        }
        Debug.Log(_preSpawnedCubes.Count);
    }

    public void SpawnCube(SpawnItem spawnCube, Vector3 hitPos)
    {
        Debug.Log("se spawneo el cubo " + spawnCube.ToString());

        var spawnedCube = PhotonNetwork.Instantiate(spawnCube.ToString(), hitPos, Quaternion.identity)
            .GetComponent<SpawnedCube>();

        foreach (var x in _preSpawnedCubes)
        {
            ConstructionPoints -= x.Size;

            spawnedCube
                .SetLife((int)spawnCube)
                .SetSize(x.Size)
                .Constructor(false)
                .CorrectPosition(hitPos)
                .SetColor(Color.white);
        }
    }
}
