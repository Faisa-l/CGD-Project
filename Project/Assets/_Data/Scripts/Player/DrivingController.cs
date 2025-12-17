using System.ComponentModel;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrivingController : MonoBehaviour
{

    [Header("MainComponents")]
    [SerializeField] Rigidbody rigidBody;

    [Header("Variables")]   
    [SerializeField] float acceleration = 20f;
    [SerializeField] float break_multiplier = 3.0f;
    [SerializeField] float speed = 0f;
    [SerializeField] float max_speed = 35f;
    [SerializeField] float rotate_speed = 5.0f;
    [SerializeField] Vector2 Movement;
    [SerializeField] bool is_moving => (Movement.y != 0);

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

        driftInput.action.started += context => { drifting = true; };
        driftInput.action.canceled += context => { drifting = false; };
    }

    // Update is called once per frame
    void Update()
    {
        //ApplyGravity();
        GroundCheck();  
        updateMove();
        updateRotate();
    }

    private void updateMove()
    {
        //if triggers held
        if (is_moving)
        {
            //if current speed is maxxed out
            if (Mathf.Abs(speed) >= max_speed)
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

                Movement.x = 0;
            }
            //otherwise decelerate
            else
            {
                speed -= acceleration * Time.deltaTime * sign * break_multiplier;
            }
        }

        float past_y_vel = rigidBody.linearVelocity.y;
        rigidBody.linearVelocity = (transform.forward * speed) + new Vector3(0,past_y_vel,0);
    }

    private void updateRotate()
    {
        transform.Rotate(0, Movement.x * rotate_speed * (drifting ? driftMultiplier : 1.0f) * Time.deltaTime, 0);

        float bodyAngle = Mathf.Ceil(body.transform.localEulerAngles.y - 360f * Mathf.Floor(body.transform.localEulerAngles.y / 180f));

        if(!drifting || Movement.x == 0)
        {
            body.transform.localRotation = new();
            body.transform.localPosition = new();
            return;
        }

        //If the body is fully rotated, then return
        if (Mathf.Abs(bodyAngle) >= maxRotation) 
        {
            return;
        }

        //If there is no animation added for the drift, then do a basic, manual animation
        if(manualDriftAnim)
        {
            body.transform.RotateAround(
                body.transform.position + body.transform.forward * body.transform.localScale.z / 2f,
                Vector3.up,
                Movement.x* manualAnimationSpeed);
        }
        else
        {
            if(driftAnimation != null)
            {
                driftAnimation.Play();
            }

            body.transform.RotateAround(
                body.transform.position + body.transform.forward * body.transform.localScale.z / 2f,
                Vector3.up,
                Movement.x * maxRotation);
        }
    }

    public void OnMove(InputValue value)
    {
        if (!is_grounded)
        {
            return;
        }

        Movement.y = value.Get<Vector2>().y;

        if (Movement.y != 0)
        {
            sign = Mathf.Sign(Movement.y);
        }
    }

    public void Move(InputValue value)
    {
        if (!is_grounded)
        {
            return;
        }

        Movement.y = value.Get<Vector2>().y;

        if (Movement.y != 0)
        {
            sign = Mathf.Sign(Movement.y);
        }
    }

    public void OnTurn(InputValue value)
    {      
        if (is_moving)
        {
            Movement.x = value.Get<Vector2>().x;
        }      
    }

    public void Turn(InputValue value)
    {
        if (is_moving)
        {
            Movement.x = value.Get<Vector2>().x;
        }
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
}
