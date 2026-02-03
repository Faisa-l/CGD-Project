using StarterAssets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// Source - https://www.youtube.com/watch?v=17j-u7z4wlE
// making a game in one hour (forklift simulation) - Flutter With Gia
public class ForkliftController : MonoBehaviour, IDriveable
{
    [Header("Lift")]
    [SerializeField] private Transform lift;
    [SerializeField] private float liftSpeed = 1.0f;
    [SerializeField] private float minimumLiftPosition = 2.4f;
    [SerializeField] private float maxLiftPosition = 9.5f;

	[Header("UI")]
	[SerializeField] private HudManager hudManager;

    [Header("Other References")]
    [SerializeField] private Transform steeringWheel;
	[SerializeField] private SkinnedMeshRenderer playerMesh; // This data type so we can change the skin to match player getting in after alpha
	[SerializeField] private Transform exitTransform;
    [SerializeField] private Transform look_at_transform;

    [Header("Events")]
    [SerializeField]
    UnityEvent onVehichleEnter;
    [SerializeField]
    UnityEvent onVehichleExit;

    private bool isLiftGoingUp = false;
    private bool isLiftGoingDown = false;

	private PlayerController driver;

    [SerializeField] private Transform cameraForwardPos;
    [SerializeField] private Transform cameraReversePos;
    Vector3 rootForward, rootReverse;
    Vector3 lookAtPosition;
    Vector3 cameraReverseOrigin;
    Vector3 cameraForwardOrigin;
    float maxCameraReverseDist;
    float maxCameraForwardDist;

    private Rigidbody rb;

    private AudioEnabler audio_enabler;

    private Gamepad playerGamepad;

    public Transform CameraForwardTransform => cameraForwardPos;
    public Transform CameraReverseTransform => cameraReversePos;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audio_enabler = GetComponent<AudioEnabler>();

        // Camera-transform variables initialisation 
        UpdateCameraTransformPositions();
        rootForward = cameraForwardPos.localPosition;
        rootReverse = cameraReversePos.localPosition;
        maxCameraReverseDist = Vector3.Magnitude(lookAtPosition - cameraReverseOrigin);
        maxCameraForwardDist = Vector3.Magnitude(lookAtPosition - cameraForwardOrigin);
    }

    // Updates positions based on the camera transforms
    // Mainly doing this to avoid duplicating this code
    private void UpdateCameraTransformPositions()
    {
        lookAtPosition = look_at_transform.position;
        cameraReverseOrigin = cameraReversePos.position;
        cameraForwardOrigin = cameraForwardPos.position;
    }

    private void Start()
	{
		SetupPlayerModel();
    }

    // Regular update
    private void Update()
	{
		//GetInput();
	}

    // Physics update
    private void FixedUpdate()
    {
        HandleLift();
        RepositionCameraTransforms();
    }

    public void move()
    {
		if (!IsVehicleOccupied())
		{
            return; 
        }

        if (playerGamepad.leftTrigger.ReadValue() != 0)
        {
            audio_enabler.Enable("reverse");
        }
        else
        {
            audio_enabler.Disable("reverse");
        }

        if (playerGamepad.rightTrigger.ReadValue() - playerGamepad.leftTrigger.ReadValue() != 0)
        {
            audio_enabler.Enable("driving");
        }
    }

    public void Lift()
    {
        audio_enabler.Enable("arms");

        isLiftGoingUp = true;
        isLiftGoingDown = false;
    }

    public void Drop()
    {
        audio_enabler.Enable("arms");


        isLiftGoingUp = false;
        isLiftGoingDown = true;
    }

    public void cancelLift()
    {
        audio_enabler.Disable("arms");

        isLiftGoingUp = false;
        isLiftGoingDown = false;
    }

    public void interact()
    {
        TryExitVehicle();

        // Is the player trying to exit the vehicle?
/*        if (Input.GetButton("Fire" + playerNumber))
        {
            if (currentExitVehicleTimer >= exitVehicleTime)
            {
            }
            else
            {
                if (currentExitVehicleTimer == 0)
                {
                    hudManager.SetVehiclePromptStatus(playerNumber, true);
                    hudManager.SetVehiclePromptText(playerNumber, "Exit Forklift");
                }

                currentExitVehicleTimer += Time.deltaTime;
            }
        }
        else
        {
            currentExitVehicleTimer = 0;
            hudManager.SetVehiclePromptStatus(playerNumber, false);
        }*/
    }

    public Transform getExitTransform()
    {
        return exitTransform;
    }

    public Vector3 getLookAtTransform()
    {
        return look_at_transform.position;
    }

    private void ChangeWheelPosition(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;

        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    private void HandleLift()
    {
        float y = lift.localPosition.y;

        if (isLiftGoingUp)
        {
            y += liftSpeed * Time.deltaTime;
            y = Mathf.Clamp(y, minimumLiftPosition, maxLiftPosition);

            lift.localPosition = new Vector3(lift.localPosition.x, y, lift.localPosition.z);
        }
        else if (isLiftGoingDown)
        {
            y -= liftSpeed * Time.deltaTime;
            y = Mathf.Clamp(y, minimumLiftPosition, maxLiftPosition);

            lift.localPosition = new Vector3(lift.localPosition.x, y, lift.localPosition.z);
        }
    }
	
	private void SetupPlayerModel()
	{
		if (IsVehicleOccupied())
		{
			playerMesh.enabled = true;
		}
		else
		{
			playerMesh.enabled = false;
		}
	}

    // Repositions the transforms of cameras based on if they would collide with eachother
    void RepositionCameraTransforms()
    {
        UpdateCameraTransformPositions();
        RaycastHit hit;
        Vector3 direction;

        // Get layer mask we need
        LayerMask mask = ~LayerMask.GetMask("Ignore Raycast", "UI", "Crates");

        // Forward cam transform
        direction = cameraForwardOrigin - lookAtPosition;
        if (Physics.Raycast(lookAtPosition, direction, out hit, maxCameraForwardDist, mask))
        {
            cameraForwardPos.position = hit.point;
        }
        else
        {
            cameraForwardPos.localPosition = rootForward;
        }

        // Reverse cam transform
        direction = cameraReverseOrigin - lookAtPosition;
        if (Physics.Raycast(lookAtPosition, direction, out hit, maxCameraReverseDist, mask))
        {
            cameraReversePos.position = hit.point;
        }
        else
        {
            cameraReversePos.localPosition = rootReverse;
        }
    }
	
	#region IDriveable
	
	public bool IsVehicleOccupied()
	{
		return playerGamepad != null;
	}
	
	public bool TryEnterVehicle(PlayerController player)
	{
		// Prevent a player trying to get in a vehicle another player is currently in
		if (IsVehicleOccupied())
        { 
            return false; 
        }
		
        Debug.Log("Entered Vehichle");
        onVehichleEnter.Invoke();
        driver = player;
        playerGamepad = player.GetPlayerGamepad();
		
		SetupPlayerModel();

        return true;
	}
	
	public bool TryExitVehicle()
	{
		driver = null;
        playerGamepad = null;
		
		playerMesh.enabled = false;

        audio_enabler.Disable("driving");

        GetComponent<DrivingController>().reset();

        Debug.Log("Exited Vehichle");
        onVehichleExit.Invoke();
        return true;
	}
	
	#endregion IDriveable
}