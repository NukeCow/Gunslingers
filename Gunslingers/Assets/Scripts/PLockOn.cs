using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLockOn : MonoBehaviour
{
    [Header("Lock On Controls")]
    [SerializeField] int buttonLockOn; //The id for the button assigned to toggling the player's lock on
    [SerializeField] string lockOnTargetChange = "Mouse X"; //The name of the axis assigned to searching for another lock on target while the player is already locked on
    [SerializeField] float targetChangeInputThreshold = 0.5f; //The lowest magnitude input from the player to count as an input to change lock on target
    [SerializeField] float changeTargetInputTime = 3f; //The amount of time the player's change target input is locked.

    [Header("Lock On Settings")]
    public CharacterController targetCurrent = null; //The player's current target
    [HideInInspector] public bool isLockedOn = false; //Is the player already locked on to an enemy?
    [SerializeField] float targetCheckRadius; //The radius of the sphere used to find lock on targets. Acts as the maximum distance a potential lock on target can be from the player.
    [SerializeField] float targetMinViewAngle; //The minimum angle from the player a potential target can be to be considered a valid lock on target.
    [SerializeField] float targetMaxViewAngle; //The maximum angle from the player a potential target can be to be considered a valid lock on target.
    [SerializeField] float targetMaxDist; //The maximum distance a lock on target can be from the player.
    [SerializeField] LayerMask targetCheckMask; //The layer the sphere used to find lock on targets will check. Filters out unnecessary colliders.
    [SerializeField] LayerMask targetRayMask; //The layer the raycast to a potential target will check for obstacles. Filters out unnecessary colliders.
    List<CharacterController> targetsAvailable = new List<CharacterController>(); //List of potential targets for the player to lock on to
    CharacterController targetNearest; //The potential target that is closest to the player
    CharacterController targetNearestLeft; //The potential target that is closest to the center of the player's camera's field of view; left of center
    CharacterController targetNearestRight; //The potential target that is closest to the center of the player's camera's field of view; right of center
    bool canChangeLockOnTarget = true; //Can the player change their lock on target?
    

    [Header("Object References")]
    [SerializeField] GameObject pObject; //Reference to the player game object
    [SerializeField] GameObject pCam; //Reference to the player's camera game object
    [SerializeField] GameObject pGFX; //Reference to the player's graphics game object
    [SerializeField] PCamera pCamController; //Reference to the camera controller script for the player's camera

    //Update is called every frame
    void Update()
    {
		if (isLockedOn)
		{
            //Check if current target is dead or out of range, or if the player cannot see the current target
            //If current target is dead, try to find new target and lock on to it. Otherwise, unlock
            RaycastHit _hit;
            if (Physics.Linecast(pGFX.transform.position, targetCurrent.transform.position, out _hit, targetRayMask) || Vector3.Distance(pObject.transform.position, targetCurrent.transform.position) > targetMaxDist)
			{
                ClearTargets();
                pCamController.UnlockCamera();
                isLockedOn = false;
			}

            if (Mathf.Abs(Input.GetAxisRaw(lockOnTargetChange)) >= targetChangeInputThreshold && canChangeLockOnTarget)
            {
                float _targetChangeAxisValue = Input.GetAxisRaw(lockOnTargetChange);
                FindLockTargets();

                if (_targetChangeAxisValue > 0f)
                {
                    SetTarget(targetNearestRight);
                }
                else
                {
                    SetTarget(targetNearestLeft);
                }

                pCamController.LockCameraToTarget(targetCurrent.transform);
                StartCoroutine(LockChangeTargetInput(changeTargetInputTime));
            }
        }

        if (Input.GetMouseButtonDown(buttonLockOn))
		{
			if (isLockedOn)
			{
                isLockedOn = false;
                ClearTargets();
                pCamController.UnlockCamera();
			}
			else
			{
                FindLockTargets();
                if(targetNearest != null)
				{
                    //Set nearest potential target as our lock on target
                    SetTarget(targetNearest);
                    pCamController.LockCameraToTarget(targetCurrent.transform);
                    isLockedOn = true;
				}
			}
		}
    }

    //Finds the closest valid lock on target to the player, as well as the nearest valid target to the right and left of the center of the player's field of view.
    public void FindLockTargets()
	{
        //Find Possible Targets In Range
        float _shortDist = Mathf.Infinity; //The shortest distance a potential target is from the player
        float _shortDistR = Mathf.Infinity; //The shortest distance to the right relative to the player a potential target is from the current target
        float _shortDistL = Mathf.NegativeInfinity; //The shortest distance to the left relative to the player a potential target is from the current target

        Collider[] _colliders = Physics.OverlapSphere(pObject.transform.position, targetCheckRadius, targetCheckMask);
        for (int i = 0; i < _colliders.Length; i++)
		{
            CharacterController _target = _colliders[i].GetComponent<CharacterController>();

            if(_target != null)
			{
                Vector3 _targetDir = _target.transform.position - pObject.transform.position;
                float _targetViewAngle = Vector3.Angle(_targetDir, pCam.transform.forward);

                //If target is dead, continue
                if (_targetViewAngle >= targetMinViewAngle && _targetViewAngle <= targetMaxViewAngle)
                {
                    RaycastHit _hit;

                    if (Physics.Linecast(pGFX.transform.position, _target.transform.position, out _hit, targetRayMask))
					{
                        continue;
					}
					else
					{
                        targetsAvailable.Add(_target);
					}
                }
            }
		}
        for (int k = 0; k < targetsAvailable.Count; k++)
		{
            if(targetsAvailable[k] != null)
			{
                float _targetDist = Vector3.Distance(pObject.transform.position, targetsAvailable[k].transform.position);

                if (_targetDist < _shortDist)
                {
                    _shortDist = _targetDist;
                    targetNearest = targetsAvailable[k];
                }

				if (isLockedOn)
				{
                    Vector3 _targetRelativePos = pObject.transform.InverseTransformPoint(targetsAvailable[k].transform.position);
                    float _targetDistLeft = _targetRelativePos.x;
                    float _targetDistRight = _targetRelativePos.x;

                    if (targetsAvailable[k] == targetCurrent) continue;
                    if (_targetRelativePos.x <= 0f && _targetDistLeft > _shortDistL)
					{
                        _shortDistL = _targetDistLeft;
                        targetNearestLeft = targetsAvailable[k];
					}
                    else if (_targetRelativePos.x >= 0f && _targetDistRight < _shortDistR)
					{
                        _shortDistR = _targetDistRight;
                        targetNearestRight = targetsAvailable[k];
					}
				}
            }
			else
			{
                ClearTargets();
                isLockedOn = false;
			}
		}
	}

    //Sets the player's current target to the character controller given
    public void SetTarget(CharacterController _target)
	{
        if(_target != null)
		{
            targetCurrent = _target;
		}
		else
		{
            targetCurrent = null;
		}
	}

    //Sets all variables related to finding targets to null and clears the available targets list
    public void ClearTargets()
	{
        targetNearest = null;
        targetNearestLeft = null;
        targetNearestRight = null;
        targetsAvailable.Clear();
	}

    IEnumerator LockChangeTargetInput(float _waitTime)
	{
        canChangeLockOnTarget = false;
        float _time = Time.time;
        while (Time.time <= _time + _waitTime)
		{
            yield return null;
		}
        canChangeLockOnTarget = true;
        yield return null;
	}
}
