using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLockOn : MonoBehaviour
{
    public string buttonLockOn;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown(buttonLockOn)) Debug.Log("Lock On Button Pressed");
    }
}
