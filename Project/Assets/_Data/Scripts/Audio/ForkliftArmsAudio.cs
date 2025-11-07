using UnityEngine;

public class ForkliftArmsAudio : MonoBehaviour
{
    [SerializeField]
    AudioSource armsSound;

    public void Enable()
    {
        if (armsSound.isPlaying) return;
        armsSound.Play();
    }

    public void Disable()
    {
        if (!armsSound.isPlaying) return;
        armsSound.Stop();
    }
}
