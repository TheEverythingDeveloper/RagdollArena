using Photon.Pun;
using UnityEngine;
using Character;

namespace Weapons {
    public class ShieldProtect : MonoBehaviour, IDamageable
    {
        public CharacterModel character;
        public WeaponsManager manager;
        public float maxDamageProtect;
        public ParticleSystem particlesShield;

        public void Damage(Vector3 origin, float damage)
        {
            var dir = (transform.position - origin) * damage;
            character.photonView.RPC("RPCDamageShield", RpcTarget.All, dir.x, dir.y, dir.z);

            if (damage > maxDamageProtect) character.Damage(origin, damage - maxDamageProtect);
        }

        public void Explosion(Vector3 origin, float force)
        {
            throw new System.NotImplementedException();
        }

    }
}