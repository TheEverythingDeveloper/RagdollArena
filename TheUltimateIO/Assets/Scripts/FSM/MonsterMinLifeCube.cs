using System.Linq;

public class MonsterMinLifeCube : Monster
{
    protected override void ConditionTarget()
    {
        target = cubes.OrderBy(x => x.Life).FirstOrDefault();
    }
}
