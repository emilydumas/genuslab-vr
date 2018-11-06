using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDriver : MonoBehaviour {

	public string eventName;
	public bool continuous;

	void Start(){
		
		
	}
	// Update is called once per frame
	
	void OnTriggerEnter(Collider other)
	{	
			if(!continuous){
				EventHandler.TriggerEvent(eventName);
			}
			
				
	}	
	 void OnTriggerStay(Collider other){
		if(continuous){
				EventHandler.TriggerEvent(eventName);
			}
	}
	

}