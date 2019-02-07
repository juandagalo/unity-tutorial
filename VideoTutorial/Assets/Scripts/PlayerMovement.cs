using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public Rigidbody rb;
	public float forwardForce = 2000f;
	public float sidewayForce = 500f;

	
	// Update is called once per frame
	void FixedUpdate () {
		rb.AddForce(0, 0, forwardForce * Time.deltaTime);	

		if (Input.GetKey("d"))
		{
			rb.AddForce(sidewayForce * Time.deltaTime, 0, 0);
			Communications.SendImmediatly("UnityTrigger1", 1);
			Communications.SendImmediatly("UnityTrigger2", 0);		
		}
		if (Input.GetKey("a"))
		{
			rb.AddForce(-sidewayForce * Time.deltaTime, 0, 0);	
			Communications.SendImmediatly("UnityTrigger2", 1);	
			Communications.SendImmediatly("UnityTrigger1", 0);		
		}	
		
	}

	public void SendTrigger()
    {
        Communications.Send("UnityTrigger1", 1);
    }
}
