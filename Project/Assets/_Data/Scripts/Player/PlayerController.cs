using Interaction;
using StarterAssets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using TMPro;


// This script is for our game's custom features
// The other FirstPersonController is for first person movement code
public class PlayerController : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField][Range(1, 4)] private int playerNumber = 1;

	Gamepad playerGamepad = null;

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
	[SerializeField] float cameraYOffset = 1.622f;

	[SerializeField] InputActionReference lifting_action;
	[SerializeField] InputActionReference dropping_action;

	[SerializeField] TMP_Text promptText;

	[Header("Events")]
	[SerializeField] private UnityEvent onEnteredVehicle;

	private float enter_vehicle_start_height;
	private bool lift_enabled = false;




	public void setPlayerGamepad(Gamepad pad)
	{
		playerGamepad = pad;
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
		if(playerGamepad == null)
		{
			Debug.LogWarning("Player has no Gamepad connected");
			return;
		}


        if (playerGamepad.buttonEast.wasPressedThisFrame) 
		{
			if (!lift_enabled)
			{
				Lift();
                lift_enabled = true;

            }
			else
			{
				Drop();
				lift_enabled = false;

            }
		}

		if(driving)
		{
			GetComponent<InteractableControl>().interactDistance = 0.0f;
			
		}
		else
		{
            GetComponent<InteractableControl>().interactDistance = 3.0f;
        }
    }

    public void OnForklift_Interact()
    {
		if (driving)
		{		
			Debug.Log("Picking Up/Dropping Forklift");
			current_forklift.GetComponentInChildren<FloatPickup>().PickUpSelectedForklift();
			
			Debug.Log("Pressed Interact Forklift");
			current_forklift.GetComponentInChildren<FloatPickup>().PickUpSelected();
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
				GetComponent<Collider>().enabled = false;
			}

            GetComponent<FirstPersonController>().enabled = true;

        }
		else if(current_forklift != null)
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
			camera.transform.GetChild(0).transform.localPosition = new Vector3(0f, cameraYOffset, 0f);

            GetComponent<CharacterController>().enabled = true;

			GetComponent<Collider>().enabled = true;

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

    private void EnterVehicle()
	{
        if (TryEnterVehicleInRange())
        {

			enter_vehicle_start_height = camera.transform.position.y;
            model.SetActive(false);
			driving = true;
			
			promptText.text = "";
			
			if (onEnteredVehicle != null)
				onEnteredVehicle.Invoke();
        }
    }

	public void drive(StarterAssetsInputs input)
	{
		current_forklift.move(input);
	}
	
	public void cameraDrive(float rotation_velocity)
	{
		camera.transform.position = new Vector3(camera.transform.position.x, enter_vehicle_start_height, camera.transform.position.z);
		Debug.Log(camera.transform.position);
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

			// Camera setup
			current_forklift = (ForkliftController)driveablesInRange[0];
            camera.transform.parent = current_forklift.transform;
			SetCameraPosition(false);
			//Transform forklift_camera_root = current_forklift.getCameraRoot();
            //camera.transform.SetPositionAndRotation(forklift_camera_root.transform.position, forklift_camera_root.transform.rotation);
            //camera.transform.LookAt(current_forklift.getLookAtTransform());
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

	// Sets the position of the camera to either forward of reverse
	public void SetCameraPosition(bool toReverse)
	{
        Transform atForward = current_forklift.CameraForwardTransform;
		Transform atReverse = current_forklift.CameraReverseTransform;

		/// TODO:
		/// - Needs to lerp or tween between the two positions
		/// - It may be desirable to instead set these positions based on to opposite locations of a circle
		/// - That way the translation will be a rotation around this circle (Transform.RotateAround)
		/// - How to do animation... idk :P
		if (toReverse)
		{
            camera.transform.SetPositionAndRotation(atReverse.transform.position, atReverse.transform.rotation);
        }
		else
		{
            camera.transform.SetPositionAndRotation(atForward.transform.position, atForward.transform.rotation);
		}
 
		camera.transform.GetChild(0).transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        camera.transform.LookAt(current_forklift.getLookAtTransform());
    }
	
	public Gamepad GetPlayerGamepad()
	{
		return playerGamepad;
	}
}