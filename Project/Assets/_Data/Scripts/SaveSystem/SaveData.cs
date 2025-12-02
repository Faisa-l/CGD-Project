using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make it appear in inspector
[System.Serializable]
/// <summary>
/// Template for save data file
/// Save data is broken down into sections to make finding variables easier
/// SaveManager will try and load an existing save data from disk when the game starts. If it does these variables will be filled out.
/// If there is no existing save then the default values will be used throughout the game
/// </summary>
public class SaveData
{
	// Containers for variables
	// Grouped to make it more manageable as we add more
	public AudioSaveData audio;
}

#region Options menu

[System.Serializable]
// Options audio menu variables
public class AudioSaveData
{
	public float sfxVolume = 1.0f;
	public float musicVolume = 0.3f;
}

#endregion Options menu