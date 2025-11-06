using StarterAssets;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static Unity.Burst.Intrinsics.X86;
using static UnityEditor.PlayerSettings;

// This script is for our game's custom features
// The other FirstPersonController is for first person movement code
public class PlayerController : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField][Range(1, 4)] private int playerNumber = 1;
	[SerializeField] private float enterVehicleTime = 0.5f;

	//[Header("UI")]
	//[SerializeField] private HudManager hudManager;

	// Enter / exiting vehicles
	private List<IDriveable> driveablesInRange = new List<IDriveable>();

	[Space(20)]
	[Header("Player Model")]
	[SerializeField] GameObject model;

	private ForkliftController current_forklift = null;

	[SerializeField] GameObject camera;
	[SerializeField] Transform player_camera_root;

	[SerializeField][Range(0, 1)] float max_rotation = 0.3f;

	public bool driving { get; private set; }

    private void Start()
	{
        driving = false;
    }

    private void OnEnable()
	{
		// Reset
		driveablesInRange = new List<IDriveable>();
	}

    private void OnDisable()
    {

    }

    private void Update()
    {

    }

    public void OnInteract()
    {
		if(!driving)
		{ 
			EnterVehicle();
		}
		else
		{
			GetComponent<CharacterController>().enabled = false;

			current_forklift.interact();
			model.SetActive(true);
			driving = false;

			Transform exit_transform = current_forklift.getExitTransform();

			transform.position = exit_transform.position;
			transform.rotation = exit_transform.rotation;

			camera.transform.position = player_camera_root.position;
			camera.transform.rotation = player_camera_root.rotation;
			camera.transform.parent = gameObject.transform;

			GetComponent<CharacterController>().enabled = true;

            current_forklift = null;
		}
    }

	public void OnLift()
	{
		if(driving)
		{
			current_forklift.Lift();
		}
	}

	public void OnDrop()
	{
		if(driving)
		{
			current_forklift.Drop();
		}
	}

    private void EnterVehicle()
	{

        if (TryEnterVehicleInRange())
        {
            model.SetActive(false);
			driving = true;
        }
    }

	public void drive(StarterAssetsInputs input)
	{
		current_forklift.move(input);
	}

	public void cameraDrive(float rotation_velocity)
	{
		if(camera.transform.localRotation.y < 0.3f && rotation_velocity > 0)
		{ 
			camera.transform.Rotate(Vector3.up * rotation_velocity); 
		}
        else if (camera.transform.localRotation.y > -0.3f && rotation_velocity < 0)
        {
            camera.transform.Rotate(Vector3.up * rotation_velocity);
        }
    }

    private bool TryEnterVehicleInRange()
	{
		if (driveablesInRange.Count > 0)
		{
			driveablesInRange[0].TryEnterVehicle(this);
			current_forklift = (ForkliftController)driveablesInRange[0];

			Transform forklift_camera_root = current_forklift.getCameraRoot();
			camera.transform.SetPositionAndRotation(forklift_camera_root.transform.position, forklift_camera_root.transform.rotation);
			camera.transform.parent = current_forklift.transform;
			
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
			
			//hudManager.SetVehiclePromptStatus(playerNumber, true);
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
				//hudManager.SetVehiclePromptStatus(playerNumber, false);
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
}