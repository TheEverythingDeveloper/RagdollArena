using System.Linq;
using UnityEngine;

public class MonsterRandom : Monster
{
    protected override void ConditionTarget()
    {
        var r = Random.Range(1, cubes.Count);
        target = cubes.Take(r).ToList().First(); //IA2-P1
    }
}
