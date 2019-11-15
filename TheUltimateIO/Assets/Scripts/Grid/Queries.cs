using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class Queries : MonoBehaviour
{
    public bool isBox;
    public float radius = 20f;
    public SpatialGrid targetGrid;
    public float width = 15f;
    public float height = 30f;

    private void Awake()
    {
        targetGrid = FindObjectOfType<SpatialGrid>();
    }

    public IEnumerable<GridEntity> Query()
    {
        if (isBox)
        {
            var h = height * 0.5f;
            var w = width * 0.5f;
            return targetGrid.Query(
                transform.position + new Vector3(-w, 0, -h),
                transform.position + new Vector3(w, 0, h),
                x => true);
        }
        else
        {
            return targetGrid.Query(
                transform.position + new Vector3(-radius, 0, -radius),
                transform.position + new Vector3(radius, 0, radius),
                x => {
                    var position2d = x - transform.position;
                    position2d.y = 0;
                    return position2d.sqrMagnitude < radius * radius;
                });
        }
    }

    void OnDrawGizmos()
    {
        if (targetGrid == null)
            return;

        Gizmos.color = Color.cyan;
        if (isBox)
            Gizmos.DrawWireCube(transform.position, new Vector3(width, 0, height));
        else
        {
            Gizmos.matrix *= Matrix4x4.Scale(Vector3.forward + Vector3.right);
            Gizmos.DrawWireSphere(transform.position, radius);
        }

    }

    public IEnumerable<GridEntity> ObjectsInGrid()
    {
        return Query(); 
    }
}
