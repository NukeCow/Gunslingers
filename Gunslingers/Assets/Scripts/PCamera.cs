using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PCamera : MonoBehaviour
{
	[Header("Lock On Settings")]
	[SerializeField] string cameraStateLockOnName = "Lock On"; //The name of the player's camera controller's animator's lock on state
	[SerializeField] string cameraStateFreeLookName = "Free Look"; //The name of the player's camera controller's animator's free look state
	bool isLockedOn; //Is the player locked on to a target?

	[Header("Object References")]
	[SerializeField] Animator stateController; //Reference to the Animator Component for the Player Camera Controller
	[SerializeField] CinemachineVirtualCamera lockOnCamera; //Reference to the player's virtual lock on camera

	//Locks the player's camera to the given target's transform component
	public void LockCameraToTarget(Transform _target)
	{
		lockOnCamera.LookAt = _target;
		if (!isLockedOn)
		{
			stateController.Play(cameraStateLockOnName);
			isLockedOn = true;
		}
	}

	//Returns the player's camera to free look mode
	public void UnlockCamera()
	{
		if (isLockedOn)
		{
			stateController.Play(cameraStateFreeLookName);
			isLockedOn = false;
		}
	}
}
