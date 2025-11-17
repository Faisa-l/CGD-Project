using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class LobbyMenuManager : MonoBehaviour
{
	private static readonly int maxPlayerCount = 4;
	
	[SerializeField] private GameObject[] playerControllerImages; // Offset by 1 (0 = player 1)
	
	private List<Gamepad> currentPlayers = new List<Gamepad>();
	
	#region Lobby
	
	private void Update()
	{
		PlayerJoinedCheck();
	}
	
	private void PlayerJoinedCheck()
	{
		// Check all active gamepads
		// Source - https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.Gamepad.html
		foreach (Gamepad gamepad in Gamepad.all)
		{
			// Has this gamepad just pressed the dpad up button?
			if (gamepad.dpad.up.wasPressedThisFrame)
			{
				// Prevent the same player joining twice
				if (!currentPlayers.Contains(gamepad))
					PlayerJoined(gamepad);
			}
		}
	}
	
	private void PlayerJoined(Gamepad gamepad)
	{
		if (gamepad == null)
		{
			Debug.LogWarning("Called PlayerJoined without passing in a gamepad");
			return;
		}
		
		if (!currentPlayers.Contains(gamepad))
		{
			currentPlayers.Add(gamepad);
			
			// Update amount of player gamepad images shown
			UpdatePlayerGamepadUI();
		}
		else
		{
			Debug.LogWarning("Tried to add the same gamepad more than once: " + gamepad);
		}
	}
	
	private void UpdatePlayerGamepadUI()
	{
		// Loop through all 4 potential player slots
		for (int i = 0; i < maxPlayerCount; i++)
		{
			// Do we have this many players?
			if (currentPlayers.Count > i)
				// Should this player is active
				playerControllerImages[i].SetActive(true);
			else
				// Show this player isn't active
				playerControllerImages[i].SetActive(false);
		}
	}
	
	#endregion Lobby
	
	#region Utility
	
	public void LoadScene(string sceneToLoad)
	{
		if (sceneToLoad != string.Empty)
			SceneManager.LoadScene(sceneToLoad);
	}
	
	#endregion Utility
}