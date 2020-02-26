using Photon.Pun;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviourPun
    {
        public AudioClip[] sounds;
        public AudioSource audioSounds;

        public void PlayRandomSound()
        {
            var soundRandom = Random.Range(0, sounds.Length);
            audioSounds.Stop();
            audioSounds.clip = sounds[soundRandom];
            photonView.RPC("RPCPlaySound", RpcTarget.All);
        }

        public void PlaySound(int sound)
        {
            audioSounds.Stop();
            audioSounds.clip = sounds[sound];
            photonView.RPC("RPCPlaySound", RpcTarget.All);
        }

        [PunRPC] void RPCPlaySound() { audioSounds.Play(); }
    }
}