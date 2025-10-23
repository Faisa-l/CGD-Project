using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// Source - https://www.youtube.com/watch?v=17j-u7z4wlE
// making a game in one hour (forklift simulation) - Flutter With Gia
public class ForkliftController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField][Range(1, 4)] private int playerNumber = 1;
    [SerializeField] private float motorTorque = 100.0f;
    [SerializeField] private float brakeForce = 30.0f;
    [SerializeField] private float maxSteerAngle = 45.0f;
    [SerializeField] private float steeringWheelPower = 3.0f;

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

    [Header("Other References")]
    [SerializeField] private Transform steeringWheel;

    private float horizontalInput = 0.0f;
    private float verticalInput = 0.0f;
    private bool isBraking = false;

    private float brakeTorque = 0.0f;
    private float steerAngle = 0.0f;

    private bool isLiftGoingUp = false;
    private bool isLiftGoingDown = false;

    // Physics update
    private void FixedUpdate()
    {
        GetInput();
        HandleTorque();
        HandleSteering();
        UpdateWheelPosition();
        UpdateSteeringWheelPosition();
        HandleLift();
    }

    private void GetInput()
    {
        // Get player input
        horizontalInput = Input.GetAxis("Horizontal" + playerNumber);
        verticalInput = Input.GetAxis("Vertical" + playerNumber);
        isBraking = Input.GetButton("Fire" + playerNumber);

        // Lift
        //if (Input.GetKey(KeyCode.Q))
        if (Input.GetAxis("Lift" + playerNumber) > 0.1f)
        {
            isLiftGoingUp = true;
            isLiftGoingDown = false;
        }
        //else if (Input.GetKey(KeyCode.E))
        else if (Input.GetAxis("Lift" + playerNumber) < -0.1f)
        {
            isLiftGoingUp = false;
            isLiftGoingDown = true;
        }
        else
        {
            isLiftGoingUp = false;
            isLiftGoingDown = false;
        }
    }

    private void HandleTorque()
    {
        // Apply movement torque to wheels
        frontLeftWheelCollider.motorTorque = verticalInput * motorTorque;
        frontRightWheelCollider.motorTorque = verticalInput * motorTorque;
        rearLeftWheelCollider.motorTorque = verticalInput * motorTorque;
        rearRightWheelCollider.motorTorque = verticalInput * motorTorque;

        // Apply brake force
        if (isBraking)
            brakeTorque = brakeForce;
        else
            brakeTorque = 0;

        frontLeftWheelCollider.brakeTorque = brakeTorque;
        frontRightWheelCollider.brakeTorque = brakeTorque;
        rearLeftWheelCollider.brakeTorque = brakeTorque;
        rearRightWheelCollider.brakeTorque = brakeTorque;
    }

    private void HandleSteering()
    {
        steerAngle = maxSteerAngle * horizontalInput;

        // Front wheel drive
        frontLeftWheelCollider.steerAngle = steerAngle;
        frontRightWheelCollider.steerAngle = steerAngle;
    }

    private void UpdateSteeringWheelPosition()
    {
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
}