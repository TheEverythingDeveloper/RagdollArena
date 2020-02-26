using Photon.Pun;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public AudioClip[] sounds;
        public AudioSource audioSounds;

        public void PlaySound()
        {
            var soundRandom = Random.Range(0, sounds.Length);
            audioSounds.Stop();
            audioSounds.clip = sounds[soundRandom];
        }

        [PunRPC] void RPCPlaySound() { audioSounds.Play(); }
    }
}