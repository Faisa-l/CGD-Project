using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

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
	[SerializeField] private float speed = 1f;
	
	[Header("UI")]
	[SerializeField] private HudManager hudManager;
	
	// Enter / exiting vehicles
	private List<IDriveable> driveablesInRange = new List<IDriveable>();
	private float currentEnterVehicleTimer = 0;
	
	// Cache
	private CharacterController controller;

	[SerializeField] GameObject camera_manager;

    [Tooltip("Animator component for the player model")]
    public Animator anim;

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
		CameraRotation();
		move();

		// Is the player trying to enter a vehicle?
		if (UnityEngine.Input.GetButton("Mouse Left" + playerNumber))
		{
			if (currentEnterVehicleTimer >= enterVehicleTime)
			{
				camera_manager.GetComponent<CameraManager>().changePerspective3rd(playerNumber);

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

    private void move()
    {
		Vector3 inputDirection = new Vector3(0,0,0);

		//get the user input
        if (UnityEngine.Input.GetButton("Horizontal" + playerNumber))
		{ 
			inputDirection.x = UnityEngine.Input.GetAxis("Horizontal" + playerNumber) < 0 ? -1 : 1;
		}

        if (UnityEngine.Input.GetButton("Vertical" + playerNumber))
        {
            inputDirection.z = UnityEngine.Input.GetAxis("Vertical" + playerNumber) < 0 ? -1 : 1;
        }

		Vector3 direction = new(0,0,0);

		//get the angle of the camera in radians
		float angle = transform.rotation.eulerAngles.y * Mathf.PI/ 180.0f;

		//rotate the direction to match with the camera direction
		direction.x = inputDirection.x * Mathf.Cos(angle) + inputDirection.z * Mathf.Sin(angle);
		direction.z = inputDirection.z * Mathf.Cos(angle) - inputDirection.x * Mathf.Sin(angle);

        direction.Normalize();

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (direction.magnitude != 0)
        {
            // walk animation
            if (anim)
                anim.SetBool("IsMoving", true);
        }
        else
        {
            // idle animation
            if (anim)
                anim.SetBool("IsMoving", false);
        }

        // move the player
        controller.Move(direction * (speed * Time.deltaTime));
    }

    private void CameraRotation()
    {
		Vector2 camera_rotation = new(0,0);

		camera_rotation.x = UnityEngine.Input.GetAxis("Mouse Y");
        camera_rotation.y = UnityEngine.Input.GetAxis("Mouse X");

		transform.eulerAngles += new Vector3(camera_rotation.x, camera_rotation.y, 0);
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