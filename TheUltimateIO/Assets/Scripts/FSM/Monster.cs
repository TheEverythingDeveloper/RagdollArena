using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using System.Linq;

public class Monster : MonoBehaviour, IDamageable
{
    protected Rigidbody _rb;
    public float maxLife;
    protected float _life;
    public float speed;
    public float velocityDamageCube;
    public float timeResetList;
    public float distView;
    protected SpawnedCube target;
    protected SpawnedCube blockWalk;
    public LayerMask maskEnemies;
    public List<SpawnedCube> cubes = new List<SpawnedCube>();
    protected FSM<MonsterStates> _myFsm;
    protected ParticleSystem _particles;
    protected Queries _queries;

    [Tooltip("Mientras mas de esto tenga, mas alcance a escuchar a su alrededor")] public float hearCapabilityRadius;

    public enum MonsterStates
    {
        IDLE,
        MOVE,
        ATTACK,
        DIE
    }

    protected void Awake()
    {
        _queries = GetComponent<Queries>();
        _life = maxLife;
        _rb = gameObject.GetComponent<Rigidbody>();
        _particles = GetComponent<ParticleSystem>();

        var idle = new State<MonsterStates>("Idle");
        var moving = new State<MonsterStates>("Moving");
        var attack = new State<MonsterStates>("Attack");
        var die = new State<MonsterStates>("Die");

        StateConfigurer.Create(idle).SetTransition(MonsterStates.MOVE, moving).SetTransition(MonsterStates.ATTACK, attack).SetTransition(MonsterStates.DIE, die).Done();
        StateConfigurer.Create(moving).SetTransition(MonsterStates.IDLE, idle).SetTransition(MonsterStates.ATTACK, attack).SetTransition(MonsterStates.DIE, die).Done();
        StateConfigurer.Create(attack).SetTransition(MonsterStates.IDLE, idle).SetTransition(MonsterStates.MOVE, moving).SetTransition(MonsterStates.DIE, die).Done();
        StateConfigurer.Create(die).Done();

        //IDLE
        idle.OnUpdate += () =>
        {
            InIdle();
        };
        //MOVING
        moving.OnUpdate += () =>
        {
            MovingUpdate();
        };
        moving.OnFixedUpdate += () =>
        {
            MovingFixedUpdate();
        };
        moving.OnExit += x =>
        {
            _rb.velocity = Vector3.zero;
        };
        //ATTACK
        attack.OnEnter += x =>
        {
            _particles.Play();
        };
        attack.OnUpdate += () =>
        {
            if(blockWalk)
                blockWalk.GetComponent<IDamageable>().Damage(velocityDamageCube * Time.deltaTime);
            else
                target.GetComponent<IDamageable>().Damage(velocityDamageCube * Time.deltaTime);
        };
        attack.OnExit += x =>
        {
            _particles.Stop();
        };

        _myFsm = new FSM<MonsterStates>(moving);
    }

    protected virtual void Start()
    {
        CreateListTargets();
    }

    #region Moving

    protected virtual void InIdle()
    {
        if (target != null) _myFsm.ChangeState(MonsterStates.MOVE);
    }

    protected virtual void MovingUpdate()
    {
        if (target == null)
        {
            _myFsm.ChangeState(MonsterStates.IDLE);
            return;
        }
        transform.forward = target.transform.position - transform.position;
    }

    protected virtual void MovingFixedUpdate()
    {
        if (target == null)
        {
            _myFsm.ChangeState(MonsterStates.IDLE);
            return;
        }
        _rb.velocity += transform.forward * speed * Time.deltaTime;
    }
    #endregion

    #region Conditions
    protected virtual void ConditionList()
    {
        var targets = _queries.ObjectsInGrid().Select(x => x.gameObject.GetComponent<SpawnedCube>()).Where(x => x.Life > 0 && !x.preCube).ToList();
        cubes = targets;
    }

    protected virtual void ConditionTarget()
    {

    }
    #endregion

    #region Updates
    protected virtual void Update()
    {
        if (cubes.Count > 0) ConditionTarget();

        if (target == null)
        {
            if (_myFsm.ActualState() != MonsterStates.IDLE)
                _myFsm.ChangeState(MonsterStates.IDLE);
        }
        RayTarget();
        _myFsm.Update();
    }

    protected virtual void FixedUpdate()
    {
        _myFsm.FixedUpdate();
    }
    #endregion

    #region ListTarget
    protected virtual void CreateListTargets()
    {
        StartCoroutine(UpdateListTarget());
    }

    protected IEnumerator UpdateListTarget()
    {
        var waitForSeconds = new WaitForSeconds(timeResetList);

        while (true)
        {
            ConditionList();
            yield return waitForSeconds;
        }
    }
    #endregion

    protected virtual void RayTarget()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.forward, out hit, distView, maskEnemies))
        {
            var cube = hit.collider.GetComponent<SpawnedCube>();

            if (cube == null) return;

            if (target == null)
                target = cube;
            else
                 blockWalk = cube == target? null : cube;

            if (_myFsm.ActualState() != MonsterStates.ATTACK)
                _myFsm.ChangeState(MonsterStates.ATTACK);

        }else if(_myFsm.ActualState() != MonsterStates.MOVE)
        {
            _myFsm.ChangeState(MonsterStates.MOVE);
            blockWalk = null;
            target = null;
        }
    }

    public virtual void Damage(float damage)
    {
        _life -= damage;

        if(_life <= 0)
        {
            _myFsm.ChangeState(MonsterStates.DIE);
            Die();
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

    public virtual void Explosion(Vector3 origin, float force)
    {
        Vector3 difference = transform.position - origin;
        float magnitude = difference.magnitude;

        _rb.AddForce(difference * force, ForceMode.Impulse);
        Damage(magnitude * force);
    }

    public void CallMonster(Vector3 pos)
    {
        //aca se llama al monstruo en caso de que haya pasado algo
        Debug.Log("Monster call to " + pos);
    }
}
