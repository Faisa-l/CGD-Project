using UnityEngine;

public class ForkliftReverseAudio : MonoBehaviour
{
    [SerializeField]
    AudioSource reversingSound;

    public void Enable()
    {
        if (reversingSound.isPlaying) return;
        reversingSound.Play();
    }

    public void Disable()
    {
        if (!reversingSound.isPlaying) return;
        reversingSound.Stop();
    }
}
