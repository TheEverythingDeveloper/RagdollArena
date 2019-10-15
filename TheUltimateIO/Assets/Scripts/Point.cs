using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    public int points;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == Layers.CHARACTER || other.gameObject.layer == Layers.PLAYER)
        {
            other.GetComponentInParent<Character.CharacterModel>().AddPoint(points);
        }
    }
}
