using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMovement : MonoBehaviour
{
    public float speed = 6f;
	public float turnSmoothing = 0.1f;
	float turnSmoothVelocity;
    
    CharacterController pController;
	Transform pCam;

	private void Awake()
	{
		pController = GetComponent<CharacterController>();
		pCam = GameObject.Find("Player Camera").transform;
	}
	// Update is called once per frame
	void Update()
    {
		float _horizontal = Input.GetAxisRaw("Horizontal");
		float _vertical = Input.GetAxisRaw("Vertical");
		Vector3 _direction = new Vector3(_horizontal, 0f, _vertical).normalized;

		if(_direction.magnitude >= 0.1f)
		{
			float _trgtAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + pCam.eulerAngles.y;
			float _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _trgtAngle, ref turnSmoothVelocity, turnSmoothing);
			transform.rotation = Quaternion.Euler(0f, _angle, 0f);

			Vector3 _faceDir = Quaternion.Euler(0f, _trgtAngle, 0f) * Vector3.forward;
			pController.Move(_faceDir.normalized * speed * Time.deltaTime);
		}
    }
}
