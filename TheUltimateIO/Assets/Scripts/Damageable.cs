using UnityEngine;
using Character;

public class Damageable : MonoBehaviour, IDamageable
{
    CharacterModel _characterModel;

    private void Awake()
    {
        _characterModel = GetComponentInParent<CharacterModel>();
    }

    public void Damage(Vector3 origin, float d)
    {
        _characterModel.Damage(origin, d);
    }

    public void Explosion(Vector3 origin, float force)
    {
        throw new System.NotImplementedException();
    }
}
