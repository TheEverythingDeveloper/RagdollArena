using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;

public class Monster : MonoBehaviour, IDamageable
{
    Rigidbody _rb;
    public float maxLife;
    float _life;
    public float speed;
    public float velocityDamageCube;
    public GameObject cube; //TODO Aca script del cubo para atacar
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
            transform.forward = cube.transform.position - transform.position;
        };
        moving.OnFixedUpdate += () =>
        {
            _rb.velocity += transform.forward * speed * Time.deltaTime;
        };
        //ATTACK
        attack.OnUpdate += () =>
        {
            Debug.Log("ATACO");
            //LifeCube -= velocityDamageCube * Time.deltaTime;
        };

        _myFsm = new FSM<MonsterStates>(moving);
    }

    private void Update()
    {
        _myFsm.Update();
    }

    private void FixedUpdate()
    {
        _myFsm.FixedUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == cube)
            _myFsm.ChangeState(MonsterStates.ATTACK);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == cube)
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
}
