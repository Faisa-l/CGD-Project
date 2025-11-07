using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerWalkingAudioScript : MonoBehaviour
{
    [SerializeField]
    AudioSource footstepsSound;

    public void Enable()
    {
        if (footstepsSound.isPlaying) return;
        footstepsSound.Play();
    }

    public void Disable()
    {
        if (!footstepsSound.isPlaying) return;
        footstepsSound.Stop();
    }
}
