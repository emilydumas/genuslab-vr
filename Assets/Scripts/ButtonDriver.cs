using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDriver : MonoBehaviour {

	public string eventName;
	private bool listening;
	public GameObject eventScript;
	private EventHandler eventHandler;

	void Start(){
		eventHandler=eventScript.GetComponent<EventHandler>();
		listening = true;
	}
	// Update is called once per frame
	
	void OnTriggerEnter(Collider other)
	{	
		if (listening){
			eventHandler.handleEvent(eventName);
			listening=false;	
		}
	}
	void OnTriggerStay(Collider other){
		if(eventName == "MoveUp"  || eventName == "MoveDown" || eventName == "MoveRight" || eventName == "MoveLeft") {
			eventHandler.Move(eventName);
		}
	}
	
	void Update(){
		if(listening==false){
			listening=true;
		}
		
	}
}
