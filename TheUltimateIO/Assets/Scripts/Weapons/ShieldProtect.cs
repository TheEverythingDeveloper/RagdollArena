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
            character.rb.AddForce((transform.position - origin) * damage, ForceMode.Impulse);
            character.photonView.RPC("RPCDamageShield", RpcTarget.All);

            if (damage > maxDamageProtect) character.Damage(origin, damage - maxDamageProtect);
        }
        [PunRPC] void RPCDamageShield() { particlesShield.Play(); }

        public void Explosion(Vector3 origin, float force)
        {
            throw new System.NotImplementedException();
        }

    }
}