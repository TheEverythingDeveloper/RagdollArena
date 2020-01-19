using System.Linq;
using UnityEngine;

public class MonsterMinSizeCubes : Monster
{
    public float maxSizeTarget;

    protected override void ConditionList()
    {
        /*var targets = _queries.ObjectsInGrid().Select(x => x.gameObject.GetComponent<SpawnedCube>()).Where(x => !x.preCube && x.Life > 0 && x.Size < maxSizeTarget).ToList();//IA2-P1
        cubes = targets;*/
    }

    protected override void ConditionTarget()
    {
        target = cubes.OrderBy(x => x.Size).FirstOrDefault();//IA2-P1
    }
}
