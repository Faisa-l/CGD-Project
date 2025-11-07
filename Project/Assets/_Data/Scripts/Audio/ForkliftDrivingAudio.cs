using UnityEngine;

public class ForkliftDrivingAudio : MonoBehaviour
{
    [SerializeField]
    AudioSource drivingSound;

    public void Enable()
    {
        if (drivingSound.isPlaying) return;
        drivingSound.Play();
    }

    public void Disable()
    {
        if (!drivingSound.isPlaying) return;
        drivingSound.Stop();
    }
}
