using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicationExasmple : MonoBehaviour {

    //public Toggle toggleTest;

	// Use this for initialization
	void Start () {
		
	}

    public void SendTrigger()
    {
        Communications.Send("UnityTrigger1", 1);
    }
}
