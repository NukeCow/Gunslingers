using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLockOn : MonoBehaviour
{
    public int buttonLockOn; //The id for the button assigned to toggling the player's lock on

    [Header("Lock On Settings")]
    [SerializeField] float targetCheckRadius; //The radius of the sphere used to find lock on targets. Acts as the maximum distance a potential lock on target can be from the player.
    [SerializeField] float targetMinViewAngle; //The minimum angle from the player a potential target can be to be considered a valid lock on target.
    [SerializeField] float targetMaxViewAngle; //The maximum angle from the player a potential target can be to be considered a valid lock on target.
    [SerializeField] float targetMaxDist; //The maximum distance a lock on target can be from the player.
    [SerializeField] LayerMask targetCheckMask; //The layer the sphere used to find lock on targets will check. Filters out unnecessary colliders.
    bool isLockedOn = false; //Is the player already locked on to an enemy?

    [Header("Object References")]
    [SerializeField] GameObject pObject; //Reference to the player game object
    [SerializeField] GameObject pCam; //Reference to the player's camera game object

    // Update is called once per frame
    void Update()
    {
        //Check if current target is dead
        //If current target is dead, try to find new target and lock on to it. Otherwise, unlock

        if (Input.GetMouseButtonDown(buttonLockOn))
		{
			if (isLockedOn)
			{
                //Unlock
			}
			else
			{
                //Lock On
			}
		}
    }

    public void FindLockTargets()
	{
        //Find Possible Targets In Range
        float _shortDist = Mathf.Infinity; //The shortest distance a potential target is from the player
        float _shortDistR = Mathf.Infinity; //The shortest distance to the right relative to the player a potential target is from the current target
        float _shortDistL = Mathf.NegativeInfinity; //The shortest distance to the left relative to the player a potential target is from the current target

        Collider[] _colliders = Physics.OverlapSphere(pObject.transform.position, targetCheckRadius, targetCheckMask);
        for (int i = 0; i < _colliders.Length; i++)
		{
            GameObject _target = _colliders[i].gameObject;

            Vector3 _targetDir = _target.transform.position - pObject.transform.position;
            float _targetDist = Vector3.Distance(pObject.transform.position, _target.transform.position);
            float _targetViewAngle = Vector3.Angle(_targetDir, pCam.transform.forward);

            //If target is dead, continue
            if (_targetDist > targetMaxDist) continue;
            if(_targetViewAngle >= targetMinViewAngle && _targetViewAngle <= targetMaxViewAngle)
			{

			}
		}
        //If not locked on, lock on to closest target
        //If locked on, find closest target to left or right of current target and lock on to that target
	}
}
