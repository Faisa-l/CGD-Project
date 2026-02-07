using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrivingController : MonoBehaviour
{
    struct Movement
    {
        public float turningValue;
        public float movingValue;
    }

    [Header("Main Components")]
    [SerializeField] Rigidbody rigidBody;

    [Header("Movement variables")]   
    [SerializeField] float acceleration = 20f;
    [SerializeField] float breakMultiplier = 3.0f;
    [SerializeField] float speed = 0f;
    [SerializeField] float maxSpeed = 35f;
    [SerializeField] float rotateSpeed = 5.0f;
    [SerializeField] Movement movement;
    [SerializeField] bool is_moving => (movement.movingValue != 0);

    [Header("Ground Checking Variables")]
    [SerializeField] Transform groundCheckTransform;
    [SerializeField] bool isGrounded;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] float wheelRadius = 0.5f;

    [Header("Drifting Variables")]
    [SerializeField] GameObject body;
    [SerializeField] float maxRotation = 30;
    [SerializeField] Animation driftAnimation;
    [SerializeField] bool manualDriftAnim = true;
    [SerializeField] float manualAnimationSpeed = 1f;
    [SerializeField] bool drifting = false;
    [SerializeField] float driftMultiplier = 2f;

    float sign = 1f;

    [Header("Lift Variables")]
    [SerializeField] private Transform lift;
    [SerializeField] private float liftSpeed = 1.0f;
    [SerializeField] private float minLiftPosition = 2.4f;
    [SerializeField] private float maxLiftPosition = 9.5f;

    [Header("UI")]
    [SerializeField] private HudManager hudManager;

    [Header("Other References")]
    [SerializeField] private Transform steeringWheel;
    [SerializeField] private SkinnedMeshRenderer playerMesh; // This data type so we can change the skin to match player getting in after alpha

    [SerializeField] private Transform lookAtTransform;
    [SerializeField] private Transform cameraForwardPos;
    [SerializeField] private Transform cameraReversePos;
    Vector3 rootForward, rootReverse;
    Vector3 lookAtPosition;
    Vector3 cameraReverseOrigin;
    Vector3 cameraForwardOrigin;
    float maxCameraReverseDist;
    float maxCameraForwardDist;

    [SerializeField] GameObject playerCamera = null;

    private Rigidbody rb;

    private AudioEnabler audio_enabler;

    private Gamepad playerGamepad;

    public Transform CameraForwardTransform => cameraForwardPos;
    public Transform CameraReverseTransform => cameraReversePos;

    bool lifting = false;

    public void setPlayerGamepad(Gamepad gamepad)
    {
        playerGamepad = gamepad;
    }

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

        playerCamera.transform.parent = null;
    }

    private void Update()
    {
        groundCheck();  
        updateMove();
        updateRotate();

        handleLift();
        repositionCameraTransforms();

        transform.SetPositionAndRotation(transform.position, new Quaternion(0, transform.rotation.y, 0, transform.rotation.w));
    }

#region Updating functions
    public void groundCheck()
    {
        RaycastHit hit; 
        float rayLength = groundDistance + wheelRadius;
        isGrounded = Physics.Raycast(groundCheckTransform.position, -groundCheckTransform.up, out hit, rayLength, groundMask);
        Debug.DrawRay(groundCheckTransform.position, -groundCheckTransform.up * rayLength, isGrounded ? Color.green : Color.red);
    }

    private void updateMove()
    {
        if (!isGrounded) return;

        //if triggers held
        if (is_moving)
        {
            //if current speed is maxxed out and the player is attempting to move in that direction
            if (Mathf.Abs(speed) >= maxSpeed && sign == Mathf.Sign(speed))
            {
                speed = sign * maxSpeed;
            }
            else
            {
                speed += acceleration * Time.deltaTime * sign * ((Mathf.Sign(speed) != sign) ? breakMultiplier : 1);
            }
        }
        //if triggers not held
        else
        {
           //if speed is around 0 then stop
           if (Mathf.Abs(speed) <= acceleration * Time.deltaTime * breakMultiplier)
           {
               speed = 0;    
               movement.movingValue = 0;
           }
           //otherwise decelerate
           else
           {
               speed -= acceleration * Time.deltaTime * Mathf.Sign(speed) * breakMultiplier;
           }
        }

        rigidBody.linearVelocity = (transform.forward * speed) + new Vector3(0,rigidBody.linearVelocity.y,0);

        //audio handling
        if (sign == -1 && is_moving)
        {
            audio_enabler.Enable("reverse");
        }
        else
        {
            audio_enabler.Disable("reverse");
        }

        if (sign == 1 && is_moving)
        {
            audio_enabler.Enable("driving");
        }
    }
    
    private void updateRotate()
    {
        //don't do rotations if the forklift isn't moving
        if (speed == 0) return;

        //do the actual forklift rotation so it turns
        transform.Rotate(0, sign * movement.turningValue * rotateSpeed * (drifting ? driftMultiplier : 1.0f) * Time.deltaTime, 0);

        //transform the angle of the forklift from what unity uses to a value that can be used with the maximum rotation value
        float bodyAngle = Mathf.Ceil(body.transform.localEulerAngles.y - 360f * Mathf.Floor(body.transform.localEulerAngles.y / 180f))%360;

        if(movement.turningValue == 0)
        {
            if(Mathf.Abs(bodyAngle) >= 0.1f)
            {
                body.transform.RotateAround(
                 body.transform.position + body.transform.forward * body.transform.localScale.z / 2f,
                 Vector3.up,
                 sign * -Mathf.Sign(bodyAngle) * manualAnimationSpeed);
            }
            else
            {
                body.transform.localRotation = new();
                body.transform.localPosition = new();
            }
        }

        //if the forklift isn't drifting, make sure it is looking forward
        if(!drifting || movement.movingValue == -1 || movement.turningValue == 0)
        {
            body.transform.localRotation = new();
            body.transform.localPosition = new();
            return;
        }


        //-----If player is drifting-----//

        //If the body is fully rotated, then return
        if (Mathf.Abs(bodyAngle) >= maxRotation && Mathf.Sign(bodyAngle) == MathF.Sign(movement.turningValue)) 
        {
            return;
        }

        //If there is no animation added for the drift, then do a basic, manual animation
        if(manualDriftAnim)
        {
            body.transform.RotateAround(
                body.transform.position + body.transform.forward * body.transform.localScale.z / 2f,
                Vector3.up,
                sign * movement.turningValue * manualAnimationSpeed * Time.deltaTime);
        }
        else
        {
            if(driftAnimation != null)
            {
                driftAnimation.Play();
            }

            //matbe not needed, need animation first if we are using one
            body.transform.RotateAround(
                body.transform.position + body.transform.forward * body.transform.localScale.z / 2f,
                Vector3.up,
                sign * movement.turningValue * maxRotation);
        }
    }

    private void handleLift()
    {
        float y = lift.localPosition.y;

        if (lifting)
        {
            y += liftSpeed * Time.deltaTime;
            y = Mathf.Clamp(y, minLiftPosition, maxLiftPosition);

            lift.localPosition = new Vector3(lift.localPosition.x, y, lift.localPosition.z);
        }
        else
        {
            y -= liftSpeed * Time.deltaTime;
            y = Mathf.Clamp(y, minLiftPosition, maxLiftPosition);

            lift.localPosition = new Vector3(lift.localPosition.x, y, lift.localPosition.z);
        }
    }

    // Repositions the transforms of cameras based on if they would collide with eachother
    void repositionCameraTransforms()
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

    // Updates positions based on the camera transforms
    // Mainly doing this to avoid duplicating this code
    private void UpdateCameraTransformPositions()
    {
        lookAtPosition = lookAtTransform.position;
        cameraReverseOrigin = cameraReversePos.position;
        cameraForwardOrigin = cameraForwardPos.position;
    }
    public void reset()
    {
        movement.movingValue = 0;
        movement.turningValue = 0;
        drifting = false;
    }

#endregion

#region Input Functions
    public void OnMove(InputValue value)
    {
        if (!isGrounded) return;

        movement.movingValue = value.Get<Vector2>().y;

        if (movement.movingValue != 0)
        {
            sign = Mathf.Sign(movement.movingValue);
        }
    }

    public void OnTurn(InputValue value)
    {      
        movement.turningValue = value.Get<Vector2>().x;
    }

    public void OnDrift()
    {
        drifting = !drifting;
    }

    public void OnLift()
    {
        lifting = !lifting;
    }

    public void OnLook(InputValue value)
    {
        Vector2 direction = value.Get<Vector2>();
        direction = new Vector2(Mathf.Round(direction.x), Mathf.Round(direction.y));

        if(direction.y == 1)
        {
            playerCamera.GetComponent<CameraController>().setFollowing(cameraReversePos);
        }
        else
        {
            playerCamera.GetComponent<CameraController>().setFollowing(cameraForwardPos);
        }
    }

    public void OnInteract()
    {
        GetComponent<FloatPickup>().PickUpSelectedForklift();

        GetComponent<FloatPickup>().PickUpSelected();
    }

#endregion

    public void OnDrawGizmos()
    {
        RaycastHit hit;
        float rayLength = groundDistance + wheelRadius;
        if (Physics.Raycast(groundCheckTransform.position, -groundCheckTransform.up, out hit, rayLength, groundMask))
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawRay(groundCheckTransform.position, -groundCheckTransform.up * rayLength);
    }

}
