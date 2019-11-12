using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using System.Linq;

public class Monster : MonoBehaviour, IDamageable
{
    Rigidbody _rb;
    public float maxLife;
    float _life;
    public float speed;
    public float velocityDamageCube;
    public float timeResetList;
    public SpawnedCube target;
    List<SpawnedCube> cubes = new List<SpawnedCube>();
    FSM<MonsterStates> _myFsm;

    public enum MonsterStates
    {
        MOVE,
        ATTACK,
        DIE
    }

    void Awake()
    {
        _life = maxLife;
        _rb = gameObject.GetComponent<Rigidbody>();

        var moving = new State<MonsterStates>("Moving");
        var attack = new State<MonsterStates>("Attack");
        var die = new State<MonsterStates>("Die");

        StateConfigurer.Create(moving).SetTransition(MonsterStates.ATTACK, attack).SetTransition(MonsterStates.DIE, die).Done();
        StateConfigurer.Create(attack).SetTransition(MonsterStates.MOVE, moving).SetTransition(MonsterStates.DIE, die).Done();
        StateConfigurer.Create(die).Done();

        //MOVING
        moving.OnUpdate += () =>
        {
            transform.forward = target.transform.position - transform.position;
        };
        moving.OnFixedUpdate += () =>
        {
            _rb.velocity += transform.forward * speed * Time.deltaTime;
        };
        //ATTACK
        attack.OnUpdate += () =>
        {
            target.GetComponent<IDamageable>().Damage(velocityDamageCube * Time.deltaTime);
        };

        _myFsm = new FSM<MonsterStates>(moving);
    }

    void Start()
    {
        CreateListTargets();
    }

    void CreateListTargets()
    {
        StartCoroutine(UpdateListTarget());
    }

    IEnumerator UpdateListTarget()
    {
        var targets = new List<SpawnedCube>();
        var waitForSeconds = new WaitForSeconds(timeResetList);

        while (true)
        {
            targets = FindObjectsOfType<SpawnedCube>().Where(x => x.GetComponent<IDamageable>() != null).ToList();
            cubes = targets;
            yield return waitForSeconds;
        }
    }

    void FindTarget()
    {
        target = cubes.OrderBy(x => x.Life).FirstOrDefault();
    }

    private void Update()
    {
        FindTarget();
        if (target == null) return;
        _myFsm.Update();
    }

    private void FixedUpdate()
    {
        if (target == null) return;
        _myFsm.FixedUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == target)
            _myFsm.ChangeState(MonsterStates.ATTACK);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == target)
            _myFsm.ChangeState(MonsterStates.MOVE);
    }

    public void Damage(float damage)
    {
        _life -= damage;

        if(_life <= 0)
        {
            _myFsm.ChangeState(MonsterStates.DIE);
        }
    }

    public void Explosion(Vector3 origin, float force)
    {
        Vector3 difference = transform.position - origin;
        float magnitude = difference.magnitude;

        _rb.AddForce(difference * force, ForceMode.Impulse);
        Damage(magnitude * force);
    }
}
