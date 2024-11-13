using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMovement : MonoBehaviour
{
    public float speed; //The player's movement speed
	public float turnSmoothing; //How much smoothing should be applied to the player's turns
	public float gravity; //Gravitational constant
	public LayerMask groundMask; //The layer that the ground checker should check for
	public float groundCheckHeight = 0.01f; //How tall the ground check cylinder is
	public float groundCheckRadius = 0.5f; //The ground checking cylinder's radius
	float turnSmoothVelocity; //A reference variable for turn clamping
	Vector3 velocity; //The player's velocity. Currently only used for gravity calculations
	bool isGrounded = false; //Is the player on the ground?
	Vector3 inputDirection; //The direction of the player's inputs

	public float speedDodge; //How fast the player moves when dodging
	public float dodgeKeyHoldTime; //Time, in seconds, the player must hold down the dodge key to start sprinting
	public float speedSprint; //How fast the player moves when sprinting
	public float dodgeTime; //The length of time, in seconds, the player's dodge is
	public bool canControl = true; //Are the player's controls enabled?
	bool isSprinting = false; //Is the player sprinting?
    
    [SerializeField] CharacterController pController; //Reference to the player's Character Controller component
	[SerializeField] Transform pCam; //Reference to the player's camera's transform component
	[SerializeField] Transform groundCheck; //Reference to the ground checker object's transform component
	PLockOn pLockOn; //Reference to the player's lock on script

	[SerializeField] KeyCode keyDodge; //The key assigned to the Dodge action

	//Called when the script is loaded
	private void Awake()
	{
		pLockOn = GetComponent<PLockOn>();
	}

	//Called every frame
	void Update()
    {
		isGrounded = Physics.CheckCapsule(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckHeight, groundCheck.position.z), groundCheckRadius, groundMask);
		if (isGrounded && velocity.y < 0) velocity.y = 0f;

		if (canControl)
		{
			float _horizontal = Input.GetAxisRaw("Horizontal");
			float _vertical = Input.GetAxisRaw("Vertical");
			inputDirection = new Vector3(_horizontal, 0f, _vertical).normalized;
			if (Input.GetKeyDown(keyDodge)) StartCoroutine(SprintTest(dodgeKeyHoldTime));
			if (Input.GetKeyUp(keyDodge)) isSprinting = false;

			if (isSprinting) pController.Move(GetPlayerMovementVector(speedSprint));
			else pController.Move(GetPlayerMovementVector(speed));
		}
		if (!isGrounded)
		{
			velocity.y += gravity * Time.deltaTime;
			pController.Move(velocity * Time.deltaTime);
		}
	}
	
	//Starts a sprint if the player holds the dodge button for the given time or dodges if the player lets go of the dodge button after a shorter amount of time than the given time.
	IEnumerator SprintTest(float _sprintTime)
	{
		float _startTime = Time.time;
		while (Time.time < _startTime + _sprintTime)
		{
			if (Input.GetKeyUp(keyDodge))
			{
				StartCoroutine(Dodge());
				canControl = false;
				break;
			}
			yield return null;
		}
		if (Time.time >= _startTime + _sprintTime)
		{
			isSprinting = true;
		}
		yield return null;
	}

	//Makes the player dodge
	IEnumerator Dodge()
	{
		float _startTime = Time.time;
		while (Time.time < _startTime + dodgeTime)
		{
			pController.Move(GetPlayerMovementVector(speedDodge));
			yield return null;
		}
		canControl = true;
		isSprinting = false;
	}

	//Returns the direction and speed of the player's movement given a speed to move the player
	public Vector3 GetPlayerMovementVector(float _speed)
	{
		float _targetMoveAngle = 0f;
		if (inputDirection.magnitude >= 0.1f)
		{
			_targetMoveAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + pCam.eulerAngles.y;
		}
		else
		{
			_targetMoveAngle = transform.rotation.eulerAngles.y;
			_speed = 0f;
		}
		float _angle = 0f;
		if (pLockOn.isLockedOn && !isSprinting)
		{
			//Lock On Rotation
			Vector3 _targetFaceDir = pLockOn.targetCurrent.transform.position - transform.position;
			float _targetFaceAngle = Mathf.Atan2(_targetFaceDir.x, _targetFaceDir.z) * Mathf.Rad2Deg;
			_angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetFaceAngle, ref turnSmoothVelocity, turnSmoothing);
		}
		else
		{
			//Free Look Rotation
			_angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetMoveAngle, ref turnSmoothVelocity, turnSmoothing);
		}
		transform.rotation = Quaternion.Euler(0f, _angle, 0f);

		Vector3 _faceDir = Quaternion.Euler(0f, _targetMoveAngle, 0f) * Vector3.forward;
		Vector3 _move = _faceDir.normalized * _speed * Time.deltaTime;
		return _move;
	}
}
