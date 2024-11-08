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

	public float speedDodge; //How fast the player moves when dodging
	public float dodgeKeyHoldTime; //Time, in seconds, the player must hold down the dodge key to start sprinting
	public float speedSprint; //How fast the player moves when sprinting
    
    [SerializeField] CharacterController pController; //Reference to the player's Character Controller component
	[SerializeField] Transform pCam; //Reference to the player's camera's transform component
	[SerializeField] Transform groundCheck; //Reference to the ground checker object's transform component

	[SerializeField] KeyCode keyDodge; //The key assigned to the Dodge action
	// Update is called once per frame
	void Update()
    {
		isGrounded = Physics.CheckCapsule(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckHeight, groundCheck.position.z), groundCheckRadius, groundMask);
		if (isGrounded && velocity.y < 0) velocity.y = -2f;

		float _horizontal = Input.GetAxisRaw("Horizontal");
		float _vertical = Input.GetAxisRaw("Vertical");
		Vector3 _direction = new Vector3(_horizontal, 0f, _vertical).normalized;

		if(_direction.magnitude >= 0.1f)
		{
			float _trgtAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + pCam.eulerAngles.y;
			float _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _trgtAngle, ref turnSmoothVelocity, turnSmoothing);
			transform.rotation = Quaternion.Euler(0f, _angle, 0f);

			Vector3 _faceDir = Quaternion.Euler(0f, _trgtAngle, 0f) * Vector3.forward;
			Vector3 _move = _faceDir.normalized * speed * Time.deltaTime;
			//If sprinting, multiply by speedSprint as well
			pController.Move(_move);
		}
		if (!isGrounded)
		{
			velocity.y += gravity * Time.deltaTime;
			pController.Move(velocity * Time.deltaTime);
		}
	}
	
	IEnumerator DodgeKeyHoldTime(float _sprintTime)
	{
		float _time = 0f;
		while (Input.GetKey(keyDodge) && _time < _sprintTime)
		{
			_time += Time.deltaTime;
		}
	}
}
