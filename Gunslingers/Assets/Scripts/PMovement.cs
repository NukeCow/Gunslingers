using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMovement : MonoBehaviour
{
    public float speed;
	public float turnSmoothing;
	public float gravity;
	public LayerMask groundMask;
	public float groundCheckHeight = 0.4f;
	public float groundCheckRadius = 0.5f;
	float turnSmoothVelocity;
	Vector3 velocity;
	bool isGrounded = false;
    
    [SerializeField] CharacterController pController;
	[SerializeField] Transform pCam;
	[SerializeField] Transform groundCheck;

	// Update is called once per frame
	void Update()
    {
		//isGrounded = Physics.CheckBox(groundCheck.position, groundCheckSize, groundCheck.rotation, groundMask);
		isGrounded = Physics.CheckCapsule(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckHeight, groundCheck.position.z), groundCheckRadius, groundMask);
		if (isGrounded && velocity.y < 0) velocity.y = -2f;

		float _horizontal = Input.GetAxisRaw("Horizontal");
		float _vertical = Input.GetAxisRaw("Vertical");
		Vector3 _direction = new Vector3(_horizontal, 0f, _vertical).normalized;

		if(_direction.magnitude >= 0.1f && isGrounded)
		{
			float _trgtAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + pCam.eulerAngles.y;
			float _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _trgtAngle, ref turnSmoothVelocity, turnSmoothing);
			transform.rotation = Quaternion.Euler(0f, _angle, 0f);

			Vector3 _faceDir = Quaternion.Euler(0f, _trgtAngle, 0f) * Vector3.forward;
			pController.Move(_faceDir.normalized * speed * Time.deltaTime);
		}
		if (!isGrounded)
		{
			velocity.y += gravity * Time.deltaTime;
			pController.Move(velocity * Time.deltaTime);
		}
	}
}
