using StarterAssets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
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

	[SerializeField] InputActionReference lifting_action;
	[SerializeField] InputActionReference dropping_action;

	private float enter_vehicle_start_height;

	public void setPlayerNumber(int num)
	{
		playerNumber = num;
	}

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
        if(Gamepad.all[playerNumber - 1].rightShoulder.IsPressed())
		{
			Lift();
		}
		else if(Gamepad.all[playerNumber - 1].leftShoulder.IsPressed())
		{
			Drop();
		}
		else
		{
			cancelLift();
		}
    }

    public void OnInteract()
    {
		if(!driving)
		{ 
			EnterVehicle();

			if(current_forklift!=null)
			{ 
				camera.transform.LookAt(current_forklift.getLookAtTransform()); 
			}
        }
		else
		{
			GetComponent<CharacterController>().enabled = false;

			current_forklift.interact();
			model.SetActive(true);
			driving = false;

			Transform exit_transform = current_forklift.getExitTransform();

			transform.position = exit_transform.position;
			transform.eulerAngles = new Vector3(0,exit_transform.rotation.y,0);

			camera.transform.position = player_camera_root.position;
			camera.transform.rotation = player_camera_root.rotation;
			camera.transform.parent = gameObject.transform;

            GetComponent<CharacterController>().enabled = true;

            current_forklift = null;
		}
    }

	public void Lift()
	{
		if(driving)
		{
			current_forklift.Lift();
		}
	}

	public void Drop()
	{
        if (driving)
		{
			current_forklift.Drop();
		}
	}

	private void cancelLift()
	{
        if (driving)
		{
			current_forklift.cancelLift();
		}
	}

    private void EnterVehicle()
	{
        if (TryEnterVehicleInRange())
        {
			enter_vehicle_start_height = camera.transform.position.y;
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
		camera.transform.position = new Vector3(camera.transform.position.x, enter_vehicle_start_height, camera.transform.position.z);
		camera.transform.RotateAround(current_forklift.transform.position, Vector3.up, rotation_velocity);
		camera.transform.LookAt(current_forklift.getLookAtTransform());
    }

    private bool TryEnterVehicleInRange()
	{
		if (driveablesInRange.Count > 0)
		{
			if (driveablesInRange[0] == null)
			{
				Debug.LogError("Closest vehicle is null");
				return false;
			}

			if(!driveablesInRange[0].TryEnterVehicle(this))
			{
				Debug.LogWarning("Failed to enter vehicle");
				return false;
			}

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