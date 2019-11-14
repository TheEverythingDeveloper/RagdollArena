using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Character;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class TowerSceneManager : MonoBehaviour
{
    public List<MonsterWave> monsterWavesList = new List<MonsterWave>();
    [SerializeField] private GameObject _monsterPrefab;
    public float generalDifficulty;
    public float spawnWidth;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            CallEveryNearMonster(FindObjectOfType<CharacterModel>().transform.position);
        }
    }

    IEnumerator SpawnEnemies()
    {
        float rnd = Random.Range(5f,10f); //cambiar esto a un numero mas grande para que las oleadas tengan mas tiempo de espera
        yield return new WaitForSeconds(rnd);

        Debug.Log("spawn");
        List<Monster> intermediateList = new List<Monster>();
        List<CharacterModel> allPlayers = FindObjectsOfType<CharacterModel>().ToList();
        //IA2-P1 : Aggregate 01 (este aggregate va a ser acumular dificultad, teniendo en cuenta las posiciones de los players que hayan)
        float difficulty = allPlayers.Aggregate(0f, (acum, current) => acum + current.transform.position.y) * generalDifficulty;

        //IA2-P2 : Aggregate 02 (va a servir para sacar el promedio de las posiciones de todos)
        Tuple<Vector3, int> average = allPlayers.Aggregate(
            Tuple.Create(Vector3.zero, 0), (acum, current) => 
            acum = Tuple.Create(acum.Item1 + current.transform.position, acum.Item2+1));
        Vector3 centerOfSpawn = average.Item1 / average.Item2;

        for (int i = 0; i < Mathf.Max(3,Mathf.RoundToInt(difficulty)); i++)
        {
            intermediateList.Add(Instantiate(_monsterPrefab, 
                centerOfSpawn + new Vector3(Random.Range(-spawnWidth,spawnWidth), 2, Random.Range(-spawnWidth,spawnWidth)), Quaternion.identity).GetComponent<Monster>());
        }

        monsterWavesList.Add(new MonsterWave(intermediateList));

        StartCoroutine(SpawnEnemies());
    }

    private void CallEveryNearMonster(Vector3 pos)
    {
        //a esto hay que agregarle que solo agarre lo que hay en la grilla donde se llamo
        var allMonsters = monsterWavesList.SelectMany(x => x.MyMonsters)
                                          .Where(x => Vector3.Distance(x.transform.position, pos) < x.hearCapabilityRadius)
                                          .Select(x =>
                                          {
                                              x.CallMonster(pos);
                                              return x;
                                          })
                                          .ToList();
    }
}

[Serializable]
public class MonsterWave
{
    private List<Monster> _myMonsters = new List<Monster>();
    public List<Monster> MyMonsters { get; private set; }

    public MonsterWave(IEnumerable<Monster> initialMons)
    {
        MyMonsters = new List<Monster>(initialMons);
    }

    public void CreateMonster(Monster newMonster)
    {
        _myMonsters.Add(newMonster);
    }
}