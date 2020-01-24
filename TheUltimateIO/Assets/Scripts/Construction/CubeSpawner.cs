using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SpawnItem
{
    Block = 0,
    Respawn = 1,
    Ram = 2,
    Catapult = 3
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
    [SerializeField] private TextMeshProUGUI _constructionPointsText;
    [SerializeField] private Image _constructionCooldownImg;
    #endregion

    private void Awake()
    {
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
                _preSpawnedCubes.Add(Instantiate((GameObject)Resources.Load(SpawningItem.ToString()), Vector3.zero, Quaternion.identity).GetComponent<SpawnedCube>());

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

    private void ChooseSpawningItem()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SpawningItem = SpawnItem.Block;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SpawningItem = SpawnItem.Respawn;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SpawningItem = SpawnItem.Ram;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            SpawningItem = SpawnItem.Catapult;
    }

    private Vector3 GetMouseSpawnPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _spawnLayermask))
            _canSpawn = true;
        else
            _canSpawn = false;

        return hit.point;
    }

    private void Update()
    {
        ChooseSpawningItem();

        if (_spawningItem == 0 || _cooldownOn) return; //TODO: Que sea igual a -1 u otra cosa aca

        Vector3 hitPoint = GetMouseSpawnPos();

        _newSize = Mathf.Clamp(_lastCubeSize + Input.mouseScrollDelta.y * wheelSizeSpeed * Time.deltaTime, minMaxSize.x, minMaxSize.y);
        _lastCubeSize = _newSize;
        float gridSize = Mathf.Ceil(_newSize) - 0.002f;
        if (_preSpawnedCubes[_preSpawnedCubes.Count-1].Size != gridSize)
        {
            foreach (var x in _preSpawnedCubes)
            {
                x.SetSize(gridSize);
            }
            _lastCubeSize = gridSize;
        }

        float scaleOffset = (1 - (_preSpawnedCubes[0].Size % 2)) * 0.5f;
        Vector3 gridHitPoint = new Vector3(Mathf.Ceil(hitPoint.x) - scaleOffset, Mathf.Ceil(hitPoint.y), Mathf.Ceil(hitPoint.z) - scaleOffset);
        //_preSpawnedCubes[0].CorrectPosition(gridHitPoint);

        if (Input.GetMouseButtonDown(0))
        {
            _mouseDownHitPoint = gridHitPoint;
            ChangePreSpawnedCube(SpawningItem);
        }

        //Diferencias de X,Y,Z con este tamaño de grilla. Equivale a cuantas celdas de la grilla tiene de diferencia.
        int dx = Mathf.FloorToInt(Mathf.Abs(gridHitPoint.x - _mouseDownHitPoint.x));
        int dy = Mathf.FloorToInt(Mathf.Abs(gridHitPoint.y - _mouseDownHitPoint.y)); 
        int dz = Mathf.FloorToInt(Mathf.Abs(gridHitPoint.z - _mouseDownHitPoint.z));

        int maxAxis = 0; //0 = x, 1 = y, 2 = z.
        if (dx > dy && dx > dz) maxAxis = 0;
        else if (dy > dx && dy > dz) maxAxis = 1;
        else maxAxis = 2;
        if (Input.GetMouseButton(0)) //Ir creando la maxima cantidad de PreSpawnedCubes que se pueda
        {
            int cubesAmoount = Mathf.FloorToInt(Mathf.Max(dx, dy, dz) / _lastCubeSize);
            Debug.Log("Se pueden spawnear: " + cubesAmoount + "cubos de este tamaño");
            Debug.DrawLine(_mouseDownHitPoint, gridHitPoint, Color.red);
            CreatePreCube(cubesAmoount, _lastCubeSize, _lastCubeSize);
        }
        if (_preSpawnedCubes == null) return;
        bool leftSense = _mouseDownHitPoint[maxAxis] > gridHitPoint[maxAxis];

        for (int i = 0; i < _preSpawnedCubes.Count; i++)
        {
            _preSpawnedCubes[0].gameObject.SetActive(true);
            SpawnedCube spawnCube = _preSpawnedCubes[i];
            scaleOffset = (1 - (spawnCube.Size % 2)) * 0.5f; //para que todos los bloques esten en el mismo tipo de grilla sin importar la escala hay que hacer que los impares esten 0.5 mas al costado
            Vector3 spawnPos = Vector3.zero;
            gridHitPoint = new Vector3(Mathf.Ceil(hitPoint.x) - scaleOffset, Mathf.Ceil(hitPoint.y), Mathf.Ceil(hitPoint.z) - scaleOffset);
            int leftSenseNum = (leftSense ? -1 : 1);
            switch (maxAxis)
            {
                case 0:
                    spawnPos = new Vector3(_mouseDownHitPoint.x + (i * _lastCubeSize * leftSenseNum) * 1.013f, _mouseDownHitPoint.y, _mouseDownHitPoint.z);
                    break;
                case 1:
                    spawnPos = new Vector3(_mouseDownHitPoint.x, _mouseDownHitPoint.y + i * _lastCubeSize * leftSenseNum * 1.013f, _mouseDownHitPoint.z);
                    break;
                case 2:
                    spawnPos = new Vector3(_mouseDownHitPoint.x, _mouseDownHitPoint.y, _mouseDownHitPoint.z + i * _lastCubeSize * leftSenseNum * 1.013f);
                    break;
            }
            if (_preSpawnedCubes.Count == 1)
                spawnPos = gridHitPoint;

            spawnCube.CorrectPosition(spawnPos);

            if (spawnCube.IsColliding(_spawnLayermask))
                spawnCube.SetColor(Color.red);
            else
            {
                spawnCube.SetColor(Color.green);
                CanSpawn(spawnPos);
            }
        }
        if (_preSpawnedCubes.Count > 1)
            _preSpawnedCubes[0].CorrectPosition(_mouseDownHitPoint);
        if (_hadSpawn)
        {
            _hadSpawn = false;
            SpawnedCube _firstCube = _preSpawnedCubes[0];
            _preSpawnedCubes.Remove(_firstCube);
            foreach (var x in _preSpawnedCubes)
                Destroy(x.gameObject);
            _preSpawnedCubes.Clear();
            _preSpawnedCubes.Add(_firstCube);
            ChangePreSpawnedCube(SpawningItem);
        }
    }

    private bool _hadSpawn;
    private void CanSpawn(Vector3 spawnPos)
    {
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

    public void SpawnCube(SpawnItem spawnCube, Vector3 hitPos)
    {
        _hadSpawn = true;

        var spawnedCube = PhotonNetwork.Instantiate(spawnCube.ToString(), hitPos, Quaternion.identity)
            .GetComponent<SpawnedCube>();

        spawnedCube
            .SetLife((int)spawnCube)
            .SetSize(_preSpawnedCubes[0].Size)
            .Constructor(false)
            .CorrectPosition(hitPos)
            .SetColor(Color.white);

        ConstructionPoints -= spawnedCube.Size;
    }
}
