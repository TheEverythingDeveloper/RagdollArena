using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public float velocity;
    private void Update()
    {
        transform.Rotate(Vector3.up, velocity * Time.deltaTime);
    }
}
