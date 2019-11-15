using UnityEngine;
using Character;

public class MonsterChildren : Monster
{
    [HideInInspector] public MonsterFather father;
    [HideInInspector] public Transform player;
    public Vector3 distPlayer;
    public Vector3 _positionMonster;
    bool _idle;

    protected override void Start()
    {

    }

    private void Update()
    {
        _positionMonster = player.position + distPlayer;

        RayTarget();
        _myFsm.Update();
    }

    protected override void InIdle()
    {
        if (Vector3.Distance(transform.position, _positionMonster) > 2)
        {
            _myFsm.ChangeState(MonsterStates.MOVE);
        }

    }

    protected override void MovingUpdate()
    {
        transform.forward = _positionMonster - transform.position;

        if (Vector3.Distance(transform.position, _positionMonster) < 2)
        {
             _myFsm.ChangeState(MonsterStates.IDLE);
        }
    }
    protected override void MovingFixedUpdate()
    {
        _rb.velocity += transform.forward * speed * Time.deltaTime;
    }

    public override void Die()
    {
        father.CallFather(distPlayer);
        base.Die();
    }
}
