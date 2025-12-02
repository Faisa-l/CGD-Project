using UnityEngine;

public class OptionsMenuManager : MonoBehaviour
{
	// Used for back button click
	public void SaveSettings()
	{
		SaveManager.instance.Save();
	}
}