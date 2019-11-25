using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeDrunk : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        var drunk = collision.gameObject.GetComponent<IDrunk>();
        if (drunk != null)
        {
            drunk.DrunkEffectActive();
        }
        Destroy(gameObject);
    }
}
