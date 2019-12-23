using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Character;
using System;

public class MonsterFather : Monster
{
    Transform _player;
    public float maxDistTarget;
    bool _limitDistPlayer;
    public List<Vector3> positionsChildren = new List<Vector3>();

    protected override void MovingUpdate()
    {
        transform.forward = _player.transform.position - transform.position;
    }
    protected override void MovingFixedUpdate()
    {
        if (_limitDistPlayer) return;
        _rb.velocity += transform.forward * speed * Time.deltaTime;
    }

    private void Update()
    {
        if (cubes.Count > 0) ConditionTarget();

        RayTarget();
        _myFsm.Update();
    }

    public void CallFather(Vector3 pos)
    {
        var m = (GameObject)Instantiate(Resources.Load("MonsterChildren"), Vector3.zero, Quaternion.identity);
        var newMonster = m.GetComponent<MonsterChildren>();
        newMonster.father = this;
        newMonster.player = _player;
        newMonster.distPlayer = pos;
    }

    protected override void CreateListTargets()
    {
        List<MonsterChildren> childrens = new List<MonsterChildren>();
        var targets = FindObjectsOfType<CharacterModel>().ToList();
        _player = targets.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).FirstOrDefault().pelvisRb.transform;

        if (_player == null) return;

        for (int i = 0; i < positionsChildren.Count; i++)
        {
            var m = (GameObject)Instantiate(Resources.Load("MonsterChildren"), Vector3.zero, Quaternion.identity);
            var newMonster = m.GetComponent<MonsterChildren>();
            newMonster.father = this;
            newMonster.player = _player;
            childrens.Add(newMonster);
        }

        var tupleChildrens = childrens.Zip(positionsChildren, (c, p) => Tuple.Create(c, p));//IA2-P1
        tupleChildrens.Select(x => x.Item1.distPlayer = x.Item2).ToList();
    }

    protected override void ConditionTarget()
    {
        _limitDistPlayer = Vector3.Distance(transform.position, _player.transform.position) < maxDistTarget;
    }
}
