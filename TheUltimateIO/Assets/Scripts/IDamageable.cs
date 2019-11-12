using UnityEngine;

public interface IDamageable 
{
    void Damage(float damage);
    void Explosion(Vector3 origin, float force);
}
