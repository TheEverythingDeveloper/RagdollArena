using System;
using UnityEngine;

public class GridEntity : MonoBehaviour
{
	public event Action<GridEntity> OnMove = delegate {};
    SpatialGrid spatialGrid;
    private void Start()
    {
        spatialGrid = FindObjectOfType<SpatialGrid>();
        spatialGrid.CallSpatialGrid(this);
    }
    void Update() {
	    OnMove(this);
	}

    public void DestroyGameObject()
    {
        spatialGrid.DestroyCubeGrid(this);
        Destroy(gameObject);
    }
}
