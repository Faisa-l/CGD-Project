using StarterAssets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// Source - https://www.youtube.com/watch?v=17j-u7z4wlE
// making a game in one hour (forklift simulation) - Flutter With Gia
public class ForkliftController : MonoBehaviour, IDriveable
{
    [Header("Settings")]
    [SerializeField][Range(0, 4)] private int playerNumber = 1; // 0 means no player currently 
    [SerializeField] private float motorTorque = 100.0f;
    [SerializeField] private float brakeForce = 30.0f;
    [SerializeField] private float maxSteerAngle = 45.0f;
    [SerializeField] private float steeringWheelPower = 3.0f;
    [SerializeField] private float downwardForce = 9.81f;
	[SerializeField] private Vector3 centerOfMass = new Vector3 (0, -0.9f, 0);

    [Header("Lift")]
    [SerializeField] private Transform lift;
    [SerializeField] private float liftSpeed = 1.0f;
    [SerializeField] private float minimumLiftPosition = 2.4f;
    [SerializeField] private float maxLiftPosition = 9.5f;

    [Header("Wheel Collider References")]
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [Header("Wheel Transform References")]
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

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

    private float horizontalInput = 0.0f;
    private float verticalInput = 0.0f;
    private bool isBraking = false;

    private float brakeTorque = 0.0f;
    private float steerAngle = 0.0f;

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
		
		// Anti-tipping
		// Source - https://discussions.unity.com/t/how-to-stop-my-car-tipping-over/34753
		rb.centerOfMass = centerOfMass;

    }

    // Regular update
    private void Update()
	{
		//GetInput();
	}

    // Physics update
    private void FixedUpdate()
    {
        HandleTorque();
        HandleSteering();
        UpdateWheelPosition();
        UpdateSteeringWheelPosition();
        HandleLift();
        RepositionCameraTransforms();
		
		// Anti-tipping
        // Source - https://www.reddit.com/r/Unity3D/comments/e808la/how_to_make_my_car_not_tip_over/
        rb.AddForce(Vector3.down * downwardForce, ForceMode.Force);
    }

    public void move(StarterAssetsInputs input)
    {
		if (!IsVehicleOccupied())
		{
            return; 
        }

        // Get player input
        verticalInput = playerGamepad.rightTrigger.ReadValue() - playerGamepad.leftTrigger.ReadValue();
        horizontalInput = input.move.x;

        if (playerGamepad.leftTrigger.ReadValue() != 0)
        {
            audio_enabler.Enable("reverse");
        }
        else
        {
            audio_enabler.Disable("reverse");
        }

        if (verticalInput != 0)
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

    private void HandleTorque()
    {
		if (!IsVehicleOccupied())
		{
			// Automatically apply handbrake when no one is in forklift
			Brake(brakeForce);
			return;
		}
			
		
        // Apply movement torque to wheels
        frontLeftWheelCollider.motorTorque = verticalInput * motorTorque;
        frontRightWheelCollider.motorTorque = verticalInput * motorTorque;
        rearLeftWheelCollider.motorTorque = verticalInput * motorTorque;
        rearRightWheelCollider.motorTorque = verticalInput * motorTorque;

        // Apply brake force
        if (isBraking)
            brakeTorque = brakeForce;
        else
            brakeTorque = 0.0f;

        Brake(brakeTorque);
    }

    private void HandleSteering()
    {
		if (IsVehicleOccupied())
			steerAngle = maxSteerAngle * horizontalInput;
		else
			steerAngle = 0.0f;

        // Front wheel drive
        frontLeftWheelCollider.steerAngle = steerAngle;
        frontRightWheelCollider.steerAngle = steerAngle;
    }

    private void UpdateSteeringWheelPosition()
    {
		if (!IsVehicleOccupied())
			return;
		
        if (horizontalInput > 0.1f)
            steeringWheel.localRotation *= Quaternion.Euler(0, 0, -steeringWheelPower * horizontalInput);
        else if (horizontalInput < -0.1f)
            steeringWheel.localRotation *= Quaternion.Euler(0, 0, steeringWheelPower * -horizontalInput);
    }

    private void UpdateWheelPosition()
    {
        ChangeWheelPosition(frontLeftWheelCollider, frontLeftWheelTransform);
        ChangeWheelPosition(frontRightWheelCollider, frontRightWheelTransform);
        ChangeWheelPosition(rearLeftWheelCollider, rearLeftWheelTransform);
        ChangeWheelPosition(rearRightWheelCollider, rearRightWheelTransform);
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
	
	private void Brake(float brakePower)
	{
		frontLeftWheelCollider.brakeTorque = brakePower;
        frontRightWheelCollider.brakeTorque = brakePower;
        rearLeftWheelCollider.brakeTorque = brakePower;
        rearRightWheelCollider.brakeTorque = brakePower;
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
		
		// Come to a stop over time
		frontLeftWheelCollider.motorTorque = 0.0f;
		frontRightWheelCollider.motorTorque = 0.0f;
		rearLeftWheelCollider.motorTorque = 0.0f;
		rearRightWheelCollider.motorTorque = 0.0f;

        audio_enabler.Disable("driving");

        Debug.Log("Exited Vehichle");
        onVehichleExit.Invoke();
        return true;
	}
	
	#endregion IDriveable
}