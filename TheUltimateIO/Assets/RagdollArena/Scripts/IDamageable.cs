using UnityEngine;

public interface IDamageable 
{
    void Damage(Vector3 origin, float damage);
    void Explosion(Vector3 origin, float force);
}
