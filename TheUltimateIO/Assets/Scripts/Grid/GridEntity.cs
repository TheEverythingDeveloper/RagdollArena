using System;
using UnityEngine;

public class GridEntity : MonoBehaviour
{
	public event Action<GridEntity> OnMove = delegate {};

    private void Start()
    {
        FindObjectOfType<SpatialGrid>().CallSpatialGrid(this);
    }
    void Update() {
	    OnMove(this);
	}
}
