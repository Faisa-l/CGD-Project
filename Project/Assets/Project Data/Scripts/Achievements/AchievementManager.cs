using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AchievementManager : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("Achievements in the game")]
	[SerializeField] private Achievement[] achievements;
	
	[Header("Sound")]
	[SerializeField] private AudioSource audioSrc;
	[SerializeField] private AudioClip unlockedSound;
	
	// Singleton
	private static AchievementManager instance;
	
	/// <summary>
	/// Setup a Singeton so there is only one SaveManager that can be acccessed in any other script
	/// </summary>
	private void Awake()
	{
		// Check for existing Singleton
		if (instance == null)
		{
			// Assign Singleton
			instance = this;
			
			// Survive scene change
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			// There can only be one save manager
			Destroy(this);
			
			Debug.LogWarning("Tried to create a second Achievement Manager. Only one is allowed.");
		}
	}
	
	private void Start()
	{
		// If an achievement doesn't have save data, add it
		SaveManager.onLoaded.AddListener(InitSaveData);
	}
	
	private void InitSaveData()
	{
		if (!SaveManager.instance)
		{
			Debug.LogWarning("Achievement Manager couldn't find Save Manager");
			return;	
		}
		
		
		// Loop through all achievements in the Save Data
		// Make sure all achievements have save data setup and if not set their default values
		foreach (var achievement in achievements)
		{
			bool existingSaveData = false;
			
			// Is there any save data for this achievement?
			if (SaveManager.instance.currentSaveData.achievements != null)
			{
				foreach (var saveData in SaveManager.instance.currentSaveData.achievements.achievementData)
				{
					if (saveData.name == achievement.name)
					{
						// There is no need to keep looking for this achievement
						existingSaveData = true;
						break;
					}
				}
			}
			
			if (!existingSaveData)
			{
				AchievementData data = new AchievementData();
				data.name = achievement.name;
				
				// Create new default data for this achievement (progress = 0, unlocked = false)
				// Only name needs to be set
				SaveManager.instance.currentSaveData.achievements.achievementData.Add(data);
			}
		}
	}
}