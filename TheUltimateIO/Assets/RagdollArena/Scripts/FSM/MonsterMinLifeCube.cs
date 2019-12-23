using System.Linq;

public class MonsterMinLifeCube : Monster
{
    protected override void ConditionTarget()
    {
        target = cubes.OrderBy(x => x.Life).First(); //IA2-P1
    }
}
