using UnityEngine;
using System.Collections.Generic;

// This script is for our game's custom features
// The other FirstPersonController is for first person movement code
public class PlayerController : MonoBehaviour
{
	[Header("Settings")]
    [SerializeField][Range(1, 4)] private int playerNumber = 1;
	
	[Header("UI")]
	[SerializeField] private HudManager hudManager;
	
	// Enter / exiting vehicles
	private List<IDriveable> driveablesInRange = new List<IDriveable>();
	// Prevent exiting vehicle and immediately re-entering because Input.GetButtonDown is still being called
	private static readonly float enterVehicleDelay = 0.01f;
	private float currentEnterVehicleDelay = 0.0f;
	
	// Cache
	private CharacterController controller;
	
	private void Start()
	{
		controller = GetComponent<CharacterController>();
	}
	
	private void OnEnable()
	{
		// Reset
		driveablesInRange = new List<IDriveable>();
		
		// Prevent exiting vehicle and immediately re-entering because Input.GetButtonDown is still being called
		currentEnterVehicleDelay = enterVehicleDelay;
	}
	
	private void Update()
	{
		// Ready to enter vehicle
		if (currentEnterVehicleDelay <= 0)
		{
			// Is the player trying to enter a vehicle?
			if (Input.GetButtonDown("Fire" + playerNumber))
			{
				if (TryEnterVehicleInRange())
				{
					hudManager.SetVehiclePromptStatus(playerNumber, false);
					gameObject.SetActive(false);
				}
			}
			else
			{
				hudManager.SetVehiclePromptText(playerNumber, "Drive Forklift");
			}
		}
		// Not ready to enter vehicle
		else
		{
			currentEnterVehicleDelay -= Time.deltaTime;
		}
		
															// Calculate percentage complete
		hudManager.SetVehiclePromptProgress(playerNumber/*, (currentEnterVehicleTimer / enterVehicleTime)*/);
	}
	
	private bool TryEnterVehicleInRange()
	{
		if (driveablesInRange.Count > 0)
		{
			driveablesInRange[0].TryEnterVehicle(this);
			return true;
		}
		else
		{
			return false;
		}
	}
	
	public void AddVehicleInRange(IDriveable driveable)
	{
		// Don't add twice
		if (!driveablesInRange.Contains(driveable))
		{
			driveablesInRange.Add(driveable);
			
			hudManager.SetVehiclePromptStatus(playerNumber, true);
		}
		else
		{
			Debug.LogWarning("Tried to add a driveable that is already in driveablesInRange");
		}
	}
	
	public void RemoveVehicleInRange(IDriveable driveable)
	{
		if (driveablesInRange.Contains(driveable))
		{
			driveablesInRange.Remove(driveable);
			
			if (driveablesInRange.Count <= 0)
			{
				hudManager.SetVehiclePromptStatus(playerNumber, false);
			}
		}
		else
		{
			Debug.LogWarning("Tried to remove driveable that isn't in driveablesInRange.");
		}
	}
	
	public int GetPlayerNumber()
	{
		return playerNumber;
	}
}