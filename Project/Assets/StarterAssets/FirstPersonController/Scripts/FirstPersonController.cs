using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviour
	{
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Space(10)]
		[Tooltip("Animator component for the player model")]
		public Animator anim;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public CameraController CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;
		public float DrivingTopClamp = 90.0f;
		public float DrivingBottomClamp = 20.0f;

		[Space(20)]
		public bool use_camera_pitch = true;
		public bool useFixedDrivingCamera = false;

		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 30.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

	
#if ENABLE_INPUT_SYSTEM
		private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		private GameObject _mainCamera;

		private const float _threshold = 0.01f;

		private AudioEnabler _audioEnabler;
        private PlayerController playerController;

		private InputValue lookInput = new();
		private InputValue speedControlInput = new();
		//private InputValue jumpInput = new();

		private Vector2 lookValue = new();
		private Vector2 speedControl = new();

        private bool IsCurrentDeviceMouse
		{
			get
			{
				#if ENABLE_INPUT_SYSTEM
				return _playerInput.currentControlScheme == "KeyboardMouse";
				#else
				return false;
				#endif
			}
		}

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}

            transform.localRotation = new Quaternion(0, 0, 0, 0);
			playerController = GetComponent<PlayerController>();
        }

        private void Start()
		{
			_controller = GetComponent<CharacterController>();

#if ENABLE_INPUT_SYSTEM
			_playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
			_audioEnabler = GetComponent<AudioEnabler>();

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;

			transform.localRotation = new Quaternion(0, 0, 0, 0);
        }

		private void FixedUpdate()
		{
			if (!_controller.enabled) return;

			doGravity();
			GroundedCheck();
			Move();
		}

		private void LateUpdate()
		{
			CameraRotation();
		}

		private void doGravity()
		{
			_controller.Move(!Grounded ? -gameObject.transform.up * _terminalVelocity * Time.deltaTime : new Vector3());
        }

		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{

            // if there is an input
            if (lookValue.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _rotationVelocity = lookValue.x * RotationSpeed * deltaTimeMultiplier;
                _cinemachineTargetPitch += lookValue.y * RotationSpeed * deltaTimeMultiplier;

                if (playerController.driving)
                {
					if (useFixedDrivingCamera)
					{
						bool isLookingBack = lookValue.y > -_threshold;
						playerController.SetCameraPosition(isLookingBack);
					}
					else
					{
						playerController.cameraDrive(_rotationVelocity);
					}

                    /*
                    if (use_camera_pitch)
                    {
                        // clamp our pitch rotation
                        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, DrivingBottomClamp, DrivingTopClamp);

                        // Update Cinemachine camera target pitch
                        CinemachineCameraTarget.GetComponentInChildren<Camera>().transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
                    }
					*/
                }
                else
				{
                    // rotate the player left and right
                    transform.Rotate(Vector3.up * _rotationVelocity);

                    // clamp our pitch rotation
                    _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

					// Update Cinemachine camera target pitch
					CinemachineCameraTarget.GetComponentInChildren<Camera>().transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
                }
            }
			else
			{
				// Reset camera position if driving and no input is registered
				if (useFixedDrivingCamera && playerController.driving) playerController.SetCameraPosition(false);
			}
		}

		private void Move()
		{

			if(GetComponent<PlayerController>().driving)
			{
				_audioEnabler.Disable("player");
				GetComponent<PlayerController>().drive();
				return;
			}

            float targetSpeed = MoveSpeed;

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (speedControl == Vector2.zero) targetSpeed = 0.0f;

			//If the player is moveing, play the walking sound
			if(targetSpeed != 0.0f)
			{
                _audioEnabler.Enable("player");
            }
			else
			{
				_audioEnabler.Disable("player");
			}

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = speedControl.magnitude;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(speedControl.x, 0.0f, speedControl.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (speedControl != Vector2.zero)
			{
				// move
				inputDirection = transform.right * speedControl.x + transform.forward * speedControl.y;
				
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
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime));
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		public void OnMove(InputValue input)
		{
			speedControl = input.Get<Vector2>();
        }

		public void OnForkliftMove(InputValue input)
		{
			GetComponent<PlayerController>().moveInput(input);
		}

		public void OnTurn(InputValue input)
		{
            GetComponent<PlayerController>().turnInput(input);
        }

		public void OnLook(InputValue input)
		{
			//lookInput = input;
			lookValue = input.Get<Vector2>();
		}

		public void OnDrift()
		{
			GetComponent<PlayerController>().driftInput();
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
	}
}