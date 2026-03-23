using UnityEngine;
using UnityEngine.Audio;

public class ParticleEffectWithSound : ParticleEffect
{
    public AudioSource soundSource;
    public AudioMixerGroup mixerGroup;

    private void Awake()
    {
        if (soundSource == null) soundSource = GetComponent<AudioSource>();
        if (mixerGroup == null) Debug.LogWarning("No mixer group set for particle effect with sound");
        else soundSource.outputAudioMixerGroup = mixerGroup;
    }

    public override void Play()
    {
        // Debug.Log("Playing");
        soundSource.Play();
        base.Play();
    }
}
