using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {

    public static AudioManager Instance;
    public AudioMixerGroup BGAudioMaster, SoundAudioMaster;
    public List<AudioSource> audioPlayers = new List<AudioSource>(); //0, loop music ,1 = musica, 2.3 = audios

    /*Musicas:
     * 0) 
     * 1) 
     * 2) 
    */
    /*Sonidos:
     * 0) 
     * 1) 
     * 2)
    */

    public List<AudioClip> musicAudioClips, soundAudioClips = new List<AudioClip>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        audioPlayers.Add(gameObject.AddComponent<AudioSource>());
        audioPlayers.Add(gameObject.AddComponent<AudioSource>());
        audioPlayers.Add(gameObject.AddComponent<AudioSource>());
        audioPlayers.Add(gameObject.AddComponent<AudioSource>());
        audioPlayers[0].outputAudioMixerGroup = BGAudioMaster;
        audioPlayers[0].loop = true;
        audioPlayers[0].volume = 0.4f;
        audioPlayers[1].outputAudioMixerGroup = BGAudioMaster;
        audioPlayers[2].outputAudioMixerGroup = SoundAudioMaster;
        audioPlayers[3].outputAudioMixerGroup = SoundAudioMaster;
        StartPlayingMusic(0,0); //Background Noise
    }

    /// <summary>
    /// Reproduce un sonido en un audio player
    /// </summary>
    /// <param name="audioPlayerID"> ID del Audio player para reproducirlo aca </param>
    /// <param name="id"> ID del audio para reproducir </param>
    public void StartPlayingMusic(int audioPlayerID, int id)
    {
        audioPlayers[audioPlayerID].clip = musicAudioClips[id];
        audioPlayers[audioPlayerID].Play();
    }

    public void StartPlayingSFX(int audioPlayerID, int id)
    {
        audioPlayers[audioPlayerID].clip = soundAudioClips[id];
        audioPlayers[audioPlayerID].Play();
    }

    public void StopPlaying(int audioPlayerID)
    {
        audioPlayers[audioPlayerID].Stop();
    }
}
