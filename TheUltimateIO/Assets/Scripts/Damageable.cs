using UnityEngine;

public class Damageable : MonoBehaviour
{
    IDamageable _iDamageable;

    private void Awake()
    {
        _iDamageable = GetComponentInParent<IDamageable>();
    }

    public void Damage(Vector3 origin, float d)
    {
        _iDamageable.Damage(origin, d);
    }
}
