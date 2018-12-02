using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDriver : MonoBehaviour {

	public string eventName;
	public bool continuous;
	
	void OnTriggerEnter(Collider other)
	{	
			if(!continuous){
				EventHandler.TriggerEvent(eventName);
				Debug.Log("Event: "+eventName);
			}			
	}	
	 void OnTriggerStay(Collider other){
		if(continuous){
				EventHandler.TriggerEvent(eventName);
			}
	}
}