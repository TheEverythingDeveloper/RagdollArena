using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Audio
{
    public class MultipleAudioManager : MonoBehaviour
    {
        public List<AudioClip[]> sounds = new List<AudioClip[]>();
        public AudioSource audioSounds;

        public void PlaySound(int list)
        {
            var soundRandom = Random.Range(0, sounds[list].Length);
            audioSounds.Stop();
            audioSounds.clip = sounds[list][soundRandom];
        }

        [PunRPC] void RPCPlaySound() { audioSounds.Play(); }
    }
}