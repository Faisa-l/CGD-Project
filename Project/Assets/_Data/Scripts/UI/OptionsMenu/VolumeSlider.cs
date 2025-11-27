using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

/// <summary>
/// Slider UI element that can control either sound effects or music volume.
/// The slider needs this script attached and OnValueChanged needs this script's SetValue assigned.
/// Then it will set the volume of the Audio Mixer group.
/// If you create a new Audio Mixer Group you need to expose the volume variable.
/// </summary>
public class VolumeSlider : MonoBehaviour
{
	[Header("Cache")]
	[Tooltip("Enum of the audio type you want this slider to set.")]
    [SerializeField] VolumeType volumeType;
	[Tooltip("Reference to the AudioMixer asset in the asset folder (such as the main one in '/_Data/AudioSFX/MainAudioMixer'.")]
    [SerializeField] AudioMixer audioMixer;
	[Tooltip("Reference to the UI slider in the scene.")]
    [SerializeField] Slider slider;

    void Start()
    {
        // Init setup
		// TODO load from save code
        slider.value = GetValue(volumeType);
    }

	/// <summary>
	/// Set the float value of the audio type specified in the enum variable volumeType
	/// Can be called once manually such as seting up the value in Start()
	/// Can be used with a slider and assigning in SetValue to be changed when the slider does (the value is automatically passed through)
	/// </summary>
    public void SetValue()
    {
		// Prevent bug where 0 makes the sound full volume
		// Instead, make it a value too quiet to hear
		if (slider.value == 0)
			slider.value = 0.0001f;
		
        switch (volumeType)
		{
			case VolumeType.SFX:
				// The slider is linear and the AudioMixer is logarithmic - see JLF comment - https://forum.unity.com/threads/changing-audio-mixer-group-volume-with-ui-slider.297884/
				// Change for this playthrough
				audioMixer.SetFloat("SFX", Mathf.Log(slider.value) * 20);
				
				// Persist for future playthroughs
				// TODO add save code here
				//SaveData.SetSfxVolume(slider.value); // Persist for future playthroughs
				break;
			
            case VolumeType.MUSIC:
				// Change for this playthrough
                audioMixer.SetFloat("Music", Mathf.Log(slider.value) * 20);
				
				// Persist for future playthroughs
				// TODO add save code here
                //SaveData.SetMusicVolume(slider.value);
				break;
				
            default:
               Debug.LogWarning("Invalid volumeType: " + volumeType);
				break;
        }
    }

	/// <summary>
	/// Get the current float value of the specified audio type
	/// -1 is a returned error value
	/// </summary>
    public static float GetValue(VolumeType volumeType)
    {
        switch (volumeType)
        {
            case VolumeType.SFX:
				// TODO get from save system
                //return SaveData.GetSfxVolume();
				return 1.0f;
            case VolumeType.MUSIC:
				// TODO get from save system
                //return SaveData.GetMusicVolume();
				return 1.0f;
            default:
                return -1; // Return error value
        }
    }

    public enum VolumeType { SFX, MUSIC }
}