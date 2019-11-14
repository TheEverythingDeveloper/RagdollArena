using System.Linq;

public class MonsterMinSizeCubes : Monster
{
    public float maxSizeTarget;

    protected override void ConditionList()
    {
        var targets = FindObjectsOfType<SpawnedCube>().Where(x => x.Life > 0 && x.Size < maxSizeTarget).ToList();
        cubes = targets;
    }

    protected override void ConditionTarget()
    {
        target = cubes.OrderBy(x => x.Size).FirstOrDefault();
    }
}
