using UnityEngine;

// Used so the first person player can keep track of what forklifts they can enter
// SphereCast inside player controller script isn't used because having it in Update is too inefficient
// The player controller doesn't simply have another controller and a SphereTrigger added to avoid conflicting with the FirstPersonController script
// The UI will respond to this with a "Press X to enter forklift" prompt
public class DriveableNotifyTrigger : MonoBehaviour
{
	// Unity doesn't support interfaces in the inspector :(
	//[SerializeField] private IDriveable driveable;
	[SerializeField] private ForkliftController forklift;
	
	private IDriveable driveable;
	
	private void Start()
	{
		driveable = forklift as IDriveable;
	}
	
	private void OnTriggerEnter(Collider other)
	{
		// Is it a player?
		if (other.CompareTag("Player"))
		{
			PlayerController playerController = other.transform.GetComponent<PlayerController>();
			
			if (playerController)
			{
				playerController.AddVehicleInRange(driveable);
			}
			else
			{
				Debug.LogWarning("Couldn't find player controller on " + other.transform.name);
			}
		}
	}
	
	private void OnTriggerExit(Collider other)
	{
		// Is it a player?
		if (other.CompareTag("Player"))
		{
			PlayerController playerController = other.transform.GetComponent<PlayerController>();
			
			if (playerController)
			{
				playerController.RemoveVehicleInRange(driveable);
			}
			else
			{
				Debug.LogWarning("Couldn't find player controller on " + other.transform.name);
			}
		}
	}
}