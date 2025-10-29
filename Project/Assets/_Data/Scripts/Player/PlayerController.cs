using UnityEngine;
using System.Collections.Generic;

// This script is for our game's custom features
// The other FirstPersonController is for first person movement code
public class PlayerController : MonoBehaviour
{
	[Header("Settings")]
    [SerializeField][Range(1, 4)] private int playerNumber = 1;
	//[SerializeField] [Min(0)] private float vehicleEnterDistance = 1f;
	// Hold for this many seconds to enter vehicle (like Halo)
	// Also prevents issue where player exiting gets back in straight away (because they are holding button)
	[SerializeField] private float enterVehicleTime = 0.5f;
	
	[Header("UI")]
	[SerializeField] private HudManager hudManager;
	
	// Enter / exiting vehicles
	private List<IDriveable> driveablesInRange = new List<IDriveable>();
	private float currentEnterVehicleTimer = 0;
	
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
		currentEnterVehicleTimer = 0;
	}
	
	private void Update()
	{
		// Is the player trying to enter a vehicle?
		if (Input.GetButton("Fire" + playerNumber))
		{
			if (currentEnterVehicleTimer >= enterVehicleTime)
			{
				if (TryEnterVehicleInRange())
				{
					hudManager.SetVehiclePromptStatus(playerNumber, false);
					gameObject.SetActive(false);
				}
			}
			else
			{
				if (currentEnterVehicleTimer == 0)
				{
					hudManager.SetVehiclePromptText(playerNumber, "Drive Forklift");
				}
				
				currentEnterVehicleTimer += Time.deltaTime;
			}
		}
		else
		{
			currentEnterVehicleTimer = 0;
		}
		
															// Calculate percentage complete
		hudManager.SetVehiclePromptProgress(playerNumber, (currentEnterVehicleTimer / enterVehicleTime));
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
	
	/*private bool VehicleCheck()
	{
		// Is there a vehicle in front of the player?
			
		// Source - https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Physics.SphereCast.html
		RaycastHit hit;
		Vector3 p1 = transform.position + controller.center;

		// Cast a sphere wrapping character controller vehicleEnterDistance meters forward
		// to see if it is about to hit anything.
		if (Physics.SphereCast(p1, controller.height / 2, transform.forward, out hit, vehicleEnterDistance))
		{
			IDriveable drivable = hit.transform.GetComponent<IDriveable>();
				
			if (drivable != null)
			{
				Debug.Log("Driveable found: " + hit.transform.name);
				return true;
			}
			else
			{
				Debug.Log(hit.transform.name + " isn't driveable");
				return false;
			}
		}
		else
		{
			Debug.Log("Nothing there");
			return false;
		}
	}*/
	
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