using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QMon9Controller : QMon1Controller
{
    protected override void NearCollidersExtraMethod(List<Collider> nearColliders)
    {
        nearColliders.Select(x =>
        {
            if (x.GetComponent<Rigidbody>())
            {
                x.GetComponent<Rigidbody>().AddExplosionForce(pushForce * -4, transform.position, viewRadius);
            }
            return x;
        }).ToList();
    }
}
