using Photon.Pun;
using UnityEngine;
using Character;
using Audio;

namespace Weapons {
    public class ShieldProtect : MonoBehaviour, IDamageable
    {
        public CharacterModel character;
        public WeaponsManager manager;
        public float maxDamageProtect;
        public ParticleSystem particlesShield;
        public AudioManager soundShield;

        public void Damage(Vector3 origin, float damage)
        {
            var dir = (transform.position - origin) * damage;
            character.photonView.RPC("RPCDamageShield", RpcTarget.All, dir.x, dir.y, dir.z);
            soundShield.PlayRandomSound();
            if (damage > maxDamageProtect) character.Damage(origin, damage - maxDamageProtect);
        }

        public void Explosion(Vector3 origin, float force)
        {
            throw new System.NotImplementedException();
        }

    }
}