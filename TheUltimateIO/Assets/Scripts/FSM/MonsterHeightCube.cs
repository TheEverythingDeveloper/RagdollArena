using System.Linq;
public class MonsterHeightCube : Monster
{
    protected override void ConditionTarget()
    {
        target = cubes.OrderByDescending(x => x.transform.position.y).FirstOrDefault();//IA2-P1
    }
}
