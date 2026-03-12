using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelGroupPanel : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("Level Group ScriptableObject in Project Folder this level group panel will represent")]
	[SerializeField] private LevelGroup levelGroup;
	[Tooltip("Reference in the scene to the individual level container for this level group")]
	[SerializeField] private GameObject levelContainer;
	[Tooltip("Reference in the scene to the group container for this level group")]
	[SerializeField] private GameObject levelGroupContainer;
	
	[Header("Cache")]
	[SerializeField] private TMP_Text levelGroupNameText;
	[SerializeField] private Image levelGroupScreenshot;
	
	private void Start()
	{
		SetupUI();
	}
	
	private void SetupUI()
	{
		if (levelGroup == null)
		{
			Debug.Log("Level Group not set in Level Group Panel");
			return;
		}
		
		levelGroupNameText.text = levelGroup.GetDisplayName();
		
		if (levelGroupScreenshot)
			levelGroupScreenshot.sprite = levelGroup.GetScreenshot();
	}
	
	// Use for button onclicked event
	public void Disable()
	{
		if (levelGroup == null)
		{
			Debug.LogWarning("Level Group not set in Level Group Panel");
			return;	
		}
		
		if (levelContainer == null)
		{
			Debug.LogWarning("Tried to load scene without a name in Level Group Panel");
			return;	
		}
		
        levelContainer.SetActive(true);
		levelGroupContainer.SetActive(false);
	}
	
	public void Enable()
	{
        levelContainer.SetActive(false);
		levelGroupContainer.SetActive(true);
	}
}