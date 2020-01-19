using UnityEngine;

public class Damageable : MonoBehaviour
{
    IDamageable _iDamageable;

    private void Awake()
    {
        _iDamageable = GetComponentInParent<IDamageable>();
    }

    public void Damage(float d)
    {
        _iDamageable.Damage(d);
    }
}
