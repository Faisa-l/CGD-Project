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

    [Header("MainComponents")]
    [SerializeField] Rigidbody rigidBody;

    [Header("Variables")]   
    [SerializeField] float acceleration = 20f;
    [SerializeField] float break_multiplier = 3.0f;
    [SerializeField] float speed = 0f;
    [SerializeField] float max_speed = 35f;
    [SerializeField] float rotate_speed = 5.0f;
    [SerializeField] Movement movement;
    [SerializeField] bool is_moving => (movement.movingValue != 0);

    [Header("Gravity Variables")]
    [SerializeField] Transform groundCheck;
    [SerializeField] bool is_grounded;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] float wheelRadius = 0.5f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float vehicleMass = 500f;
    [SerializeField] float downwardVelocity = 0.0f;

    [Header("Drifting Variables")]
    [SerializeField] GameObject body;
    [SerializeField] float maxRotation = 30;
    [SerializeField] Animation driftAnimation;

    float sign = 1f;

    [Header("Manual drift variables")]
    [SerializeField] bool manualDriftAnim = true;
    [SerializeField] float manualAnimationSpeed = 1f;

    [Header("Drifting")]
    [SerializeField] bool drifting = false;
    [SerializeField] InputActionReference driftInput;
    [SerializeField] float driftMultiplier = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        is_grounded = true;

        //driftInput.action.started += context => { drifting = true; };
        //driftInput.action.canceled += context => { drifting = false; };
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();  
        updateMove();
        updateRotate();

        transform.SetPositionAndRotation(transform.position, new Quaternion(0, transform.rotation.y, 0, transform.rotation.w));
    }

    private void updateMove()
    {
        if (!is_grounded) return;

        //if triggers held
        if (is_moving)
        {
            //if current speed is maxxed out and the player is attempting to move in that direction
            if (Mathf.Abs(speed) >= max_speed && sign == Mathf.Sign(speed))
            {
                speed = sign * max_speed;
            }
            else
            {
                speed += acceleration * Time.deltaTime * sign * ((Mathf.Sign(speed) != sign) ? break_multiplier : 1);
            }
        }
        //if triggers not held
        else
        {
           //if speed is around 0 then stop
           if (Mathf.Abs(speed) <= acceleration * Time.deltaTime * break_multiplier)
           {
               speed = 0;    
               movement.movingValue = 0;
           }
           //otherwise decelerate
           else
           {
               speed -= acceleration * Time.deltaTime * Mathf.Sign(speed) * break_multiplier;
           }
        }

        rigidBody.linearVelocity = (transform.forward * speed) + new Vector3(0,rigidBody.linearVelocity.y,0);
    }

    private void updateRotate()
    {
        //don't do rotations if the forklift isn't moving
        if (speed == 0) return;

        //do the actual forklift rotation so it turns
        transform.Rotate(0, sign * movement.turningValue * rotate_speed * (drifting ? driftMultiplier : 1.0f) * Time.deltaTime, 0);

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

    public void OnMove(InputValue value)
    {
        if (!is_grounded) return;

        movement.movingValue = value.Get<Vector2>().y;

        if (movement.movingValue != 0)
        {
            sign = Mathf.Sign(movement.movingValue);
        }
    }

    public void Move(InputValue value)
    {
        if (!is_grounded)
        {
            return;
        }

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

    public void Turn(InputValue value)
    {
        movement.turningValue = value.Get<Vector2>().x;
    }

    public void drift()
    {
        drifting = !drifting;
    }

    public void ApplyGravity()
    {
        if (!is_grounded)
        {
            downwardVelocity += gravity * vehicleMass * Time.deltaTime;
            //rigidBody.Move(Vector3.up * downwardVelocity * Time.deltaTime);
        }
        else
        {
            downwardVelocity = 0;
        }
    }

    public void GroundCheck()
    {
        RaycastHit hit; 
        float rayLength = groundDistance + wheelRadius;
        is_grounded = Physics.Raycast(groundCheck.position, -groundCheck.up, out hit, rayLength, groundMask);
        Debug.DrawRay(groundCheck.position, -groundCheck.up * rayLength, is_grounded ? Color.green : Color.red);
    }

    public void OnDrawGizmos()
    {
        RaycastHit hit;
        float rayLength = groundDistance + wheelRadius;
        if (Physics.Raycast(groundCheck.position, -groundCheck.up, out hit, rayLength, groundMask))
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawRay(groundCheck.position, -groundCheck.up * rayLength);
    }

    public void reset()
    {
        movement.movingValue = 0;
        movement.turningValue = 0;
        drifting = false;
    }
}
