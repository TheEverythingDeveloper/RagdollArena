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

    protected override void MovingUpdate()
    {
        transform.forward = _positionMonster - transform.position;
    }
    protected override void MovingFixedUpdate()
    {
        if (_idle) return;
        _rb.velocity += transform.forward * speed * Time.deltaTime;
    }

    protected override void ConditionTarget()
    {
        _positionMonster = player.position + distPlayer;
        _idle = Vector3.Distance(transform.position, _positionMonster) < 2;
    }

    public override void Die()
    {
        father.CallFather(distPlayer);
        base.Die();
    }
}
